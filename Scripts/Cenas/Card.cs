using Godot;

public partial class Card : Node2D
{
    [Signal]
    public delegate void CardHoveredOnSignalEventHandler(Node2D CurrentCard);

    [Signal]
    public delegate void CardHoveredOffSignalEventHandler(Node2D CurrentCard);

    public override void _Ready()
    {
        ConnectCardSignals();
    }



    public void OnArea2DMouseEntered()
    {
        EmitSignal(SignalName.CardHoveredOnSignal, this);
    }

    public void OnArea2DMouseExited()
    {
        EmitSignal(SignalName.CardHoveredOffSignal, this);
    }

    private void ConnectCardSignals()
    {
        // Conecta os sinais ao CardManager (que é o pai)
        var cardManager = GetParent<CardManager>();
        this.CardHoveredOnSignal += cardManager.OnCardHoveredOn;
        this.CardHoveredOffSignal += cardManager.OnCardHoveredOff;
    }
}
