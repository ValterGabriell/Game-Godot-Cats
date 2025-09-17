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
    public AnimatedSprite2D AnimatedSprite2D { get; set; }

    [Export]
    public PlayerConfig PlayerConfig { get; set; }

    private GameManager GameInstance;

    public override void _Ready()
    {
        GameInstance = GameManager.GetInstance();
    }

    public override void _PhysicsProcess(double delta)
    {

        var (activePlayer, _) = GameInstance.GetActiveAndInactivePlayer();

        if (activePlayer == null) return;

        if (IsNotActivePlayer(activePlayer))
            return;

        if (IsPerformingAttack())
            return; // Skip movement processing during attack animation

        if (IsActivePlayerDead(activePlayer))
            ResetPlayerOnDeath(activePlayer);


        Vector2 velocity = Velocity;

        activePlayer?.UpdateCurrentPlayerPosition(this.Position);

        HandleMovement(delta, velocity);
    }

    private bool IsNotActivePlayer(PlayerConfig activePlayer)
    {
       
        return PlayerConfig.EnumCharacter != activePlayer.EnumCharacter;
    }

    private void HandleMovement(double delta, Vector2 velocity)
    {
        // Aplicar gravidade
        if (!IsOnFloor())
            velocity.Y += Gravity * (float)delta;


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

    private void ResetPlayerOnDeath(PlayerConfig activePlayer)
    {
        activePlayer.PlayerState = PlayerState.Idle; // Reset state
        this.Position = activePlayer.LastPlayerPosition;
    }

    private static bool IsActivePlayerDead(PlayerConfig activePlayer)
    {
        return activePlayer.PlayerState == PlayerState.Dead;
    }

    private bool IsPerformingAttack()
    {
        return AnimatedSprite2D != null && AnimatedSprite2D.Animation == EnumAnimationName.KatrinaAttackHead.ToString();
    }



}
