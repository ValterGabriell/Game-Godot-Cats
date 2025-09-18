using Godot;
using System;


public partial class CameraHandle : Node2D
{
    [Export]
    private Camera2D CurrentCamera { get; set; }
    private GameManager GameManager => GameManager.GetInstance();

    public override void _PhysicsProcess(double delta)
    {
        var activePlayer = GameManager?.GetActiveAndInactivePlayer().activePlayer;
        if (activePlayer != null)
        {
            this.GlobalPosition = activePlayer.GetCurrentPosition();
        }

        if (CurrentCamera != null && CurrentCamera.Enabled)
        {
            // Centraliza a câmera na posição do jogador
            CurrentCamera.GlobalPosition = this.GlobalPosition;
            CurrentCamera.Offset = Vector2.Zero; // Garante que não há deslocamento
        }
    }
}
