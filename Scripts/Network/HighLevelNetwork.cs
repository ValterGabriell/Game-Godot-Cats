using Godot;
using System;

public partial class HighLevelNetwork : Node
{
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 7777;
    private ENetMultiplayerPeer peer;




    public void StartServer(MultiplayerApi multiplayerApi, PackedScene _)
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateServer(SERVER_PORT);
        multiplayerApi.MultiplayerPeer = peer; // <-- funciona em Godot 4.2+
        GD.Print("Servidor iniciado na porta " + SERVER_PORT);
    }

    public void StartClient(MultiplayerApi multiplayerApi, PackedScene packedScene
        , NodePath spawnPath)
    {
        peer = new ENetMultiplayerPeer();
        peer.CreateClient(SERVER_IP, SERVER_PORT);

        multiplayerApi.MultiplayerPeer = peer; // <-- funciona em Godot 4.2+
        GD.Print("Cliente conectando na porta " + SERVER_PORT);

        if (!multiplayerApi.IsServer()) return;
        var player = packedScene.Instantiate();
        player.Name = $"Player1";
        GetNode(spawnPath).CallDeferred("add_child", player); // <- aqui corrigido!
    }
}