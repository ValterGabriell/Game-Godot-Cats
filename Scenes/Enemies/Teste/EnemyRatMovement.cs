using Godot;
using System;
using System.ComponentModel;
using System.Linq;

public partial class EnemyRatMovement : CharacterBody2D
{
    private enum State
    {
        Idle,
        Chasing,
        Attacking,
        Returning
    }


    [Export]
    public int SPEED { get; set; } = 500;

    [Export]
    private NavigationAgent2D Agent { get; set; }

    [Export]
    private RayCast2D RayCast { get; set; }

    [Export]
    private Timer Timer { get; set; }

    private GameManager GameManagerInstance { get; set; }

    private State CurrentState { get; set; } = State.Idle;

    [Export]
    private Marker2D[] PositionsToPatrol { get; set; } = [];

    private Marker2D CurrentPatrolPosition { get; set; }

    public override void _Ready()
    {
        GameManagerInstance = GameManager.GetInstance();
        if (PositionsToPatrol.Length > 0)
            CurrentPatrolPosition = PositionsToPatrol[0];
    }

    public override void _PhysicsProcess(double delta)
    {
        if (this.GlobalPosition.DistanceTo(CurrentPatrolPosition.Position) < 10)
            GetNextPosition();

        if(CurrentState == State.Idle || CurrentState == State.Returning)
        {
            Vector2 direction = (CurrentPatrolPosition.Position - this.GlobalPosition);
            Velocity = direction * SPEED * (float) delta;
        }


        if (RayCast.IsColliding() && RayCast.GetCollider() is BasePlayer player && this.CurrentState != State.Chasing)
            ChasePlayer();
        else if (!RayCast.IsColliding() && this.CurrentState == State.Chasing)
            StopChase();

        MoveAndSlide();
    }

    private void GetNextPosition()
    {
        CurrentPatrolPosition = PositionsToPatrol[(Array.IndexOf(PositionsToPatrol, CurrentPatrolPosition) + 1) % PositionsToPatrol.Length];
    }

    private void StopChase()
    {
        this.CurrentState = State.Returning;
    }

    private void ChasePlayer()
    {
        this.CurrentState = State.Chasing;
        SPEED = 250;
    }

  
    public void OnNavTimer_timeout()
    {
        
    }
}
