using Godot;

public partial class Deck : Node2D
{
    private int NumberCardOnPlayerDeck = 0;

    public static void DrawCard(PlayerHand playerHand)
    {
        playerHand.GetCardFromDeck();
    }

    public static void OnPlayerHandFullOfCard(Node2D CurrentCard)
    {
        GD.Print("Mão cheia");
    }

}
