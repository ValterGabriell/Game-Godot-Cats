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

    // ✅ VARIÁVEIS PARA DETECÇÃO AUTOMÁTICA DO SERVIDOR
    private bool _forceServerMode = false;
    private long _myPlayerId = 0;

    public override void _Ready()
    {
        // Verificação segura do node PlayerList
        if (HasNode("PlayerList"))
            _playerList = GetNode<ItemList>("PlayerList");

        if (HasNode("BtnNextTurn"))
        {
            _btnNextTurn = GetNode<Button>("BtnNextTurn");
            _btnNextTurn.Text = "Aguardando início...";
            _btnNextTurn.Pressed += NextTurnPressed;
        }

        // Conecta ao evento de mudança de estado do jogo
        if (GameState.Instance != null)
        {
            GameState.Instance.GameStateChanged += OnGameStateChanged;
        }

        // ✅ SETUP COM DETECÇÃO INTELIGENTE
        SetupMultiplayerConnection();
    }

    private void SetupMultiplayerConnection()
    {
        // ✅ DETECÇÃO CORRIGIDA: Armazena o ID real e detecta se é servidor
        _myPlayerId = Multiplayer.GetUniqueId();
        bool shouldBeServer = (_myPlayerId == 1);
        bool currentIsServer = Multiplayer.IsServer();

        GD.Print($"🔍 Detecção automática - MyID: {_myPlayerId}, ShouldBeServer: {shouldBeServer}, CurrentIsServer: {currentIsServer}");

        // ✅ CORREÇÃO AUTOMÁTICA: Se deveria ser servidor mas não é, OU se tem peers conectados e não é servidor
        if (shouldBeServer && !currentIsServer)
        {
            GD.Print("⚠️ CORREÇÃO: Player ID 1 deveria ser servidor, forçando modo servidor");
            _forceServerMode = true;
        }
        // ✅ DETECÇÃO ALTERNATIVA: Se tem peers conectados, provavelmente é o servidor
        else if (!currentIsServer && Multiplayer.GetPeers().Length > 0)
        {
            GD.Print("⚠️ CORREÇÃO ALTERNATIVA: Tenho peers conectados, provavelmente sou servidor");
            _forceServerMode = true;
        }

        // ✅ CONFIGURAÇÃO FINAL
        if (Multiplayer.HasMultiplayerPeer())
        {
            Multiplayer.PeerConnected += OnPeerConnected;
            Multiplayer.PeerDisconnected += OnPeerDisconnected;

            // Obtém a lista inicial de jogadores conectados
            GetConnectedPlayers();

            GD.Print($"✅ UiMainGame configurado - MyID: {_myPlayerId}, IsServer: {IsServer()}, ForceServerMode: {_forceServerMode}");
            GD.Print($"📊 Peers conectados: {Multiplayer.GetPeers().Length}");
        }
        else
        {
            GD.PrintErr("❌ Nenhuma configuração de multiplayer encontrada!");
        }
    }

    private void OnGameStateChanged(bool isStarted)
    {
        if (isStarted && _btnNextTurn != null)
        {
            PlayerInfo playerInfo = GameManager.Instance.GetCurrentTurn();
            if (playerInfo != null)
            {
                _btnNextTurn.Text = "Turno atual: " + playerInfo.PlayerName;

                // ✅ Habilita/desabilita o botão baseado no turno
                bool isMyTurn = playerInfo.PlayerID == GetMyPlayerId();
                _btnNextTurn.Disabled = !isMyTurn;
            }
        }
    }

    // ✅ MÉTODO AUXILIAR: Obter meu ID de player (sempre retorna o ID real)
    private long GetMyPlayerId()
    {
        return _myPlayerId;
    }

    // ✅ MÉTODO AUXILIAR: Verificar se sou servidor (com correção automática)
    private bool IsServer()
    {
        // Se forçamos o modo servidor, sempre retorna true
        if (_forceServerMode)
        {
            return true;
        }

        // Caso contrário, usa a detecção normal
        if (MultiplayerManager.Instance != null)
        {
            return MultiplayerManager.Instance.IsServer();
        }
        return Multiplayer.IsServer();
    }

    // ✅ RPC para requisitar mudança de turno (qualquer peer pode chamar)
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void RequestNextTurn()
    {
        long requesterId = Multiplayer.GetRemoteSenderId();

        if (requesterId == 0)
        {
            requesterId = GetMyPlayerId();
        }

        GD.Print($"RequestNextTurn chamado - RequesterId: {requesterId}, IsServer: {IsServer()}, ForceServerMode: {_forceServerMode}");
        GD.Print($"MyID armazenado: {GetMyPlayerId()}, MyID atual: {Multiplayer.GetUniqueId()}");

        // ✅ LÓGICA CORRIGIDA: Só processa se for realmente o servidor
        if (IsServer())
        {
            ProcessTurnRequest(requesterId);
        }
        else
        {
            GD.Print($"❌ Não sou servidor - ignorando requisição. MyID: {GetMyPlayerId()}");
        }
    }

    // ✅ Método separado para processar a requisição (apenas no servidor)
    private void ProcessTurnRequest(long requesterId)
    {
        PlayerInfo currentPlayer = GameManager.Instance.GetCurrentTurn();

        GD.Print($"ProcessTurnRequest - Current player: {currentPlayer?.PlayerID} ({currentPlayer?.PlayerName})");
        GD.Print($"Requester: {requesterId}");

        if (currentPlayer == null)
        {
            GD.PrintErr("Erro: Nenhum jogador atual definido");
            return;
        }

        // Valida se é o jogador correto
        if (requesterId != currentPlayer.PlayerID)
        {
            GD.Print($"Jogador {requesterId} tentou finalizar turno, mas é vez do jogador {currentPlayer.PlayerID}");
            return;
        }

        // ✅ DEBUG: Logs detalhados
        GD.Print($"=== MUDANÇA DE TURNO ===");
        GD.Print($"Jogador atual: {currentPlayer.PlayerID} ({currentPlayer.PlayerName})");

        // Executa a mudança
        GameManager.Instance.NextTurn();

        PlayerInfo nextPlayer = GameManager.Instance.GetCurrentTurn();
        if (nextPlayer != null)
        {
            GD.Print($"Próximo jogador: {nextPlayer.PlayerID} ({nextPlayer.PlayerName})");
            GD.Print($"========================");

            // ✅ ENVIA PARA TODOS OS CLIENTES
            Rpc(nameof(UpdateNextTurnButtonWithData), nextPlayer.PlayerName, nextPlayer.PlayerID);
        }
    }

    private void NextTurnPressed()
    {
        // ✅ ATUALIZA O ID ANTES DE USAR (para garantir consistência)
        _myPlayerId = Multiplayer.GetUniqueId();

        GD.Print($"NextTurnPressed - Meu ID: {GetMyPlayerId()}, IsServer: {IsServer()}");

        // ✅ VALIDAÇÃO LOCAL: Só envia se for realmente meu turno
        PlayerInfo currentPlayer = GameManager.Instance.GetCurrentTurn();
        if (currentPlayer != null && currentPlayer.PlayerID != GetMyPlayerId())
        {
            GD.Print($"❌ Não é meu turno! Atual: {currentPlayer.PlayerID}, Eu: {GetMyPlayerId()}");
            return;
        }

        // Envia requisição para o servidor
        Rpc(nameof(RequestNextTurn));
    }

    // ✅ MODIFICADO: Recebe também o PlayerID para sincronizar o GameManager nos clientes
    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void UpdateNextTurnButtonWithData(string playerName, long playerID)
    {
        GD.Print($"UpdateNextTurnButtonWithData recebido - Player: {playerName} (ID: {playerID})");

        // ✅ IMPORTANTE: Sincroniza o GameManager nos clientes
        if (!IsServer())
        {
            // Busca o PlayerInfo correspondente e atualiza o turno atual no cliente
            var playerInfo = FindPlayerInfoById(playerID);
            if (playerInfo != null)
            {
                // ✅ Usa o método público agora
                GameManager.Instance.SetCurrentTurnPlayer(playerInfo);
            }
        }

        if (_btnNextTurn != null)
        {
            _btnNextTurn.Text = "Turno Atual: " + playerName;

            // Habilita/desabilita baseado no jogador atual
            bool isMyTurn = playerID == GetMyPlayerId();
            _btnNextTurn.Disabled = !isMyTurn;

            // ✅ DEBUG: Log local para verificar
            GD.Print($"UI Atualizada - Meu turno: {isMyTurn}, Meu ID: {GetMyPlayerId()}, Player ID: {playerID}");
        }
    }

    // ✅ NOVO: Método para buscar PlayerInfo por ID
    private PlayerInfo FindPlayerInfoById(long playerID)
    {
        if (GameState.Instance != null && GameState.Instance.ConnectedPlayers.ContainsKey(playerID))
        {
            string playerName = GameState.Instance.ConnectedPlayers[playerID];
            return new PlayerInfo(playerID, playerName, 0);
        }
        return null;
    }

    private void OnPeerConnected(long id)
    {
        // ✅ NOVO: Verifica se pode conectar
        if (GameState.Instance != null && !GameState.Instance.CanPlayerConnect(id))
        {
            // Se for o servidor, desconecta o jogador não autorizado
            if (IsServer())
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
        long myId = GetMyPlayerId();
        var myNick = GetPlayerNameById(myId);
        connectedPlayers[myId] = myNick;

        // Adiciona todos os outros peers
        int index = 1;
        foreach (int peerId in peerIds)
        {
            var nick = GetPlayerNameById(peerId);
            connectedPlayers[peerId] = nick;
            index++;
        }

        UpdatePlayerUI();
    }

    private string GetPlayerNameById(long id)
    {
        // Tenta buscar do GameState primeiro
        if (GameState.Instance != null && GameState.Instance.ConnectedPlayers.ContainsKey(id))
        {
            return GameState.Instance.ConnectedPlayers[id];
        }

        // Se não encontrar, usa nome genérico
        if (id == GetMyPlayerId())
            return "Você";

        return $"Jogador {id}";
    }

    private void DisconnectUnauthorizedPeer(long peerId)
    {
        if (IsServer())
        {
            // Remove o peer da conexão
            Multiplayer.MultiplayerPeer.DisconnectPeer((int)peerId);
        }
    }

    private void UpdatePlayerUI()
    {
        if (_playerList != null)
        {
            _playerList.Clear();

            foreach (var player in connectedPlayers)
            {
                string displayName = player.Value;

                // Adiciona indicadores especiais
                if (player.Key == GetMyPlayerId())
                    displayName += " (VOCÊ)";
                if (player.Key == 1)
                    displayName += " [HOST]";

                _playerList.AddItem(displayName);
            }
        }
    }

    // ✅ NOVO: RPC para notificar rejeição de conexão
    [Rpc(MultiplayerApi.RpcMode.Authority)]
    private void NotifyConnectionRejected(string reason)
    {
        // Aqui você pode mostrar uma mensagem de erro na UI
    }

    public override void _ExitTree()
    {
        // Desconecta os eventos ao sair
        if (Multiplayer.HasMultiplayerPeer())
        {
            Multiplayer.PeerConnected -= OnPeerConnected;
            Multiplayer.PeerDisconnected -= OnPeerDisconnected;
        }
    }
}