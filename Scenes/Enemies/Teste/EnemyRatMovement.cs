using Godot;
using System;
using System.ComponentModel;
using System.Linq;

public partial class EnemyRatMovement : CharacterBody2D
{
    [Export]
    public int SPEED { get; set; } = 100;

    [Export]
    private NavigationAgent2D Agent { get; set; }

    private GameManager GameManagerInstance { get; set; }

    public override void _Ready()
    {
        GameManagerInstance = GameManager.GetInstance();
        GoToTarget();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Agent.IsNavigationFinished())
        {
            Velocity = Vector2.Zero;
            return;
        }
        GD.Print($"Agent path point count: {Agent.GetCurrentNavigationPath().Count()}");

        Vector2 nextPoint = Agent.GetNextPathPosition().Normalized();
        Velocity = nextPoint * (float)delta * SPEED;
        MoveAndSlide();
    }

    private void GoToTarget()
    {
        var (activePlayer, _) = GameManagerInstance.GetActiveAndInactivePlayer();
        if (activePlayer != null)
        {
            var targetPos = activePlayer.GetCurrentPosition();
            Agent.SetTargetPosition(targetPos);
        }
    }

    public void OnNavTimer_timeout()
    {
        GoToTarget();
    }
}
