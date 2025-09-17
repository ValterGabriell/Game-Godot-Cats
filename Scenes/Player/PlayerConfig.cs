using Godot;
using NovoProjetodeJogo.Scenes.Utils;
using System;

public partial class PlayerConfig : Node2D
{
    [Export]
    public bool IsActivePlayer { get; set; } = false;

    [Export]
    public EnumCharacter EnumCharacter { get; set; }

    [Export]
    public Camera2D PlayerCamera { get; set; }

    private CharacterBody2D characterBody2D;


    private bool IsKeyPressed { get; set; } = false;

    public override void _EnterTree()
    {
        CallDeferred(nameof(ConfigurePlayer));
    }

    private void ConfigurePlayer()
    {       
             var instance = GameManager.GetInstance();
             var player = new BasePlayer(PlayerState.Idle, this.Position, EnumCharacter, isActivePlayer: IsActivePlayer);
              instance.SetActivePlayer(player);


            var nodeCamera = GetNode<Node2D>("CharacterBody2D/CameraHandle");
            CameraHandle cameraHandle = nodeCamera as CameraHandle;

            if (cameraHandle != null)
            {
                if (IsActivePlayer)
                {
                    cameraHandle.ActivePlayer = player;
                }
                cameraHandle.PlayerCamera = PlayerCamera;
     
                // Configurar câmera baseado no estado ativo
            if (PlayerCamera != null)
            {
                PlayerCamera.Enabled = IsActivePlayer;
            }
            }
    }
public override void _Process(double delta)
{
    // Apenas um player precisa detectar a tecla (por exemplo, sempre o primeiro da lista)
    var allPlayerConfigs = GetTree().GetNodesInGroup("PlayerConfigs");
    if (allPlayerConfigs.Count > 0 && allPlayerConfigs[0] == this)
    {
        if (Input.IsActionJustPressed("ui_focus_next"))
        {
            SwitchActivePlayer();
        }
    }
}

private void SwitchActivePlayer()
{
    var allPlayerConfigs = GetTree().GetNodesInGroup("PlayerConfigs");
    PlayerConfig currentActivePlayer = null;
    PlayerConfig currentInactivePlayer = null;
    
    foreach (PlayerConfig player in allPlayerConfigs)
    {
        if (player.IsActivePlayer)
            currentActivePlayer = player;
        else
            currentInactivePlayer = player;
    }
    
    if (currentActivePlayer != null && currentInactivePlayer != null)
    {
        currentActivePlayer.IsActivePlayer = false;
        currentInactivePlayer.IsActivePlayer = true;
        
        currentActivePlayer.ConfigurePlayer();
        currentInactivePlayer.ConfigurePlayer();
    }
}
}