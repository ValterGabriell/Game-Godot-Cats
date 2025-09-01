using Godot;
using Multiplayer;
using System;

public partial class HighLevelNetwork : Node
{
    private const string SERVER_IP = "";
    private const int SERVER_PORT = 7777;
    private ENetMultiplayerPeer peer;

    public void StartServer()
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateServer(SERVER_PORT);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Servidor iniciado na porta " + SERVER_PORT);
    }

    public void StartClient()
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(SERVER_IP, SERVER_PORT);
        Multiplayer.MultiplayerPeer = peer;
        GD.Print("Cliente conectando na porta " + SERVER_PORT);
    }
}
