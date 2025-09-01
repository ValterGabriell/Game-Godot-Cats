using Godot;
using System;

public partial class Player : Node2D
{


    public override void _EnterTree()
    {
        GD.Print(this.Name);
        int id = int.Parse(this.Name);
        GD.Print(id);
        SetMultiplayerAuthority(id);
        GD.Print("Player entered the scene tree.");
    }
}
