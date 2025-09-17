// No seu GameManager, adicione estes métodos:
using Godot;
using System.Linq;

public partial class GameManager : Node
{
    private PlayerConfig currentActivePlayer;
    private PlayerConfig currentInactivePlayer;
    private static GameManager Instance;
    public int CurrentPhase { get; set; } = 1;
    [Signal]
    public delegate void ActivePlayerSwitchedEventHandler(PlayerConfig newActivePlayer);

    public override void _Ready()
    {
        Instance ??= this;
    }



    public static GameManager GetInstance()
    {
        return Instance;
    }

    public void SetActiveAndInactivePlayers()
    {
        var allPlayerConfigs = GetTree().GetNodesInGroup("PlayerConfigs").OfType<PlayerConfig>().ToList();
        if (allPlayerConfigs.Count != 2) return;
        var active = allPlayerConfigs.FirstOrDefault(p => p.IsActivePlayer);
        var inactive = allPlayerConfigs.FirstOrDefault(p => !p.IsActivePlayer);

        if (active != null && inactive != null)
        {
            active.IsActivePlayer = false;
            inactive.IsActivePlayer = true;
            currentActivePlayer = inactive;
            currentInactivePlayer = active;
        }
    }



    public (PlayerConfig activePlayer, PlayerConfig inactivePlayer) GetActiveAndInactivePlayer()
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