using Godot;
using System;

public partial class Phase01 : Node2D
{

    public override void _Ready()
    {
        GameManager.GetInstance().CurrentPhase = 1;

    }
}
