using Godot;
using System;
using System.Collections.Generic;

public partial class UiMainGame : Node2D
{
    // Dicionário para armazenar informações dos jogadores
    private Dictionary<long, string> connectedPlayers = new Dictionary<long, string>();
    private List<long> orderPlayers = [];

    private ItemList _playerList;

    public override void _Ready()
    {
        // Verificação segura do node PlayerList
        if (HasNode("PlayerList"))
        {
            _playerList = GetNode<ItemList>("PlayerList");
        }
        else
        {
            GD.PrintErr("Node 'PlayerList' não encontrado na cena. Verifique a estrutura da cena.");
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


    private void OnPlayerReconnected(long playerId, string playerName)
    {
        GD.Print($"🔄 RECONEXÃO BEM-SUCEDIDA: {playerName} (ID: {playerId})");
        // Aqui você pode adicionar efeitos visuais, notificações, etc.
        ShowReconnectionNotification(playerName);
    }


    private void OnPeerConnected(long id)
    {
        GD.Print($"Tentativa de conexão do peer: {id}");

        // ✅ NOVO: Verifica se pode conectar
        if (GameState.Instance != null && !GameState.Instance.CanPlayerConnect(id))
        {
            GD.Print($"CONEXÃO REJEITADA: Peer {id} tentou entrar durante a partida (não é jogador original)");

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

        GD.Print($"Peer {id} AUTORIZADO a conectar");

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
        GD.Print($"Jogador desconectado: {id}");

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
            // Mostra todos os jogadores originais, marcando os desconectados
            foreach (var originalPlayerId in GameState.Instance.OriginalPlayers)
            {
                bool isConnected = GameState.Instance.ConnectedPlayers.ContainsKey(originalPlayerId);
                string playerName = GameState.Instance.ConnectedPlayers.GetValueOrDefault(originalPlayerId, $"Jogador {originalPlayerId}");

                if (isConnected)
                {
                    connectedPlayers[originalPlayerId] = playerName;

                    orderPlayers.Add(originalPlayerId);
                }
                else
                {
                    // Marca como desconectado
                    connectedPlayers.Remove(originalPlayerId);
                }
            }
        }
        else
        {
            // Modo normal (lobby ou sem GameState)
            var peerIds = Multiplayer.GetPeers();
            connectedPlayers[Multiplayer.GetUniqueId()] = "Você";

            foreach (int peerId in peerIds)
            {
                connectedPlayers[peerId] = GetPlayerNameById(peerId);
            }
        }

        UpdatePlayerUI();

        GD.Print($"Total de jogadores: {connectedPlayers.Count}");
        foreach (var player in connectedPlayers)
        {
            GD.Print($"ID: {player.Key}, Nome: {player.Value}");
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
            GD.Print($"Peer não autorizado {peerId} foi desconectado");
        }
    }

    private void ShowReconnectionNotification(string playerName)
    {
        // Aqui você pode mostrar uma notificação na UI
        GD.Print($"🎉 {playerName} voltou à partida!");
        // Exemplo: mostrar toast, som de notificação, etc.
    }

    private void UpdatePlayerUI()
    {
        if (_playerList != null)
        {
            _playerList.Clear();
            foreach (var player in connectedPlayers)
            {
                var ordem = orderPlayers.IndexOf(player.Key) + 1;
                _playerList.AddItem($"{player.Value} - Ordem: {ordem}");
            }
        }
    }

    // ✅ NOVO: RPC para notificar rejeição de conexão
    [Rpc(MultiplayerApi.RpcMode.Authority)]
    private void NotifyConnectionRejected(string reason)
    {
        GD.PrintErr($"Conexão rejeitada: {reason}");
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