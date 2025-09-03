using Godot;
using NovoProjetodeJogo;
using NovoProjetodeJogo.Cenas.MainGame;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class UiMainGame : Node2D
{
    // Dicionário para armazenar informações dos jogadores
    private Dictionary<long, string> connectedPlayers = new Dictionary<long, string>();

    /*COMPONENTES UI*/
    private ItemList _playerList;
    private Button _btnNextTurn;

    public override void _Ready()

    { 
        // Verificação segura do node PlayerList
        if (HasNode("PlayerList"))
            _playerList = GetNode<ItemList>("PlayerList");

        if (HasNode("BtnNextTurn"))
        {
            _btnNextTurn = GetNode<Button>("BtnNextTurn");
            PlayerInfo playerInfo = GameManager.Instance.GetCurrentTurn();
            
            _btnNextTurn.Text = "Próximo Turno De: ";
            _btnNextTurn.Pressed += NextTurnPressed;
        }


        // Conecta aos eventos de multiplayer para monitorar mudanças

        /*
         O que ele faz:
•	Retorna true: Se existe um MultiplayerPeer configurado e ativo (seja como servidor ou cliente)
•	Retorna false: Se não há nenhuma conexão multiplayer configurada (modo single player)

        1.	Quando você é o servidor: Após executar multiplayerApi.MultiplayerPeer = peer no LobbyManager
2.	Quando você é um cliente: Após se conectar com sucesso a um servidor
3.	Durante o jogo: Enquanto a conexão multiplayer estiver ativa

     */
        if (Multiplayer.HasMultiplayerPeer())
        {
            Multiplayer.PeerConnected += OnPeerConnected;
            Multiplayer.PeerDisconnected += OnPeerDisconnected;

            // ✅ NOVO: Conecta aos eventos do GameState
            if (GameState.Instance != null)
            {
                GameState.Instance.PlayerReconnected += OnPlayerReconnected;
            }
            // Obtém a lista inicial de jogadores conectados
            GetConnectedPlayers();
        }
    }

    private void NextTurnPressed()
    {
        
        GameManager.Instance.NextTurn();
    }

    private void OnPlayerReconnected(long playerId, string playerName)
    {
        
        // Aqui você pode adicionar efeitos visuais, notificações, etc.
        ShowReconnectionNotification(playerName);
    }


    private void OnPeerConnected(long id)
    {
        

        // ✅ NOVO: Verifica se pode conectar
        if (GameState.Instance != null && !GameState.Instance.CanPlayerConnect(id))
        {
           
            // Se for o servidor, desconecta o jogador não autorizado
            if (Multiplayer.IsServer())
            {
                // Envia mensagem de rejeição antes de desconectar
                RpcId(id, nameof(NotifyConnectionRejected), "Partida em andamento. Apenas jogadores originais podem reconectar.");

                // Desconecta o peer não autorizado
                CallDeferred(nameof(DisconnectUnauthorizedPeer), id);
            }
            return;
        }

        

        // Atualiza GameState
        if (GameState.Instance != null)
        {
            // Busca o nome do jogador (pode ser necessário solicitar via RPC)
            string playerName = GetPlayerNameById(id);
            GameState.Instance.PlayerConnected(id, playerName);
        }

        // Atualiza a lista quando um novo jogador se conecta
        GetConnectedPlayers();
    }

    private void OnPeerDisconnected(long id)
    {
        

        // Atualiza GameState
        if (GameState.Instance != null)
        {
            GameState.Instance.PlayerDisconnected(id);
        }

        // Remove da lista local
        if (connectedPlayers.ContainsKey(id))
        {
            connectedPlayers.Remove(id);
        }
        UpdatePlayerUI();
    }


    private void GetConnectedPlayers()
    {
        if (!Multiplayer.HasMultiplayerPeer()) return;

        // Limpa a lista atual
        connectedPlayers.Clear();

        // Se o GameState existe e o jogo começou, usa a lista do GameState
        if (GameState.Instance != null && GameState.Instance.IsGameStarted)
        {
            

            // Usa a lista do GameState que já tem todos os jogadores
            foreach (var player in GameState.Instance.ConnectedPlayers)
            {
                connectedPlayers[player.Key] = player.Value;
                
            }

            UpdatePlayerUI();

            
            return;
        }

        

        // Modo normal (lobby ou sem GameState)
        var peerIds = Multiplayer.GetPeers();

        // ✅ ADICIONADO: Inclui o próprio jogador local
        long myId = Multiplayer.GetUniqueId();
        var myNick = GetPlayerNameById(myId);
        connectedPlayers[myId] = myNick;
        

        // Adiciona todos os outros peers
        int index = 1; // Começa do index 1 pois já adicionamos o jogador local
        foreach (int peerId in peerIds)
        {
            var nick = GetPlayerNameById(peerId);
            connectedPlayers[peerId] = nick;
            

            var player = new PlayerInfo
            {
                PlayerID = peerId,
                PlayerName = nick,
                PlayerOrder = index
            };

            index++;
        }

        UpdatePlayerUI();

        
        foreach (var player in connectedPlayers)
        {
            
        }
    }


    private string GetPlayerNameById(long id)
    {
        // Tenta buscar do GameState primeiro
        if (GameState.Instance != null && GameState.Instance.ConnectedPlayers.ContainsKey(id))
        {
            return GameState.Instance.ConnectedPlayers[id];
        }

        // Se não encontrar, usa nome genérico
        if (id == Multiplayer.GetUniqueId())
            return "Você";

        return $"Jogador {id}";
    }

    private void DisconnectUnauthorizedPeer(long peerId)
    {
        if (Multiplayer.IsServer())
        {
            // Remove o peer da conexão
            Multiplayer.MultiplayerPeer.DisconnectPeer((int)peerId);
            
        }
    }

    private void ShowReconnectionNotification(string playerName)
    {
        // Aqui você pode mostrar uma notificação na UI
        
        // Exemplo: mostrar toast, som de notificação, etc.
    }

    private void UpdatePlayerUI()
    {
        if (_playerList != null)
        {
            _playerList.Clear();
            int index = 0;

            

            foreach (var player in connectedPlayers)
            {
                string displayName = player.Value;

                // Adiciona indicadores especiais
                if (player.Key == Multiplayer.GetUniqueId())
                    displayName += " (VOCÊ)";
                if (player.Key == 1)
                    displayName += " [HOST]";

                _playerList.AddItem(displayName);
                
                index++;
            }
        }
        else
        {
            
        }
    }

    // ✅ NOVO: RPC para notificar rejeição de conexão
    [Rpc(MultiplayerApi.RpcMode.Authority)]
    private void NotifyConnectionRejected(string reason)
    {
        
        // Aqui você pode mostrar uma mensagem de erro na UI
        // Por exemplo: popup, toast, etc.
    }

    public override void _ExitTree()
    {
        // Desconecta os eventos ao sair
        if (Multiplayer.HasMultiplayerPeer())
        {
            Multiplayer.PeerConnected -= OnPeerConnected;
            Multiplayer.PeerDisconnected -= OnPeerDisconnected;
        }

        if (GameState.Instance != null)
        {
            GameState.Instance.PlayerReconnected -= OnPlayerReconnected;
        }
    }
}