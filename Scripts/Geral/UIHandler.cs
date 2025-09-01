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
        [Export]
        public NodePath SpawnPath { get; set; }

        public override void _Ready()
        {
            multiplayerApi = Multiplayer;
            textMoneyCurrentPlayerLabel = GetParent().GetNode<RichTextLabel>("UIManager/CountMoney");
            btnServer = GetParent().GetNode<Button>("UIManager/BtnServer");
            btnClient = GetParent().GetNode<Button>("UIManager/BtnConnect");

            textMoneyCurrentPlayerLabel.Text = "Money: " + currentPlayMoney.ToString();
            //btnServer.Pressed += OnBntServerPressed;
            //btnClient.Pressed += OnBntClientPressed;
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
            var ghLevelNetwork = new HighLevelNetwork();
            ghLevelNetwork.StartClient(multiplayerApi, PackedScene, SpawnPath);
        }

        public void OnBntServerPressed()
        {
            var ghLevelNetwork = new HighLevelNetwork();
            ghLevelNetwork.StartServer(multiplayerApi, PackedScene);
        }
    }
}
