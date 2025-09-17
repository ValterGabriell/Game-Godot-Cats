using Godot;
using System;

public partial class EnemieTeste : Node
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

    public void KillEnemy()
    {
        QueueFree();
    }
}
