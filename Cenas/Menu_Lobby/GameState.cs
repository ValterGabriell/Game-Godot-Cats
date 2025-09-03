using Godot;
using NovoProjetodeJogo.Cenas.MainGame;
using System.Collections.Generic;

namespace NovoProjetodeJogo
{
    public partial class GameState : Node
    {
        public static GameState Instance { get; private set; }

        // Estado do jogo
        public bool IsGameStarted { get; set; } = false;

        // Lista dos jogadores ORIGINAIS que começaram a partida
        public HashSet<long> OriginalPlayers { get; private set; } = new HashSet<long>();

        // Lista dos jogadores atualmente conectados
        public Dictionary<long, string> ConnectedPlayers { get; private set; } = new Dictionary<long, string>();

        [Signal]
        public delegate void GameStateChangedEventHandler(bool isStarted);

        [Signal]
        public delegate void PlayerReconnectedEventHandler(long playerId, string playerName);

        public override void _Ready()
        {
            Instance = this;
        }

        public void StartGame(Dictionary<long, string> playersAtStart)
        {
            IsGameStarted = true;
            OriginalPlayers.Clear();
            ConnectedPlayers.Clear();

            // Salva quem são os jogadores originais
            int index = 0;
            foreach (var player in playersAtStart)
            {
                OriginalPlayers.Add(player.Key);
                ConnectedPlayers[player.Key] = player.Value;

                GameManager.Instance.AddPlayer(new PlayerInfo(player.Key, player.Value,index ));
                index++;
            }

            EmitSignal(SignalName.GameStateChanged, IsGameStarted);
            
        }

        public bool IsOriginalPlayer(long playerId)
        {
            return OriginalPlayers.Contains(playerId);
        }

        public bool CanPlayerConnect(long playerId)
        {
            if (!IsGameStarted)
            {
                // Se o jogo não começou, qualquer um pode conectar
                return true;
            }

            // Se o jogo já começou, só jogadores originais podem reconectar
            return IsOriginalPlayer(playerId);
        }

        public void PlayerConnected(long playerId, string playerName)
        {
            ConnectedPlayers[playerId] = playerName;

            if (IsGameStarted && IsOriginalPlayer(playerId))
            {
                EmitSignal(SignalName.PlayerReconnected, playerId, playerName);
                
            }
        }

        public void PlayerDisconnected(long playerId)
        {
            ConnectedPlayers.Remove(playerId);
        }

        public void EndGame()
        {
            IsGameStarted = false;
            OriginalPlayers.Clear();
            ConnectedPlayers.Clear();
            EmitSignal(SignalName.GameStateChanged, IsGameStarted);
        }
    }
}