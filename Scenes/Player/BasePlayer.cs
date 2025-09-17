using Godot;
using NovoProjetodeJogo.Scenes.Utils;
using System;

public enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Falling,
    Dead
}



public partial class BasePlayer : Node
{

    public EnumCharacter CurrentCharacter { get; set; } = EnumCharacter.Katrina;
    public PlayerState PlayerState { get; set; }
    public Vector2 LastPlayerPosition { get; set; }
    private Vector2 CurrentPlayerPosition { get; set; }
    public bool IsActivePlayer { get; set; } = false;

    public BasePlayer()
    {
    }



    public BasePlayer(PlayerState playerState, Vector2 lastPlayerPosition, EnumCharacter EnumCharacter)
    {
        PlayerState = playerState;
        LastPlayerPosition = lastPlayerPosition;
        IsActivePlayer = true;
        CurrentCharacter = EnumCharacter;
    }

        public BasePlayer(PlayerState playerState, Vector2 lastPlayerPosition, EnumCharacter EnumCharacter, bool isActivePlayer)
    {
        PlayerState = playerState;
        LastPlayerPosition = lastPlayerPosition;
        IsActivePlayer = isActivePlayer;
        CurrentCharacter = EnumCharacter;
    }

    public void NullPlayer()
    {
        PlayerState = PlayerState.Idle;
        LastPlayerPosition = Vector2.Zero;
        IsActivePlayer = false;
    }

    public void SetAsActivePlayer(bool active)
    {
        IsActivePlayer = active;
    }

    public void UpdateCurrentPlayerPosition(Vector2 position)
    {
        this.CurrentPlayerPosition = position;
    }

    public Vector2 GetCurrentPosition()
    {
        return this.CurrentPlayerPosition;
    }

    
}
