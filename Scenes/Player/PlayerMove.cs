using Godot;
using System;

public partial class PlayerMove : CharacterBody2D
{
    [Export]
    public float Speed = 650.0f;

    [Export]
    public float JumpVelocity = -950.0f;

    [Export]
    public float Gravity = 2700;

    [Export]
    public AnimatedSprite2D sprite2D { get; set; }


    public override void _Process(double delta)
    {
        var instance = GameManager.GetInstance();

        var (activePlayer, _) = instance.GetActiveAndInactivePlayer();
        if (activePlayer == null) return;

        if (activePlayer.PlayerState == PlayerState.Dead)
        {
            activePlayer.PlayerState = PlayerState.Idle; // Reset state
            this.Position = activePlayer.LastPlayerPosition;
        }

      
    }
    

 public override void _PhysicsProcess(double delta)
{
    if (sprite2D != null && sprite2D.Animation == EnumAnimationName.KatrinaAttackHead.ToString())
    {
        return; // Skip movement processing during attack animation
    }
    Vector2 velocity = Velocity;

    var instance = GameManager.GetInstance();
    var (activePlayer, _) = instance.GetActiveAndInactivePlayer();

    var cameraHandle = GetNode<CameraHandle>("CameraHandle");
    bool isThisPlayerActive = cameraHandle != null && cameraHandle.ActivePlayer == activePlayer;
    if(activePlayer != null)
    {
        activePlayer.UpdateCurrentPlayerPosition(this.Position);
    }
   


    // Se não for o player ativo, apenas aplicar gravidade e retornar
    if (!isThisPlayerActive && !IsOnFloor())
    {
        velocity.Y += Gravity * (float)delta;
        Velocity = velocity;
        MoveAndSlide();
        return;
    }

    // Aplicar gravidade
    if (!IsOnFloor())
    {
        velocity.Y += Gravity * (float)delta;
    }

    // Controle de pulo (apenas para player ativo)
    if (Input.IsActionJustPressed("ui_accept") || Input.IsKeyPressed(Key.Space))
    {
        if (IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }
    }

    // Movimento horizontal (apenas para player ativo)
    Vector2 direction = Vector2.Zero;

    if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A))
        direction.X -= 1;
    if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D))
        direction.X += 1;

    // Aplicar movimento horizontal
    if (direction != Vector2.Zero)
    {
        velocity.X = direction.X * Speed;
    }
    else
    {
        velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
    }

    Velocity = velocity;
    MoveAndSlide();
}
    
     // Métodos para ativar/desativar este jogador
 
}
