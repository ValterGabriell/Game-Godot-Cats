using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;

public partial class ThrowableItemAttack : RigidBody2D
{
    public void _on_make_damage_enemy_area_body_entered(Node2D body)
    {
        Logger.LogMessage("Throwable item hit something!");
        if (body.IsInGroup(EnumGroups.Enemy.ToString()))
        {
            Logger.LogMessage("Enemy hit!");
            // Chame o método ReceiveDamage no inimigo
            var enemy = body.GetNode("ReceiveDagame") as EnemieTeste;
            enemy?.ReceiveDamage(30);
        }
    }
}
