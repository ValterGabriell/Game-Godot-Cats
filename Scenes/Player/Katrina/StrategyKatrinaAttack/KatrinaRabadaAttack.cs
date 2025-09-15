using Godot;

public class KatrinaRabadaAttack : KatrinaAttackBase, IKatrinaAttack
{
    public void Attack(float delta, AnimatedSprite2D animatedsprite2D, Node2D player, Area2D raboCollider)
    {
        ActiveAttackCollider(nomeCollider: "CollisionRabo", area2DCollider: raboCollider);
        AnimateAttack((float)delta, animatedsprite2D, player);
        return;
    }
}