using Godot;

public interface IKatrinaAttack
{
    public void Attack(float delta, AnimatedSprite2D animatedsprite2D, Node2D player, Area2D headCollider);
}