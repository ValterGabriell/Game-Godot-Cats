using Godot;
using System;

public partial class Player : Node2D
{
    public override void _EnterTree()
    {

        int id = int.Parse(this.Name);

        SetMultiplayerAuthority(id);

    }
}
