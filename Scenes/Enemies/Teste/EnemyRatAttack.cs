using Godot;
using NovoProjetodeJogo.Scenes.Player.PlayerCharacterBody;
using System;

public partial class EnemyRatAttack : Node
{
    [Export]
    public RayCast2D RaycastAttackEnemy { get; set; }

    private bool hasAttacked = false;

    public override void _PhysicsProcess(double delta)
    {
        if (RaycastAttackEnemy.IsColliding() && !hasAttacked)
        {
            if(RaycastAttackEnemy.GetCollider() is CharacterBody2D)
            {
                var player = RaycastAttackEnemy.GetCollider() as CharacterBody2D;
                var a = player as PlayerCharacterBody;
                a.ReceiveAttack(10);
                hasAttacked = true;
            }
        }
    }

    
}
