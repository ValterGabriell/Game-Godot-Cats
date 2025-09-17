using Godot;
using NovoProjetodeJogo.Scenes.Utils;
using System;
using System.Linq;

public partial class PlayerConfig : Node2D
{
    [Export]
    public bool IsActivePlayer { get; set; } = false;

    [Export]
    public EnumCharacter EnumCharacter { get; set; }

    [Export]
    public CameraHandle CameraHandle { get; set; }

    public PlayerState PlayerState { get; set; } = PlayerState.Idle;
    public Vector2 LastPlayerPosition { get; set; }
    public Vector2 CurrentPlayerPosition { get; set; }
    public int Vida { get; set; } = 100;

    private CharacterBody2D characterBody2D;

    private GameManager GameManager;

    private bool IsKeyPressed { get; set; } = false;

    public override void _EnterTree()
    {
        CallDeferred(nameof(ConfigurePlayer));
    }

    public override void _Ready()
    {
        GameManager = GameManager.GetInstance();
    }

    private void ConfigurePlayer()
    {
        GameManager.SetActiveAndInactivePlayers();

    }
    public override void _Process(double delta)
    {
        // Apenas um player precisa detectar a tecla (por exemplo, sempre o primeiro da lista)
        var allPlayerConfigs = GetTree().GetNodesInGroup("PlayerConfigs");
        if (allPlayerConfigs.Count > 0 && allPlayerConfigs[0] == this)
        {
            if (Input.IsActionJustPressed("ui_focus_next"))
            {
                GameManager.SetActiveAndInactivePlayers(); // Troca os papéis               

            }
        }
    }


    public void UpdateCurrentPlayerPosition(Vector2 position)
    {
        this.CurrentPlayerPosition = position;
    }

    public Vector2 GetCurrentPosition()
    {
        return this.CurrentPlayerPosition;
    }
}