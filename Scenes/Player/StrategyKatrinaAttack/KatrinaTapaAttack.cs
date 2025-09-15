using Godot;

public class KatrinaTapaAttack : KatrinaAttackBase, IKatrinaAttack
{
    public void Attack(float delta, AnimatedSprite2D animatedsprite2D, Node2D player, Area2D tapaCollider)
    {
        ActiveAttackCollider(nomeCollider: "CollisionTapa", area2DCollider: tapaCollider);
        AnimateAttack((float)delta, animatedsprite2D, player);
        return;
    }
}