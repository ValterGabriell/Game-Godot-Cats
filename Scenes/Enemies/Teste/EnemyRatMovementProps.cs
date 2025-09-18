using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovoProjetodeJogo.Scenes.Enemies.Teste
{
    public partial class EnemyRatMovement
    {
        private enum State
        {
            Idle,
            Patrolling,
            Chasing,
            Returning
        }

        [Export]
        public int PatrolSpeed { get; set; } = 120;

        [Export]
        public int ChaseSpeed { get; set; } = 250;

        [Export]
        private NavigationAgent2D Agent { get; set; }

        [Export]
        private RayCast2D RayCast { get; set; }

        [Export]
        private RayCast2D AttackRayCastToFlip { get; set; }

        [Export]
        private Timer Timer { get; set; }

        [Export]
        private Marker2D[] PositionsToPatrol { get; set; } = [];

        [Export]
        private AnimatedSprite2D AnimatedSprite2D { get; set; }


        private State CurrentState { get; set; } = State.Patrolling;
        private Marker2D CurrentPatrolPositionMarker { get; set; }
        private GameManager GameInstance;

    }
}
