using Godot;
using NovoProjetodeJogo;
using System;
using System.Collections.Generic;

public partial class LobbyManager : Node2D
{
    /*COMPONENTES DE TELA*/
    private Button _btnHostearPartida;
    private Button _btnEntrarNaPartida;
    private Button _btnIniciarPartida;
    private TextEdit _textInputNickname;
    private ItemList _playerList;


    /*VARIAVEL DE TRABALHO*/
    private MultiplayerApi multiplayerApi;
    private ENetMultiplayerPeer peer;
    // Dicionário para armazenar jogadores (ID -> Nickname)
    private Dictionary<long, string> connectedPlayers = new Dictionary<long, string>();

    /*CONSTANTES*/
    private const string SERVER_IP = "localhost";
    private const int SERVER_PORT = 7777;

    private const string GAME_SCENE_PATH = "res://Cenas/MainGame/Main.tscn";
    //
    public override void _Ready()
    {
        multiplayerApi = Multiplayer;
        _btnHostearPartida = GetNode<Button>("CanvasLayer/BtnHostearPartida");
        _btnEntrarNaPartida = GetNode<Button>("CanvasLayer/BtnEntrarNaPartida");
        _btnIniciarPartida = GetNode<Button>("CanvasLayer/BtnIniciarPartida");
        _textInputNickname = GetNode<TextEdit>("CanvasLayer/TextInputNickname");
        _playerList = GetNode<ItemList>("CanvasLayer/PlayersList");

        _btnEntrarNaPartida.Pressed += OnBtnEntrarNaPartidaPressed;
        _btnHostearPartida.Pressed += OnBtnHostearPartidaPressed;
        _btnIniciarPartida.Pressed += OnBtnIniciarPartidaPressed;
    }



    // ========== EVENTOS DE CONEXÃO ==========
    private void OnBtnHostearPartidaPressed()
    {
        if (!ValidateNickname()) return;
        peer = new ENetMultiplayerPeer();
        peer.CreateServer(SERVER_PORT);

        MultiplayerPeer.ConnectionStatus connectionStatus = peer.GetConnectionStatus();

        if (connectionStatus == MultiplayerPeer.ConnectionStatus.Connected)
        {
            multiplayerApi.MultiplayerPeer = peer;
            /*
             * O PeerConnected é um sinal (event) do Godot que é disparado 
             * automaticamente sempre que um novo peer (jogador) se conecta ao servidor multiplayer.
             * Quando um cliente se conecta, apenas o servidor recebe o sinal PeerConnected. 
             * O cliente não é notificado sobre outros jogadores que já estavam conectados ou que se conectam depois.
             */
            /*
              Disparo automático do evento
             // Isso acontece AUTOMATICAMENTE pelo Godot, não pelo seu código!
             // Quando um cliente faz: peer.CreateClient(SERVER_IP, SERVER_PORT)
             // O Godot internamente dispara: PeerConnected(id_do_cliente)
              */
            // No método OnBtnHostearPartidaPressed - executado UMA VEZ
            multiplayerApi.PeerConnected += AddPlayer; // ✅ Registra o método AddPlayer para ser chamado
            multiplayerApi.PeerDisconnected += RemovePlayer;  // ✅ Registra o método OnPeerDisconnected para ser chamado

            connectedPlayers[1] = GetNickname(); // Adiciona o host à lista de jogadores conectados
            UpdatePlayerListUI();
        }
    }

    private void OnBtnEntrarNaPartidaPressed()
    {
        if (!ValidateNickname()) return;
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(SERVER_IP, SERVER_PORT);
        multiplayerApi.MultiplayerPeer = peer;

        // Eventos para o cliente

        // Imediatamente após o cliente se conectar com sucesso ao servidor multiplayer
        multiplayerApi.ConnectedToServer += OnClientConnectedToServer;

        //Quando o cliente falha ao tentar se conectar ao servidor multiplaye
        multiplayerApi.ConnectionFailed += OnClientConnectionFailed;

        //Quando outros clientes se conectam depois que este cliente já está conectado

        multiplayerApi.PeerConnected += AddPlayer; // Outros jogadores que se conectam depois

        //Quando outros clientes se desconectam
        multiplayerApi.PeerDisconnected += RemovePlayer; // Jogadores que se desconectam
    }

