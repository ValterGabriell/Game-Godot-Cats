using Godot;
using NovoProjetodeJogo.Scenes.Utils;
using System;

namespace NovoProjetodeJogo.Scenes.Enemies.Teste;
public partial class EnemyRatMovement : CharacterBody2D
{
   
    public override void _Ready()
    {
        if (PositionsToPatrol.Length > 0)
            CurrentPatrolPositionMarker = PositionsToPatrol[0];

        GameInstance = GameManager.GetInstance();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsPatrollingOrReturning())
        {
            if (IsNearPatrolPoint())
                GetNextPosition();

            Vector2 direction = (CurrentPatrolPositionMarker.Position - GlobalPosition).Normalized();
            direction = RestrictDirectionToHorizontal(direction);

            Velocity = direction * PatrolSpeed;

            PlayerUtils.FlipSprite(direction.X, this.AnimatedSprite2D, this.RayCast, this.AttackRayCastToFlip);
        }

        if (RayCast.IsColliding())
            StartChasing();


        if (CurrentState == State.Chasing)
            UpdateChaseMovement((float) delta);


        MoveAndSlide();
    }

    private bool IsNearPatrolPoint()
    {
        return GlobalPosition.DistanceTo(CurrentPatrolPositionMarker.Position) < 75f;
    }

    private bool IsPatrollingOrReturning()
    {
        return CurrentState == State.Patrolling || CurrentState == State.Returning;
    }

    private void UpdateChaseMovement(float delta)
    {
        Vector2 playerPos = GameInstance.GetCurrentActivePlayerPosistion();
        Vector2 direction = (playerPos - GlobalPosition).Normalized(); // Normaliza para manter velocidade constante
        direction = RestrictDirectionToHorizontal(direction);
        Velocity = direction * ChaseSpeed; // Não multiplica por delta
    }

    private static Vector2 RestrictDirectionToHorizontal(Vector2 direction)
    {
        direction.Y = 0; // Impede movimento no eixo Y
        return direction;
    }

    private void StartChasing()
    {
        CurrentState = State.Chasing;
        Timer.Start();
    }

    private void GetNextPosition()
    {
        int idx = Array.IndexOf(PositionsToPatrol, CurrentPatrolPositionMarker);
        idx = (idx + 1) % PositionsToPatrol.Length;
        CurrentPatrolPositionMarker = PositionsToPatrol[idx];
    }

    public void OnNavTimer_timeout()
    {
        CurrentState = State.Returning;
    }
}