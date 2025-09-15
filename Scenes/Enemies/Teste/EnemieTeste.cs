using Godot;
using System;

public partial class EnemieTeste : Area2D
{
    private int health = 40;
    public void ReceiveDamage(float damage)
    {
        health -= (int)damage;
        Logger.LogMessage($"Enemy hit and took {damage} damage!");
        if (health <= 0)
        {
            Logger.LogMessage("Enemy defeated!");
            QueueFree();
        }
    }
}
