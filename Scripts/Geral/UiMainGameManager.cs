using Godot;
using System;
using System.Linq;

public partial class UiMainGameManager : Node2D
{
    public override void _Ready()
    {
        // Conecta aos sinais de multiplayer para rastrear conexões em tempo real
        if (Multiplayer.HasMultiplayerPeer())
        {
            Multiplayer.PeerConnected += OnPeerConnected;
            Multiplayer.PeerDisconnected += OnPeerDisconnected;
            Multiplayer.ConnectedToServer += OnConnectedToServer;
            Multiplayer.ConnectionFailed += OnConnectionFailed;
            Multiplayer.ServerDisconnected += OnServerDisconnected;
        }

        // Aguarda um frame para garantir que todas as conexões sejam processadas
        CallDeferred(nameof(DelayedPlayerCheck));
    }

    // Método chamado após um delay para garantir sincronização
    private void DelayedPlayerCheck()
    {
        GD.Print("=== VERIFICAÇÃO INICIAL DE JOGADORES ===");
        LogConnectedPlayers();
    }

    // Sinal chamado quando um peer se conecta
    private void OnPeerConnected(long id)
    {
        GD.Print($"[SIGNAL] Peer conectado: {id}");
        // Aguarda um pouco para atualizar a contagem
        GetTree().CreateTimer(0.1).Timeout += LogConnectedPlayers;
    }

    // Sinal chamado quando um peer se desconecta
    private void OnPeerDisconnected(long id)
    {
        GD.Print($"[SIGNAL] Peer desconectado: {id}");
        LogConnectedPlayers();
    }

    // Sinal chamado quando cliente se conecta ao servidor
    private void OnConnectedToServer()
    {
        GD.Print("[SIGNAL] Conectado ao servidor");
        // Aguarda um pouco para sincronização completa
        GetTree().CreateTimer(0.5).Timeout += LogConnectedPlayers;
    }

    private void OnConnectionFailed()
    {
        GD.Print("[SIGNAL] Falha na conexão");
    }

    private void OnServerDisconnected()
    {
        GD.Print("[SIGNAL] Servidor desconectado");
    }

    public void LogConnectedPlayers()
    {
        if (Multiplayer.HasMultiplayerPeer())
        {
            // Obtém todos os IDs dos peers conectados
            var connectedPeers = Multiplayer.GetPeers();
            GD.Print("=== JOGADORES CONECTADOS ===");
            GD.Print($"Total de peers conectados: {connectedPeers.Length}");

            // Log de cada jogador conectado
            foreach (int peerId in connectedPeers)
            {
                GD.Print($"Peer ID: {peerId}");
            }

            // Informações do jogador local
            GD.Print($"ID Local: {Multiplayer.GetUniqueId()}");
            GD.Print($"É servidor: {Multiplayer.IsServer()}");

            // Contagem total incluindo o jogador local
            int totalPlayers = connectedPeers.Length + 1; // +1 para incluir o próprio jogador
            GD.Print($"Total de jogadores (incluindo local): {totalPlayers}");

            GD.Print("=== FIM DA LISTA ===");
        }
        else
        {
            GD.Print("Nenhum multiplayer peer configurado. Jogo em modo single player.");
        }
    }

    // Método para ser chamado quando você quiser verificar jogadores conectados
    public void CheckConnectedPlayers()
    {
        LogConnectedPlayers();
    }

    // Método para obter informações mais detalhadas dos jogadores
    public void LogDetailedPlayerInfo()
    {
        if (!Multiplayer.HasMultiplayerPeer()) return;

        var connectedPeers = Multiplayer.GetPeers();
        GD.Print("=== INFORMAÇÕES DETALHADAS DOS JOGADORES ===");

        // Jogadores conectados (peers)
        for (int i = 0; i < connectedPeers.Length; i++)
        {
            int peerId = connectedPeers[i];
            GD.Print($"Jogador {i + 1}:");
            GD.Print($"  - ID: {peerId}");
            GD.Print($"  - É autoridade remota: {Multiplayer.GetRemoteSenderId() == peerId}");
        }

        // Informações do jogador local
        GD.Print($"Jogador Local:");
        GD.Print($"  - ID: {Multiplayer.GetUniqueId()}");
        GD.Print($"  - É servidor: {Multiplayer.IsServer()}");
        GD.Print($"=== FIM DAS INFORMAÇÕES ===");
    }

    public override void _ExitTree()
    {
        // Desconecta os sinais ao sair
        if (Multiplayer.HasMultiplayerPeer())
        {
            Multiplayer.PeerConnected -= OnPeerConnected;
            Multiplayer.PeerDisconnected -= OnPeerDisconnected;
            Multiplayer.ConnectedToServer -= OnConnectedToServer;
            Multiplayer.ConnectionFailed -= OnConnectionFailed;
            Multiplayer.ServerDisconnected -= OnServerDisconnected;
        }
    }
}