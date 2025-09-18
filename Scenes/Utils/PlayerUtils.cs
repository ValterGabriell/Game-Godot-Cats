using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.TextServer;

namespace NovoProjetodeJogo.Scenes.Utils
{
    public static class PlayerUtils
    {
        public static bool IsNotActivePlayer(EnumCharacter activePlayerEnumChar, EnumCharacter currentEnumChar)
        {

            return currentEnumChar != activePlayerEnumChar;
        }

        public static void FlipSprite(float directionX, AnimatedSprite2D sprite, params RayCast2D[] rayCasts)
        {
            if (rayCasts != null)
            {
                foreach (var rayCast in rayCasts)
                {
                    if (rayCast != null)
                        rayCast.Scale = new Vector2(directionX < 0 ? -1 : 1, 1);
                }
            }
            if (sprite != null)
            {
                sprite.FlipH = directionX < 0;
            }
        }
    }
}
