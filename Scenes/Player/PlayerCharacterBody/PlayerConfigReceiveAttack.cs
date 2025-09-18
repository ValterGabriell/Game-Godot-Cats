using Godot;

namespace NovoProjetodeJogo.Scenes.Player.PlayerCharacterBody;

public partial class PlayerCharacterBody : CharacterBody2D
{
    public void ReceiveAttack(int damage)
    {
        this.PlayerConfig.Vida -= damage;
        Logger.LogMessage($"Player {this.PlayerConfig.EnumCharacter} recebeu {damage} de dano. Vida atual: {this.PlayerConfig.Vida}", color: "red", bold: true);
        if (this.PlayerConfig.Vida <= 0)
        {
            this.PlayerConfig.Vida = 0;
     
        }
    }
}