using Godot;

public class KatrinaCabecadaAttack : KatrinaAttackBase, IKatrinaAttack
{
    public void Attack(float delta, AnimatedSprite2D animatedsprite2D, Node2D player, Area2D headCollider)
    {
        ActiveAttackCollider(nomeCollider: "CollisionHead", area2DCollider: headCollider);
        AnimateAttack((float)delta, animatedsprite2D, player);
        return;
    }
}