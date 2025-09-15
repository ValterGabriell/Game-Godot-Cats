using Godot;
using System;

public partial class MyhaAttack : Node
{
    [Export]
    public AnimatedSprite2D sprite2D { get; set; }

    [Export]
    public Area2D derrubaAlgoCollider { get; set; }

    private bool isAttacking = false;

    public override void _Ready()
    {
        sprite2D.AnimationFinished += OnAnimationFinished;
        derrubaAlgoCollider.BodyEntered += OnAttackCollider;
    }

    private void OnAttackCollider(Node2D body)
    {
        Logger.LogMessage($"Myha hit {body.Name}!");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed(EnumInputs.FirstPlayerAttack.ToString()) && !isAttacking)
        {
            sprite2D.Play(EnumAnimationName.MyhaThrowsSomething.ToString());
            derrubaAlgoCollider.Monitoring = true;
            derrubaAlgoCollider.GetNode<CollisionShape2D>("DerrubaAlgoCollider").Disabled = false;
           
        }

        if (Input.IsActionJustPressed(EnumInputs.SecondPlayerAttack.ToString()) && !isAttacking)
        {
            Logger.LogMessage("Myha Attack!");
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
            derrubaAlgoCollider.Monitoring = false;
            derrubaAlgoCollider.GetNode<CollisionShape2D>("DerrubaAlgoCollider").Disabled = true;
        }

    }

}
