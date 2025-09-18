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
    public AnimatedSprite2D AnimatedSprite2D { get; set; }

    [Export]
    public Area2D ThrowCollider { get; set; }

    private const EnumCharacter currentCharacter = EnumCharacter.Myha;

    private bool isAttacking = false;

    private float forceAttackThrow = 0;
    private EnumForceDirection forceDirection = EnumForceDirection.Increase;
    private const float MAX_FORCE_ATTACK_THROW = 100;

    private GameManager GameInstance;

    public override void _Ready()
    {
        AnimatedSprite2D.AnimationFinished += OnAnimationFinished;
        ThrowCollider.BodyEntered += OnAttackCollider;
        GameInstance = GameManager.GetInstance();
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
        var (activePlayer, _) = GameInstance.GetActiveAndInactivePlayer();

        if (PlayerUtils.IsNotActivePlayer(activePlayer.EnumCharacter, currentCharacter))
            return;

        if (Input.IsActionJustPressed(EnumInputs.FirstPlayerAttack.ToString()) && !isAttacking)
        {
            AnimatedSprite2D.Play(EnumAnimationName.MyhaThrowsSomething.ToString());
            ThrowCollider.Monitoring = true;
            ThrowCollider.GetNode<CollisionShape2D>("throwableItemCollider").Disabled = false;
        }

        if (Input.IsActionPressed(EnumInputs.SecondPlayerAttack.ToString()) && !isAttacking)
        {
            AnimatedSprite2D.Play(EnumAnimationName.MyhaThrowsSomething.ToString());
            ThrowCollider.Monitoring = true;
            ThrowCollider.GetNode<CollisionShape2D>("throwableItemCollider").Disabled = false;

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

    private static bool IsNotCurrentCharacter(PlayerConfig activePlayer)
    {
        return activePlayer.EnumCharacter != currentCharacter;
    }

    private void OnAnimationFinished()
    {

        if (AnimatedSprite2D.Animation == EnumAnimationName.KatrinaAttackHead.ToString() || AnimatedSprite2D.Animation == EnumAnimationName.MyhaThrowsSomething.ToString())
        {
            AnimatedSprite2D.Play(EnumAnimationName.Idle.ToString());
            ThrowCollider.Monitoring = false;
            ThrowCollider.GetNode<CollisionShape2D>("throwableItemCollider").Disabled = true;
        }

    }

}
