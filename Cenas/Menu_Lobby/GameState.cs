using Godot;
using System.Collections.Generic;

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
        foreach (var player in playersAtStart)
        {
            OriginalPlayers.Add(player.Key);
            ConnectedPlayers[player.Key] = player.Value;
        }
        
        EmitSignal(SignalName.GameStateChanged, IsGameStarted);
        GD.Print($"Jogo iniciado com {OriginalPlayers.Count} jogadores originais");
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
            GD.Print($"Jogador {playerName} (ID: {playerId}) RECONECTOU à partida");
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