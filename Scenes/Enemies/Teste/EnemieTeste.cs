using Godot;
using System;

public partial class EnemieTeste : CharacterBody2D
{
    public void ReceiveDamage()
    {
        GD.Print("Enemy hit!");
    }
}
