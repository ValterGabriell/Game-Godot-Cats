using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed = 300.0f;

    [Export]
    public Texture2D PlayerTexture = GD.Load<Texture2D>("res://icon.svg");

    private Sprite2D _currentPlayerSprite;

    public override void _Ready()
    {
        _currentPlayerSprite = GetNode<Sprite2D>("Sprite2D");
        _currentPlayerSprite.Texture = PlayerTexture;
                // Definir tamanho fixo de 128x128 pixels
        if (_currentPlayerSprite.Texture != null)
        {
            Vector2 textureSize = _currentPlayerSprite.Texture.GetSize();
            Vector2 targetSize = new Vector2(128, 128);
            Vector2 scale = new Vector2(targetSize.X / textureSize.X, targetSize.Y / textureSize.Y);
            _currentPlayerSprite.Scale = scale;
        }
    }
	
	public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Obter direção do input usando teclas diretas
        Vector2 direction = Vector2.Zero;

        if (Input.IsActionPressed("ui_left") || Input.IsKeyPressed(Key.A))
            direction.X -= 1;
        if (Input.IsActionPressed("ui_right") || Input.IsKeyPressed(Key.D))
            direction.X += 1;
        if (Input.IsActionPressed("ui_up") || Input.IsKeyPressed(Key.W))
            direction.Y -= 1;
        if (Input.IsActionPressed("ui_down") || Input.IsKeyPressed(Key.S))
            direction.Y += 1;

        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Y = direction.Y * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
