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
        
        LogConnectedPlayers();
    }

    // Sinal chamado quando um peer se conecta
    private void OnPeerConnected(long id)
    {
        
        // Aguarda um pouco para atualizar a contagem
        GetTree().CreateTimer(0.1).Timeout += LogConnectedPlayers;
    }

    // Sinal chamado quando um peer se desconecta
    private void OnPeerDisconnected(long id)
    {
        
        LogConnectedPlayers();
    }

    // Sinal chamado quando cliente se conecta ao servidor
    private void OnConnectedToServer()
    {
        
        // Aguarda um pouco para sincronização completa
        GetTree().CreateTimer(0.5).Timeout += LogConnectedPlayers;
    }

    private void OnConnectionFailed()
    {
        
    }

    private void OnServerDisconnected()
    {
        
    }

    public void LogConnectedPlayers()
    {
        if (Multiplayer.HasMultiplayerPeer())
        {
            // Obtém todos os IDs dos peers conectados
            var connectedPeers = Multiplayer.GetPeers();
            
            

            // Log de cada jogador conectado
            foreach (int peerId in connectedPeers)
            {
                
            }

            // Informações do jogador local
            
            

            // Contagem total incluindo o jogador local
            int totalPlayers = connectedPeers.Length + 1; // +1 para incluir o próprio jogador
            

            
        }
        else
        {
            
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
        

        // Jogadores conectados (peers)
        for (int i = 0; i < connectedPeers.Length; i++)
        {
            int peerId = connectedPeers[i];
            
            
            
        }

        // Informações do jogador local
        
        
        
        
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