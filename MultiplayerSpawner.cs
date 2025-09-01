using Godot;
using System;

public partial class MultiplayerSpawner : Godot.MultiplayerSpawner
{
    [Export]
    PackedScene PackedScene;

    Node spawnNode;
    public override void _Ready()
    {
        var player = PackedScene.Instantiate();
        player.Name = $"Player teste";
        spawnNode = GetNode(this.SpawnPath);
        spawnNode.CallDeferred("add_child", player); // <- aqui corrigido!
        Multiplayer.PeerConnected += OnMultiplayerPeerConnected;
    }

    private void OnMultiplayerPeerConnected(long id)
    {
        if (!Multiplayer.IsServer()) return;
        var player = PackedScene.Instantiate();
        player.Name = $"Player{id}";
        GD.Print($"Player {id} connected");
        GD.Print(this.SpawnPath);
        GD.Print(spawnNode.Name);
        spawnNode.CallDeferred("add_child", player); // <- aqui corrigido!
    }
}
