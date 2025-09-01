using Godot;
using System;

public partial class MultiplayerSpawner : Godot.MultiplayerSpawner
{
    [Export]
    PackedScene PackedScene;

    public override void _Ready()
    {
        Multiplayer.PeerConnected += OnMultiplayerPeerConnected;
    }

    private void OnMultiplayerPeerConnected(long id)
    {
        if (!Multiplayer.IsServer()) return;
        var player = PackedScene.Instantiate();
        player.Name = $"Player{id}";
        GetNode(this.SpawnPath).CallDeferred("AddChild",player);
    }
}