    private void AddPlayer(long id)
    {
       

        //Apenas o SERVIDOR deve notificar os outros clientes
        if (multiplayerApi.IsServer())
        {
            // SERVIDOR: Novo cliente se conectou
            // Solicita o nickname do novo cliente
            RpcId(id, nameof(RequestNickname));
        }
    }
    private void RemovePlayer(long id)
    {
       

        // Remove da lista local
        if (connectedPlayers.ContainsKey(id))
        {
            connectedPlayers.Remove(id);
            UpdatePlayerListUI();
        }

        // Se for servidor, notifica todos os outros clientes
        //Apenas o SERVIDOR deve notificar os outros clientes
        if (multiplayerApi.IsServer())
        {
            Rpc(nameof(RemovePlayerFromList), id);
        }
    }

    private void OnClientConnectedToServer()
    {
       
        // Cliente envia seu nickname automaticamente para o servidor
        // ❗ ISSO EXECUTA NO CLIENTE, NÃO NO SERVIDOR!
        RpcId(1, nameof(RegisterPlayer), multiplayerApi.GetUniqueId(), GetNickname());
    }

    private void OnClientConnectionFailed()
    {
       
    }

    // ========== FIM DO EVENTOS DE CONEXÃO ==========



    // ========== RPCs ==========
    /*
     RpcMode.Authority :    Quem pode enviar: Apenas o peer com autoridade (normalmente o servidor)
                            Quem pode receber: Todos os outros peers
                            Uso: Comandos do servidor para clientes

    AnyPeer:                Quem pode enviar: Qualquer peer (cliente ou servidor)
                            Quem pode receber: Todos os peers (incluindo o remetente)
                            Uso: Comunicação bidirecional entre clientes e servidor
     */
    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void RequestNickname()
    {
        // Cliente responde com seu nickname
        var myId = multiplayerApi.GetUniqueId();
        //cliente envia seu nickname para o servidor
        RpcId(1, nameof(RegisterPlayer), myId, GetNickname());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void RegisterPlayer(long playerId, string nickname)
    {
        // ❗ AGORA ESTAMOS NO SERVIDOR!
        connectedPlayers[playerId] = nickname; // Servidor atualiza sua lista
        UpdatePlayerListUI();// Servidor atualiza sua UI

        if (multiplayerApi.IsServer()) // ✅ AQUI É TRUE (estamos no servidor)
        {
            // Servidor envia a lista completa para o novo cliente
            var playerIds = new long[connectedPlayers.Count];
            var playerNicknames = new string[connectedPlayers.Count];

            int index = 0;
            foreach (var kvp in connectedPlayers)
            {
                playerIds[index] = kvp.Key;
                playerNicknames[index] = kvp.Value;
                index++;
            }

            // Envia lista completa apenas para o novo cliente
            RpcId(playerId, nameof(ReceiveCompletePlayerList), playerIds, playerNicknames);

            // Notifica todos os outros clientes sobre o novo jogador
            foreach (var existingPlayerId in connectedPlayers.Keys)
            {
                if (existingPlayerId != playerId && existingPlayerId != 1) // Não envia para o novo jogador nem para o servidor
                {
                    RpcId(existingPlayerId, nameof(AddPlayerToList), playerId, nickname);
                }
            }
        }
    }

    [Rpc(MultiplayerApi.RpcMode.Authority)]
    private void ReceiveCompletePlayerList(long[] playerIds, string[] playerNicknames)
    {
        // Cliente recebe a lista completa de jogadores
        connectedPlayers.Clear();

        for (int i = 0; i < playerIds.Length; i++)
        {
            connectedPlayers[playerIds[i]] = playerNicknames[i];
        }

        UpdatePlayerListUI();
       
    }

    [Rpc(MultiplayerApi.RpcMode.Authority)]
    private void AddPlayerToList(long playerId, string nickname)
    {
        // Adiciona um novo jogador à lista local
        connectedPlayers[playerId] = nickname;
        UpdatePlayerListUI();
       
    }

    [Rpc(MultiplayerApi.RpcMode.Authority)]
    private void RemovePlayerFromList(long playerId)
    {
        // Remove jogador da lista local
        if (connectedPlayers.ContainsKey(playerId))
        {
            string nickname = connectedPlayers[playerId];
            connectedPlayers.Remove(playerId);
            UpdatePlayerListUI();
           
        }
    }


    [Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true)]
    private void StartGame()
    {
       
        // Transição para a cena do jogo
        CallDeferred(nameof(SwitchToGameScene));
    }


