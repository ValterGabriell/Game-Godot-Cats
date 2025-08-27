using Godot;
using Godot.Collections;
using NovoProjetodeJogo.Scripts.Cenas.CardManager;
using NovoProjetodeJogo.Scripts.Geral;

public partial class CardManager : Node
{
    private Node2D WorkCurrentDraggedCard = null;
    private Vector2? WorkScreenSize = null;
    private CardStateEnum WorkCardState = CardStateEnum.IDLE;
    private bool IsHooveringCard = false;

    /*MAIN METHODS*/
    public override void _Ready()
    {
        WorkScreenSize = GetViewport().GetVisibleRect().Size;
    }

    public override void _Input(InputEvent @event)
    {
        if (IsLeftMouseButtonPressed(@event))
            UpdateDraggedCardReference();

        if (IsLeftMouseButtonReleased(@event))
            ClearDraggedCardReference();
    }
    /*MAIN METHODS*/


    // Adicione estes métodos para receber os sinais dos cards
    public void OnCardHoveredOn(Node2D currentCard)
    {
        if (!IsHooveringCard)
        {
            WorkCardState = CardStateEnum.HOVERED;
            HighlightCard(currentCard);
            IsHooveringCard = true;
        }

    }

    public void OnCardHoveredOff(Node2D currentCard)
    {
        WorkCardState = CardStateEnum.IDLE;
        NormalizeCard(currentCard);
        Node2D node2D = DetectCardAtMousePosition();
        if (node2D != null)
            HighlightCard(node2D);
        else
            IsHooveringCard = false;
    }



    public override void _Process(double delta)
    {
        if (WorkCurrentDraggedCard is not null)
        {
            GetViewportAndMousePosition(out Viewport _, out Vector2 globalMousePos);
            WorkCurrentDraggedCard.GlobalPosition = ClampMovementOfCard(globalMousePos);
        }
    }

    /*PRIVATE*/

    private Vector2 ClampMovementOfCard(Vector2 InGlobalMousePosition)
    {
        return new Vector2(
                Mathf.Clamp(InGlobalMousePosition.X, 0, WorkScreenSize.Value.X),
                Mathf.Clamp(InGlobalMousePosition.Y, 0, WorkScreenSize.Value.Y)
            );
    }

    private void HighlightCard(Node2D card)
    {
        // Lógica para destacar a carta (exemplo: aumentar escala, mudar cor, etc.)
        card.Scale = new Vector2(1.1f, 1.1f); // Exemplo: aumenta a escala da carta
        card.ZIndex = 1; // Traz a carta para frente
    }

    private void NormalizeCard(Node2D card)
    {
        // Lógica para normalizar a carta (exemplo: diminuir escala, mudar cor, etc.)
        card.Scale = new Vector2(1f, 1f); // Exemplo: retorna a escala da carta ao normal
        card.ZIndex = 0; // Retorna a carta para trás
    }


    private void UpdateDraggedCardReference()
    {
        Node2D WorkCard = DetectCardAtMousePosition();

        if (WorkCard != null) WorkCurrentDraggedCard = WorkCard;
        WorkCardState = CardStateEnum.DRAGGED;

        if (WorkCurrentDraggedCard == null)
            return;

        (CardSlot WorkCardSlot, _) = GetSlotAtMousePosition(ShouldSlotAppear: true);
        GD.Print(WorkCardSlot.CardOnSlot);
        GD.Print(WorkCurrentDraggedCard);
        if (WorkCardSlot == null || WorkCardSlot.IsOccupied && WorkCardSlot.CardOnSlot == WorkCurrentDraggedCard)
            WorkCardSlot.VacateSlot();
    }

    private void ClearDraggedCardReference()
    {
        if (WorkCurrentDraggedCard is null) return;
        (CardSlot WorkCardSlot, Vector2? WorkSlotPosition) = GetSlotAtMousePosition(ShouldSlotAppear: false);

        if (WorkCardSlot == null || WorkSlotPosition == null || WorkCardSlot.IsOccupied)
        {
            WorkCurrentDraggedCard = null;
            return;
        }


        GD.Print("Clear Dragged Card Reference");
        WorkCardSlot.OccupySlot(WorkCurrentDraggedCard);
        WorkCurrentDraggedCard.Position = WorkSlotPosition.Value;
        /*WorkCurrentDraggedCard.GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;*/ // Desativa a colisão da carta para evitar múltiplas detecções
        WorkCurrentDraggedCard = null;
        WorkCardState = CardStateEnum.IDLE;
        GD.Print("Dragged card cleared, slot occupied:", WorkCardSlot.IsOccupied);
    }

