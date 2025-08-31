using Godot;

public partial class Deck : Node2D
{
	private int NumberCardOnPlayerDeck = 0;

	public static void DrawCard(PlayerHand playerHand)
	{
		GD.Print("Comprar carta");
		GD.Print(playerHand);
		playerHand.GetCardFromDeck();
	}

	public void OnPlayerHandFullOfCard(PlayerHand CurrentCard)
	{
		GD.Print("Mão cheia");
	}

}
