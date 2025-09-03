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
    private Node2D _mainGameNode;


    /*VARIAVEL DE TRABALHO*/
    private MultiplayerApi multiplayerApi;
    private ENetMultiplayerPeer peer;
    // Dicionário para armazenar jogadores (ID -> Nickname)
    private Dictionary<long, string> connectedPlayers = new Dictionary<long, string>();

    /*CONSTANTES*/
    private const string SERVER_IP = "localhost";
    private const int SERVER_PORT = 7777;

    //
    public override void _Ready()
    {
        multiplayerApi = Multiplayer;
        _btnHostearPartida = GetNode<Button>("CanvasLayer/BtnHostearPartida");
        _btnEntrarNaPartida = GetNode<Button>("CanvasLayer/BtnEntrarNaPartida");
        _btnIniciarPartida = GetNode<Button>("CanvasLayer/BtnIniciarPartida");
        _textInputNickname = GetNode<TextEdit>("CanvasLayer/TextInputNickname");
        _mainGameNode = GetNode<Node2D>("MainGame");
        _playerList = GetNode<ItemList>("CanvasLayer/PlayersList");
    
        _btnEntrarNaPartida.Pressed += OnBtnEntrarNaPartidaPressed;
        _btnHostearPartida.Pressed += OnBtnHostearPartidaPressed;
        _btnIniciarPartida.Pressed += OnBtnIniciarPartidaPressed;
    }



    // ========== EVENTOS DE CONEXÃO ==========
    private void OnBtnHostearPartidaPressed()
    {
        if (!ValidateNickname()) return;

        // ✅ USA O SINGLETON
        MultiplayerManager.Instance.SetupServer(SERVER_PORT);

        multiplayerApi = MultiplayerManager.Instance.GetMultiplayerApi();

        if (multiplayerApi.HasMultiplayerPeer())
        {
            multiplayerApi.PeerConnected += AddPlayer;
            multiplayerApi.PeerDisconnected += RemovePlayer;

            connectedPlayers[1] = GetNickname();
            UpdatePlayerListUI();

            GD.Print($"✅ Host configurado - IsServer: {multiplayerApi.IsServer()}");
        }
    }

    private void OnBtnEntrarNaPartidaPressed()
    {
        if (!ValidateNickname()) return;

        // ✅ USA O SINGLETON
        MultiplayerManager.Instance.SetupClient(SERVER_IP, SERVER_PORT);

        multiplayerApi = MultiplayerManager.Instance.GetMultiplayerApi();

        if (multiplayerApi.HasMultiplayerPeer())
        {
            multiplayerApi.ConnectedToServer += OnClientConnectedToServer;
            multiplayerApi.ConnectionFailed += OnClientConnectionFailed;
            multiplayerApi.PeerConnected += AddPlayer;
            multiplayerApi.PeerDisconnected += RemovePlayer;

            GD.Print($"✅ Cliente configurado - IsServer: {multiplayerApi.IsServer()}");
        }
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
        GD.Print("Mudando para a cena do jogo...");
        GD.Print(_mainGameNode);

        if (_mainGameNode == null)
        {
            return;
        }

        this.Visible = false;
        _mainGameNode.Visible = true;
    }

}
