using Godot;
using System;

public partial class DeathArea : Area2D
{

    public void OnBodyArea2DEntered(Node2D area)
    {
       if(area.IsInGroup(EnumGroups.Player.ToString()))
       {

            GameManager.GetInstance().GetActiveAndInactivePlayer().activePlayer.PlayerState = PlayerState.Dead;
       }
    }
}
