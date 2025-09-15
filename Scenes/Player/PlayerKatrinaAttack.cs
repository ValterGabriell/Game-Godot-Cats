using Godot;
using System;
using System.ComponentModel.DataAnnotations.Schema;

public partial class PlayerKatrinaAttack. : Node
{
    [Export]
    public AnimatedSprite2D sprite2D { get; set; }

    private CharacterBody2D player;

    [Export]
    public Area2D headCollider { get; set; }

    [Export]
    public Area2D raboCollider { get; set; }

    [Export]
    public Area2D tapaCollider { get; set; }

    private bool isAttacking = false;

    private KatrinaAttackStrategy headAttackStrategy;
    private KatrinaAttackStrategy raboAttackStrategy;
    private KatrinaAttackStrategy tapaAttackStrategy;


    public override void _Ready()
    {
        player = GetParent().GetParent<CharacterBody2D>();
        sprite2D.AnimationFinished += OnAnimationFinished;

        headAttackStrategy = new KatrinaAttackStrategy(new KatrinaCabecadaAttack());
        raboAttackStrategy = new KatrinaAttackStrategy(new KatrinaRabadaAttack());
        tapaAttackStrategy = new KatrinaAttackStrategy(new KatrinaTapaAttack());
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed(EnumInputs.KatrinaHeadAttack.ToString()) && !isAttacking)
        {
            headAttackStrategy.ExecuteAttack((float)delta, sprite2D, player, headCollider);
            return;
        }

        if (Input.IsActionJustPressed(EnumInputs.KatrinaRaboAttack.ToString()) && !isAttacking)
        {
            raboAttackStrategy.ExecuteAttack((float)delta, sprite2D, player, raboCollider);
            return;
        }

        if (Input.IsActionJustPressed(EnumInputs.KatrinaTapaAttack.ToString()) && !isAttacking)
        {
            tapaAttackStrategy.ExecuteAttack((float)delta, sprite2D, player, tapaCollider);
            return;
        }
    }

    /* Verifica colisão após a animação de ataque */
    public void _on_area_collision_head_area_entered(Area2D area)
    {
        Logger.LogMessage("Head collider hit!");
        ProcessAttackOnEnemy(area, damage: 10);
    }

    public void _on_area_collision_rabo_area_entered(Area2D area)
    {
        Logger.LogMessage("Rabo collider hit!");
        ProcessAttackOnEnemy(area, damage: 20);
    }

    public void _on_area_collision_tapa_area_entered(Area2D area)
    {
        Logger.LogMessage("Tapa collider hit!");
        ProcessAttackOnEnemy(area, damage: 5);
    }


    private static void ProcessAttackOnEnemy(Area2D area, float damage)
    {
        if (area.IsInGroup(EnumGroups.Enemy.ToString()))
        {
            Logger.LogMessage("Enemy hit!");
            // Chame o método ReceiveDamage no inimigo
            var enemy = area as EnemieTeste;
            enemy?.ReceiveDamage(damage);
        }
    }



    private void DeactiveCollider(string nomeCollider, Area2D area2DCollider)
    {
        CollisionShape2D collider = area2DCollider.GetNode<CollisionShape2D>(nomeCollider);
        collider.Disabled = true;
        area2DCollider.Monitoring = false;
    }

    private void AnimateAttack(float delta)
    {
        sprite2D.Play(EnumAnimationName.AttackHead.ToString());
        player.Position += new Vector2(500, 0) * delta;
    }



    private void OnAnimationFinished()
    {

        if (sprite2D.Animation == EnumAnimationName.AttackHead.ToString())
        {
            sprite2D.Play(EnumAnimationName.Idle.ToString());
            DeactiveCollider(nomeCollider: "CollisionHead", area2DCollider: headCollider);
            DeactiveCollider(nomeCollider: "CollisionRabo", area2DCollider: raboCollider);
            DeactiveCollider(nomeCollider: "CollisionTapa", area2DCollider: tapaCollider);
        }

    }

}