    private (CardSlot, Vector2?) GetSlotAtMousePosition(bool ShouldSlotAppear)
    {
        Node2D CurrentSlot = DetectSlotAtMousePosition();
        if (CurrentSlot == null) return (null, null);
        CurrentSlot.Visible = ShouldSlotAppear;  // Temporariamente torna o slot invisível para evitar detecção dupla
        return (CurrentSlot as CardSlot, CurrentSlot.Position);
    }

    private Node2D DetectSlotAtMousePosition()
    {
        Array<Dictionary> result = DetectNodeAtMousePoint(Constantes.COLLISION_MASK_SLOT);
        if (result.Count == 0) return null;

        Dictionary hitInfo = result[0];
        GodotObject collider = hitInfo["collider"].AsGodotObject();
        if (collider is Node2D colliderNode)
            return (Node2D)colliderNode.GetParent();
        return null;
    }

    private Node2D DetectCardAtMousePosition()
    {
        Array<Dictionary> result = DetectNodeAtMousePoint(Constantes.COLLISION_MASK_CARD);
        if (result.Count == 0) return null;  // Nenhum objeto encontrado
        return GetCardWithHighestZIndex(result);  // Retorna o nó pai do colisor
    }

    private Array<Dictionary> DetectNodeAtMousePoint(uint InCollisionMask)
    {
        /*
                 O Viewport no Godot é um nó fundamental que representa uma área de renderização na qual o conteúdo visual 
                do jogo é desenhado. 
                É essencialmente uma "janela" ou "tela" onde os objetos 2D e 3D são exibidos.
                 */
        GetViewportAndMousePosition(out Viewport viewport, out Vector2 globalMousePos);

        var parameters = new PhysicsPointQueryParameters2D
        {
            Position = globalMousePos,           // Posição global do mouse para verificar
            CollideWithAreas = true,            // Permite detectar áreas além de corpos físicos
            CollisionMask = InCollisionMask  // Máscara de colisão = 1 (apenas cartas)
        };

        var spaceState = viewport.World2D.DirectSpaceState;
        Array<Dictionary> result = spaceState.IntersectPoint(parameters);
        return result;
    }

    private static Node2D GetCardWithHighestZIndex(Array<Dictionary> cards)
    {
        Node2D HighestZIndexCard = null;
        int HighestZIndex = 0;

        Dictionary HitInfo = cards[0];      // Pega o primeiro objeto detectado
        GodotObject ColliderHighestZ = HitInfo["collider"].AsGodotObject();

        if (ColliderHighestZ is Node2D colliderNode)
        {
            HighestZIndexCard = (Node2D)colliderNode.GetParent();
            HighestZIndex = HighestZIndexCard.ZIndex;
        }

        foreach (var card in cards)
        {
            GodotObject currentCard = card["collider"].AsGodotObject();
            if (currentCard is Node2D currentCardNode)
            {
                if (currentCardNode.ZIndex > HighestZIndex)
                {
                    HighestZIndexCard = (Node2D)currentCardNode.GetParent();
                    HighestZIndex = HighestZIndexCard.ZIndex;
                }
            }
        }
        return HighestZIndexCard;
    }

    private void GetViewportAndMousePosition(out Viewport viewport, out Vector2 globalMousePos)
    {
        viewport = GetViewport();
        globalMousePos = viewport.GetMousePosition();
    }

    private static bool IsLeftMouseButtonPressed(InputEvent @event)
    {
        return @event is InputEventMouseButton mouseEvent
                    && mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed;
    }

    private static bool IsLeftMouseButtonReleased(InputEvent @event)
    {
        return @event is InputEventMouseButton mouseEvent
                    && !mouseEvent.Pressed;
    }


}
