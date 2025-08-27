using Godot;

public partial class CardSlot : Node2D
{
    public bool IsOccupied { get; private set; } = false;
    public Node2D CardOnSlot { get; private set; } = null;

    public void OccupySlot(Node2D card)
    {
        GD.Print("Slot occupied");
        CardOnSlot = card;
        IsOccupied = true;
    }

    public void VacateSlot()
    {
        GD.Print("Slot vacated");
        IsOccupied = false;
        CardOnSlot = null;
    }
}
