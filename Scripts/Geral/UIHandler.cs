using Godot;

namespace NovoProjetodeJogo.Scripts
{
    public partial class UIHandler :Node2D
    {
        RichTextLabel textMoneyCurrentPlayerLabel;
        Button btnServer;
        Button btnClient;
        int currentPlayMoney = 2;
        MultiplayerApi multiplayerApi;

        [Export]
        public PackedScene PackedScene { get; set; }


        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 7777;
        private ENetMultiplayerPeer peer;

        public override void _Ready()
        {
            multiplayerApi = Multiplayer;
            textMoneyCurrentPlayerLabel = GetParent().GetNode<RichTextLabel>("UIManager/CountMoney");
            btnServer = GetParent().GetNode<Button>("UIManager/BtnServer");
            btnClient = GetParent().GetNode<Button>("UIManager/BtnConnect");
            textMoneyCurrentPlayerLabel.Text = "Money: " + currentPlayMoney.ToString();
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

        public void OnBntClientPressed()
        {
            peer = new ENetMultiplayerPeer();
            peer.CreateClient(SERVER_IP, SERVER_PORT);
            multiplayerApi.MultiplayerPeer = peer;
            if (!multiplayerApi.IsServer()) return;
        }

        public void OnBntServerPressed()
        {
            peer = new ENetMultiplayerPeer();
            peer.CreateServer(SERVER_PORT);
            multiplayerApi.MultiplayerPeer = peer;
            multiplayerApi.PeerConnected += AddPlayer;
        }

        public void AddPlayer(long id)
        {
            var player = PackedScene.Instantiate();
            player.Name = id.ToString();
            CallDeferred("add_child", player);
        }
    }
}
