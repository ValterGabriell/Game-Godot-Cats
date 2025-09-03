using Godot;
using NovoProjetodeJogo.Cenas.MainGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NovoProjetodeJogo
{
    public partial class GameManager : Node
    {
        public static GameManager Instance { get; private set; }


        private List<PlayerInfo> orderPlayers = [];

        private PlayerInfo _currentTurnPlayer = null;

        public override void _Ready()
        {
            Instance = this;
        }

        public PlayerInfo GetCurrentTurn()
        {
            return _currentTurnPlayer;
        }

        public void NextTurn()
        {
            
            int index = orderPlayers.IndexOf(_currentTurnPlayer);
            if (index == -1 || index == orderPlayers.Count - 1)
                _currentTurnPlayer = orderPlayers[0];
            else
            {
                _currentTurnPlayer = orderPlayers[index + 1];
            }

            GD.Print($"Próximo Turno: {_currentTurnPlayer.PlayerName}");
        }

        public void AddPlayer(PlayerInfo playerInfo)
        {
            orderPlayers.Add(playerInfo);
        }


        public PlayerInfo GetCurrentPlayerById(long id)
        {
           return this.orderPlayers.FirstOrDefault(p => p.PlayerID == id);
        }
    }
}