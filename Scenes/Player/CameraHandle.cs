using Godot;
using System;

public partial class CameraHandle : Node2D
{
    public Camera2D PlayerCamera { get; set; }
    public BasePlayer ActivePlayer { get; set; } = null;
    


    public override void _PhysicsProcess(double delta)
    {
        // ATUALIZAR POSIÇÃO DA CÂMERA MANUALMENTE
        if (PlayerCamera != null && PlayerCamera.Enabled)
        {
            PlayerCamera.GlobalPosition = this.GlobalPosition;
        }
    }
    
}
