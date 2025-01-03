// File: GameController.cs
using Core;

namespace Backgammon_UI;
public class GameController
{
    private Game _game;
    private GameMode _gameMode;
    private CheckerColor _humanPlayerColor;
    public List<(int From, int To)> LastMovesBuffer { get; private set; } = new List<(int From, int To)>();
    private int[] _lastRollValues = new int[2];
    private int[] _lastRollBuffer = new int[2];
    
    public enum TurnState
    {
        RollingDice,
        MakingMove,
        EndTurn
    }
    public enum GameMode
    {
        PlayerVsPlayer,
        PlayerVsAI,
        AIVsAI
    }
    public TurnState CurrentTurnState;
    public event Action? OnGameUpdated;

    public GameController()
    {
        _game = new Game();
    }

    public void ConfigureNewGame(int mode, CheckerColor playerColor, int depth)
    {
        // Reset last roll buffers
        _lastRollBuffer = new int[2];
        _lastRollValues = new int[2];
        LastMovesBuffer = new List<(int From, int To)>();
        CurrentTurnState = TurnState.RollingDice;
        // Initialize the game instance
        _game = new Game
        {
            depth = depth
        };
        
        // Set the game mode
        _gameMode = (GameMode)mode;
        if (_gameMode == GameMode.PlayerVsAI)
        {
            _humanPlayerColor = playerColor;
        }
        
        // Ensure the correct starting player
        if (playerColor == CheckerColor.Black && _game.CurrentPlayer.Color == CheckerColor.White)
        {
            SwitchTurn();
        }
        // Notify UI to update the board and settings
        OnGameUpdated?.Invoke();
    }
    
    public void StartTurn()
    {
        CurrentTurnState = TurnState.RollingDice;
        OnGameUpdated?.Invoke();
    }
    
    private void HandleTurn()
    {
        if (_gameMode == GameMode.PlayerVsPlayer || 
            (_gameMode == GameMode.PlayerVsAI && IsHumanTurn()))
        {
            // Wait for player input (handled by MainFrontend)
            return;
        }

        // AI Turn
        RollDice();
        var bestMoves = _game.GetBestMoveForCurrentPlayer();
        LastMovesBuffer.AddRange(bestMoves); // Add all AI moves to the buffer
        _game.ExecuteAIMoves(bestMoves);
        EndTurn();
    }


    public void RollDice()
    {
        if (CurrentTurnState != TurnState.RollingDice)
            throw new InvalidOperationException("Cannot roll dice outside of the RollingDice state.");

        LastMovesBuffer.Clear();
        _lastRollValues = _lastRollBuffer;
        _game.RollDice();
        _lastRollBuffer = _game.Dice.GetDiceValues();
        CurrentTurnState = TurnState.MakingMove;

        // Check if moves are possible
        if (!_game.HasAvailableMoves())
        {
            EndTurn();
        }

        OnGameUpdated?.Invoke();
    }
    
    public int[] GetAvailableEndPoints(int startIndex)
    {
        return _game.GetAvailableEndPoints(startIndex);
    }

    public bool TryMoveChecker(int fromIndex, int toIndex)
    {
        bool moveSuccess = _game.MakeMove(fromIndex, toIndex);
        if (moveSuccess)
        {
            LastMovesBuffer.Add((fromIndex, toIndex)); // Track the move
            // Check if further moves are available
            if (!_game.HasAvailableMoves())
            {
                EndTurn();
            }
        }
        OnGameUpdated?.Invoke();
        return moveSuccess;
    }

    public bool BearOffChecker(int fromIndex)
    {
        bool moveSuccess = _game.BearOffChecker(fromIndex);
        if (moveSuccess)
        {
            // Check if further moves are available
            if (!_game.HasAvailableMoves())
            {
                EndTurn();
            }
        }
        OnGameUpdated?.Invoke();
        return moveSuccess;
    }
    
    private void EndTurn()
    {
        CurrentTurnState = TurnState.EndTurn;
        _game.EndTurn();
        if (_game.IsGameOver)
        {
            // Notify user that the game is over and declare the winner
            string winner = _game.CurrentPlayer.Color == CheckerColor.White ? "White" : "Black";
            System.Windows.MessageBox.Show($"Game Over! {winner} player wins!", "Game Over", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            return; // Exit turn flow since the game is over
        }
        StartTurn();
        SwitchTurn();
    }

    public Game GetGameState()
    {
        return _game;
    }
    public int[] GetLastRollValues()
    {
        return _lastRollValues;
    }

    public void SwitchTurn()
    {
        HandleTurn();
    }
    private bool IsHumanTurn()
    {
        return _gameMode == GameMode.PlayerVsPlayer ||
               (_gameMode == GameMode.PlayerVsAI && _game.CurrentPlayer.Color == _humanPlayerColor);
    }
}