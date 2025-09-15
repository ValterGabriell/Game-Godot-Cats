using Godot;

public sealed class KatrinaAttackStrategy
{
    private IKatrinaAttack _currentAttack;

    public KatrinaAttackStrategy(IKatrinaAttack attack)
    {
        _currentAttack = attack;
    }

    public void ExecuteAttack(float delta, AnimatedSprite2D animatedsprite2D, Node2D player, Area2D collider)
    {
        _currentAttack!.Attack(delta, animatedsprite2D, player, collider);
    }
}