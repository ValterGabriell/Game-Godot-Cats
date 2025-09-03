using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NovoProjetodeJogo.Cenas.MainGame
{
    public class PlayerInfo
    {
        public PlayerInfo()
        {
        }

        public PlayerInfo(long playerID, string playerName, int playerOrder)
        {
            PlayerID = playerID;
            PlayerName = playerName;
            PlayerOrder = playerOrder;
        }
        public long PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int PlayerOrder { get; set; }
    }
}
