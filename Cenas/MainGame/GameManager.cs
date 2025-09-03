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


        private ENetMultiplayerPeer _peer;
        private bool _isServer = false;
        private List<PlayerInfo> orderPlayers = [];

        private PlayerInfo _currentTurnPlayer = null;

        public override void _Ready()
        {
            Instance = this;
        }

        public void SetupServer(int port)
        {
            _peer = new ENetMultiplayerPeer();
            _peer.CreateServer(port);

            GetTree().GetMultiplayer().MultiplayerPeer = _peer;
            _isServer = true;

        }

        public PlayerInfo GetCurrentTurn()
        {
            return _currentTurnPlayer;
        }

        public void SetInitialPlayer(PlayerInfo playerInfo)
        {
            _currentTurnPlayer = playerInfo;
        }

        // ✅ NOVO: Método público para sincronizar o turno atual nos clientes
        public void SetCurrentTurnPlayer(PlayerInfo playerInfo)
        {
            _currentTurnPlayer = playerInfo;
        }


        public void NextTurn()
        {
            if (orderPlayers.Count == 0)
            {
                return;
            }

            if (_currentTurnPlayer == null)
            {
                return;
            }


            int index = orderPlayers.IndexOf(_currentTurnPlayer);

            if (index == -1)
            {
                _currentTurnPlayer = orderPlayers[0];
            }
            else if (index == orderPlayers.Count - 1)
            {
                _currentTurnPlayer = orderPlayers[0];
            }
            else
            {
                _currentTurnPlayer = orderPlayers[index + 1];
            }

        }

        public void AddPlayer(PlayerInfo playerInfo)
        {
            orderPlayers.Add(playerInfo);
        }

        // ✅ NOVO: Método para debug
        public void LogPlayerOrder()
        {
            for (int i = 0; i < orderPlayers.Count; i++)
            {
                var player = orderPlayers[i];
                string marker = player == _currentTurnPlayer ? " <- ATUAL" : "";
            }
        }

    }
}