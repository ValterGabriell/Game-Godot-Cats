using Godot;

public partial class MultiplayerManager : Node
{
    public static MultiplayerManager Instance { get; private set; }

    private ENetMultiplayerPeer _peer;
    private bool _isServer = false;

    public override void _Ready()
    {
        Instance = this;

        // ✅ IMPORTANTE: Configura como autoload (singleton)
        // No Godot: Project -> Project Settings -> AutoLoad
        // Script Path: res://Scripts/Geral/MultiplayerManager.cs
        // Node Name: MultiplayerManager
    }

    public void SetupServer(int port)
    {
        _peer = new ENetMultiplayerPeer();
        _peer.CreateServer(port);

        GetTree().GetMultiplayer().MultiplayerPeer = _peer;
        _isServer = true;

        GD.Print($"✅ Servidor criado - IsServer: {GetTree().GetMultiplayer().IsServer()}");
    }

    public void SetupClient(string ip, int port)
    {
        _peer = new ENetMultiplayerPeer();
        _peer.CreateClient(ip, port);

        GetTree().GetMultiplayer().MultiplayerPeer = _peer;
        _isServer = false;

        GD.Print($"✅ Cliente conectado - IsServer: {GetTree().GetMultiplayer().IsServer()}");
    }

    public bool IsConfigured()
    {
        return _peer != null && GetTree().GetMultiplayer().HasMultiplayerPeer();
    }

    public bool IsServer()
    {
        return _isServer && GetTree().GetMultiplayer().IsServer();
    }

    public void EnsureConfiguration()
    {
        if (_peer != null && !GetTree().GetMultiplayer().HasMultiplayerPeer())
        {
            GetTree().GetMultiplayer().MultiplayerPeer = _peer;
            GD.Print("✅ Multiplayer reconfigurado após mudança de cena");
        }

        // Verifica se a configuração está correta
        GD.Print($"Estado atual - IsServer: {GetTree().GetMultiplayer().IsServer()}, MyID: {GetTree().GetMultiplayer().GetUniqueId()}");
    }

    public MultiplayerApi GetMultiplayerApi()
    {
        return GetTree().GetMultiplayer();
    }
}