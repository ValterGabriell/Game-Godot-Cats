using Godot;

public class KatrinaAttackBase
{
    public virtual void ActiveAttackCollider(string nomeCollider, Area2D area2DCollider)
    {
        CollisionShape2D collider = area2DCollider.GetNode<CollisionShape2D>(nomeCollider);
        collider.Disabled = false;
        area2DCollider.Monitoring = true;
    }

    public void AnimateAttack(float delta, AnimatedSprite2D animatedsprite2D, Node2D player)
    {
        animatedsprite2D.Play(EnumAnimationName.KatrinaAttackHead.ToString());
        player.Position += new Vector2(500, 0) * delta;
    }

}