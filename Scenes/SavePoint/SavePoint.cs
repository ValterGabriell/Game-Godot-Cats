using Godot;
using System;

public partial class SavePoint : Node2D
{

    
    public void OnBodyArea2DEntered(Node2D area)
    {
        if(area.IsInGroup(EnumGroups.Player.ToString()))
        {
            GameManager.GetInstance().SavePlayerPosition(this.Position);
        }
    }
}
