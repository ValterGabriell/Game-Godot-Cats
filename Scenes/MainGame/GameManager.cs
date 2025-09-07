// No seu GameManager, adicione estes métodos:
using Godot;

public partial class GameManager : Node
{
    private BasePlayer currentActivePlayer;
    private BasePlayer currentInactivePlayer;
    private static GameManager Instance;
    public int CurrentPhase { get; set; } = 1;

    public override void _Ready()
    {
        Instance ??= this;
    }


    
    public static GameManager GetInstance()
    {
        Instance ??= new GameManager();

        return Instance;
    }

    public void SetActivePlayer(BasePlayer player)
    {

        if (player.IsActivePlayer)
        {
            Logger.LogMessage("Jogador ativo definido: " + player.CurrentCharacter);
            currentActivePlayer = player;
        }
        else
        {
            Logger.LogMessage("Jogador inativo definido: " + player.CurrentCharacter);
            currentInactivePlayer = player;
        }
     

    }



    public (BasePlayer activePlayer, BasePlayer inactivePlayer) GetActiveAndInactivePlayer()
    {
        return (currentActivePlayer, currentInactivePlayer);
    }
    
    public void SavePlayerPosition(Vector2 position)
    {
        if (currentActivePlayer != null)
        {
            currentActivePlayer.LastPlayerPosition = position;
    
        }
    }
}