using Godot;
using NovoProjetodeJogo.Scenes.Utils;
using System;

enum EnumForceDirection
{
    Increase = 1,
    Decrease = -1
}

public partial class MyhaAttack : Node
{
    [Export]
    public AnimatedSprite2D sprite2D { get; set; }

    [Export]
    public Area2D throwCollider { get; set; }

    private bool isAttacking = false;

    private float forceAttackThrow = 0;
    private EnumForceDirection forceDirection = EnumForceDirection.Increase; 
    private const float MAX_FORCE_ATTACK_THROW = 100;

    public override void _Ready()
    {
        sprite2D.AnimationFinished += OnAnimationFinished;
        throwCollider.BodyEntered += OnAttackCollider;
    }

    private void OnAttackCollider(Node2D body)
    {
        if (body.IsInGroup(EnumGroups.ItensThrowable.ToString()))
        {
            var rigidBodyItemThrowable = body as RigidBody2D;
            rigidBodyItemThrowable.CollisionMask &= ~MascarasBits.Layer1;
        }
    }


    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed(EnumInputs.FirstPlayerAttack.ToString()) && !isAttacking)
        {
            sprite2D.Play(EnumAnimationName.MyhaThrowsSomething.ToString());
            throwCollider.Monitoring = true;
            throwCollider.GetNode<CollisionShape2D>("throwableItemCollider").Disabled = false;
           
        }

        if (Input.IsActionPressed(EnumInputs.SecondPlayerAttack.ToString()) && !isAttacking)
        {
            sprite2D.Play(EnumAnimationName.MyhaThrowsSomething.ToString());
            throwCollider.Monitoring = true;
            throwCollider.GetNode<CollisionShape2D>("throwableItemCollider").Disabled = false;

            forceAttackThrow += (int)forceDirection * (float)(100 * delta);
            forceAttackThrow = Mathf.Clamp(forceAttackThrow, 0, MAX_FORCE_ATTACK_THROW);

            if (forceAttackThrow >= MAX_FORCE_ATTACK_THROW)
                forceDirection = EnumForceDirection.Decrease;
            if (forceAttackThrow <= 0)
                forceDirection = EnumForceDirection.Increase;

            Logger.LogMessage($"Myha Attack! {forceAttackThrow}");
        }

        if (Input.IsActionJustPressed(EnumInputs.ThirdPlayerAttack.ToString()) && !isAttacking)
        {
            Logger.LogMessage("Myha Attack!");
        }
    }


    private void OnAnimationFinished()
    {

        if (sprite2D.Animation == EnumAnimationName.KatrinaAttackHead.ToString() || sprite2D.Animation == EnumAnimationName.MyhaThrowsSomething.ToString())
        {
            sprite2D.Play(EnumAnimationName.Idle.ToString());
            throwCollider.Monitoring = false;
            throwCollider.GetNode<CollisionShape2D>("throwableItemCollider").Disabled = true;
        }

    }

}
