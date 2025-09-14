using Godot;
using System;

public partial class PlayerAttack : Node
{
    [Export]
    public AnimatedSprite2D sprite2D { get; set; }

    private CharacterBody2D player;

    [Export]
    public Area2D headCollider { get; set; }
    
    public override void _Ready()
    {
        player = GetParent().GetParent<CharacterBody2D>();
        sprite2D.AnimationFinished += OnAnimationFinished;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed(EnumInputs.Attack.ToString()))
        {
            ActiveHeadCollider();
            AnimateAttack((float)delta);
            VerifyCollision();
        }
           
    }
    
    private void DeactiveHeadCollider()
    {
        CollisionShape2D headCollisionShape = headCollider.GetNode<CollisionShape2D>("CollisionHead");
        headCollisionShape.Disabled = true;
        headCollider.Monitoring = false;
    }

    private void AnimateAttack(float delta)
    {
        sprite2D.Play(EnumAnimationName.AttackHead.ToString());
        player.Position += new Vector2(500, 0) *delta; 
    }

    private void VerifyCollision()
    {
        Logger.LogMessage("Verifying collision...");
        var bodies = headCollider.GetOverlappingBodies();
        Logger.LogMessage(bodies.Count.ToString());
        foreach (var body in bodies)
        {
            Logger.LogMessage(body.ToString());
            if (body is EnemieTeste enemie)
            {
                Logger.LogMessage("Hit enemy!");
                enemie.ReceiveDamage();
            }
            else
            {
                Logger.LogMessage("No enemy hit.");
            }
        }
    }

    private void ActiveHeadCollider()
    {
        CollisionShape2D headCollisionShape = headCollider.GetNode<CollisionShape2D>("CollisionHead");
        headCollisionShape.Disabled = false;
        headCollider.Monitoring = true;
    }
    
    private void OnAnimationFinished()
    {
        // Só volta para Idle se a animação atual for AttackHead
        if (sprite2D.Animation == EnumAnimationName.AttackHead.ToString())
        {
            sprite2D.Play(EnumAnimationName.Idle.ToString());
            DeactiveHeadCollider();
        }
            
    }

}
