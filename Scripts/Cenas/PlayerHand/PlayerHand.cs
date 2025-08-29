using Godot;
using System.Collections.Generic;

public partial class PlayerHand : Node
{
    private const int CurrentPlayerHand = 2;
    private const string CardPath = "res://Cenas/Card.tscn";
    private List<Node2D> PlayerCards = [];
    private float CenterScreenX;
    private const float CARD_WIDTH = 100;
    private const float HAND_Y_POSITION = 600;

    public override void _Ready()
    {
        CenterScreenX = GetViewport().GetVisibleRect().Size.X / 2;
        CardManager cardManager = GetParent().GetNode<CardManager>("CardManager");
        PackedScene cardScene = ResourceLoader.Load<PackedScene>(CardPath);
        for (int i = 0; i < CurrentPlayerHand; i++)
        {
            Node2D cardInstance = cardScene.Instantiate<Node2D>();
            cardManager.AddChild(cardInstance);
            cardInstance.Name = $"Card{i + 1}";
            AddCardToHand(cardInstance);
        }
    }

    public void AddCardToHand(Node2D InCard)
    {
        if (!IsCardInHand(InCard))
        {
            PlayerCards.Add(InCard);
            UpdateHandPosition();
        }

        if (IsCardInHand(InCard))
        {
            var card = (Card)InCard;
            AnimateCardToPosition(InCard, card.GetOriginalPosition());
        }

    }

    private bool IsCardInHand(Node2D InCard)
    {
        return PlayerCards.Contains(InCard);
    }

    private void UpdateHandPosition()
    {
        for (int i = 0; i < PlayerCards.Count; i++)
        {
            Vector2 newPosition =
                new(CalculateCardPositions(i), HAND_Y_POSITION);
            Card cardToMove = PlayerCards[i] as Card;
            cardToMove.UpdateOriginalPosition(newPosition);
            AnimateCardToPosition(cardToMove, newPosition);
        }
    }

    private void AnimateCardToPosition(Node2D cardToMove, Vector2 newPosition)
    {
        var tween = this.GetTree().CreateTween();
        tween.TweenProperty(cardToMove, "position", newPosition, 0.1f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
    }

    private float CalculateCardPositions(int InCardPosition)
    {
        float totalWidth = (PlayerCards.Count - 1) * CARD_WIDTH;
        float xOffset = CenterScreenX + InCardPosition * CARD_WIDTH - totalWidth / 2;
        return xOffset;
    }
}