    // ========== FIM RPCs ==========


    // ========== UI ==========
    private void UpdatePlayerListUI()
    {
        _playerList.Clear();

        foreach (var kvp in connectedPlayers)
        {
            string displayText = $"{kvp.Value} (ID: {kvp.Key})";
            if (kvp.Key == 1) displayText += " [HOST]";
            if (kvp.Key == multiplayerApi.GetUniqueId()) displayText += " [VOCÊ]";

            _playerList.AddItem(displayText);
        }

       
    }
    // ========== FIM DA UI ==========




    /*LOGICA DE NEGOCIO*/

    private void OnBtnIniciarPartidaPressed()
    {
        // Somente o host pode iniciar a partida
        if (!multiplayerApi.IsServer())
        {
           
            return;
        }

        // Verifica se há jogadores suficientes para iniciar
        if (connectedPlayers.Count < 2) // Ajuste conforme necessário
        {
           
            return;
        }

       
        // Envia comando para todos os clientes mudarem de cena
        Rpc(nameof(StartGame));
    }
    private bool ValidateNickname()
    {
        string nickname = GetNickname();
        if (string.IsNullOrEmpty(nickname))
        {
           
            return false;
        }
        if (nickname.Length < 3 || nickname.Length > 15)
        {
           
            return false;
        }
       
        return true;
    }

    private string GetNickname()
    {
        return _textInputNickname.Text.Trim();
    }

    private void SwitchToGameScene()
    {
       

        // Carrega a cena do jogo PRIMEIRO
        PackedScene gameScene = ResourceLoader.Load<PackedScene>(GAME_SCENE_PATH);

        if (gameScene == null)
        {
            return;
        }

        // Instancia a nova cena
        Node gameSceneInstance = gameScene.Instantiate();

        // ✅ SALVA A REFERÊNCIA DA CENA ATUAL ANTES DE QUALQUER OPERAÇÃO
        var currentScene = GetTree().CurrentScene;

        // ✅ ADICIONA A NOVA CENA À ÁRVORE (FORÇA O _Ready() A EXECUTAR)
        GetTree().Root.AddChild(gameSceneInstance);

        // ✅ DEFINE A NOVA CENA COMO ATUAL
        GetTree().CurrentScene = gameSceneInstance;

        // ✅ AGORA verifica se GameState foi inicializado (APÓS adicionar à árvore)
        if (GameState.Instance == null)
        {
            return;
        }

        // ✅ DEBUG: Imprime os jogadores que serão passados para o GameState
       
        foreach (var player in connectedPlayers)
        {
           
        }

        // Salva os jogadores originais no GameState
        GameState.Instance.StartGame(connectedPlayers);

        // ✅ REMOVE A CENA ANTIGA DE FORMA SEGURA
        if (currentScene != null)
        {
            GetTree().Root.RemoveChild(currentScene);
            currentScene.QueueFree();
        }

       
    }

}
