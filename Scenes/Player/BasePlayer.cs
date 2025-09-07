using Godot;
using System;

public enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Falling,
    Dead
}

public enum CharacterCurrent
{
    Katrina,
    Myha
}

public partial class BasePlayer : Node
{

    public CharacterCurrent CurrentCharacter { get; set; } = CharacterCurrent.Katrina;
    public PlayerState PlayerState { get; set; }
    public Vector2 LastPlayerPosition { get; set; }
    public bool IsActivePlayer { get; set; } = false;

    public BasePlayer()
    {
    }



    public BasePlayer(PlayerState playerState, Vector2 lastPlayerPosition, CharacterCurrent characterCurrent)
    {
        PlayerState = playerState;
        LastPlayerPosition = lastPlayerPosition;
        IsActivePlayer = true;
        CurrentCharacter = characterCurrent;
    }

        public BasePlayer(PlayerState playerState, Vector2 lastPlayerPosition, CharacterCurrent characterCurrent, bool isActivePlayer)
    {
        PlayerState = playerState;
        LastPlayerPosition = lastPlayerPosition;
        IsActivePlayer = isActivePlayer;
        CurrentCharacter = characterCurrent;
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
}
