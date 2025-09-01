using Godot;

namespace NovoProjetodeJogo.Scripts
{
    public partial class UIHandler :Node2D
    {

        private Node _mainGameScene;
        private MultiplayerApi multiplayerApi;
        private Button _BtnJoin;
        private Button _BtnHost;
        private const string SERVER_IP = "localhost";
        private const int SERVER_PORT = 7777;
        private ENetMultiplayerPeer peer;
        private int currentPlayMoney = 2;


        [Export]
        public PackedScene PackedScene { get; set; }

        [Export]
        public TextEdit NicknameTextEdit { get; set; }


        public override void _Ready()
        {
            multiplayerApi = Multiplayer;
            _BtnJoin = GetParent().GetNode<Button>("UIManager/BtnJoin");
            _BtnHost = GetParent().GetNode<Button>("UIManager/BtnHost");
            _mainGameScene = ResourceLoader.Load<PackedScene>("res://Main.tscn").Instantiate();

            _BtnJoin.Pressed += OnBntJoinPressed;
            _BtnHost.Pressed += OnBntHostPressed;
        }
        public void IncreaseCurrentPlayMoney(int amount)
        {
            currentPlayMoney += amount;
            GD.Print("Money increased by " + amount + ". Total money: " + currentPlayMoney);
        }

        public void DecreaseCurrentPlayMoney(int amount)
        {
            currentPlayMoney -= amount;
            GD.Print("Money decreased by " + amount + ". Total money: " + currentPlayMoney);
        }

        public void OnBntJoinPressed()
        {
            if(!ValidateNickname()) return;
            peer = new ENetMultiplayerPeer();
            peer.CreateClient(SERVER_IP, SERVER_PORT);
            multiplayerApi.MultiplayerPeer = peer;

            // Para clientes, verificar se a conexão foi estabelecida
            multiplayerApi.ConnectedToServer += OnConnectedToServer;
            multiplayerApi.ConnectionFailed += OnConnectionFailed;
        }

        public void OnBntHostPressed()
        {
            if (!ValidateNickname()) return;
            peer = new ENetMultiplayerPeer();
            peer.CreateServer(SERVER_PORT);

            MultiplayerPeer.ConnectionStatus connectionStatus = peer.GetConnectionStatus();

            if (connectionStatus == MultiplayerPeer.ConnectionStatus.Connected)
            {
                multiplayerApi.MultiplayerPeer = peer;
                multiplayerApi.PeerConnected += AddPlayer;
                SwitchToMainGameScene();
            }
        }

        private void OnConnectedToServer()
        {
            GD.Print("Successfully connected to server as client");
            SwitchToMainGameScene();
        }

        private void OnConnectionFailed()
        {
            GD.Print("Failed to connect to server");
        }

        private void SwitchToMainGameScene()
        {
            GetTree().Root.AddChild(_mainGameScene);
            var currentScene = GetParent();
            GetTree().Root.RemoveChild(currentScene);
        }

        private bool ValidateNickname()
        {
            string nickname = GetNickname();
            if (string.IsNullOrEmpty(nickname))
            {
                GD.Print("Nickname cannot be empty.");
                return false;
            }
            if (nickname.Length < 3 || nickname.Length > 15)
            {
                GD.Print("Nickname must be between 3 and 15 characters.");
                return false;
            }
            GD.Print("Nickname is valid: " + nickname);
            return true;
        }

        private string GetNickname()
        {
            return NicknameTextEdit.Text.Trim();
        }

        public void AddPlayer(long id)
        {
            string nickname = GetNickname();
            var player = PackedScene.Instantiate();
            player.Name = nickname + " - " + id.ToString();
            CallDeferred("add_child", player);
        }
    }
}
