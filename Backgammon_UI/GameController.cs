// File: GameController.cs
using Core;

namespace Backgammon_UI;
public class GameController
{
    private Game _game;
    private GameMode _gameMode;
    private CheckerColor _humanPlayerColor;
    private bool _isAIProcessing;
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

    public void ConfigureNewGame(int mode, CheckerColor playerColor, int depth, string whiteConfig, string blackConfig)
    {
        // Reset last roll buffers
        _lastRollBuffer = new int[2];
        _lastRollValues = new int[2];
        LastMovesBuffer = new List<(int From, int To)>();
        CurrentTurnState = TurnState.RollingDice;
        // Set the game mode
        _gameMode = (GameMode)mode;
        
        // Initialize the game instance
        _game = new Game(whiteConfig, blackConfig)
        {
            depth = depth
        };
        
        _humanPlayerColor = playerColor;
        if (!IsHumanTurn())
        {
            HandleTurn();
        }
        // Notify UI to update the board and settings
        OnGameUpdated?.Invoke();
    }
    
    public void StartTurn()
    {
        CurrentTurnState = TurnState.RollingDice;
        OnGameUpdated?.Invoke();
    }
    
    private async void HandleTurn()
    {
        if (_gameMode == GameMode.PlayerVsPlayer || 
            (_gameMode == GameMode.PlayerVsAI && IsHumanTurn()))
        {
            // Wait for player input (handled by MainFrontend)
            return;
        }
        // Prevent re-triggering AI calculations while it's still processing
        if (_isAIProcessing) return;
        _isAIProcessing = true;

        // AI Turn
        RollDice();
        var bestMoves = await Task.Run(() => _game.GetBestMoveForCurrentPlayer());
        LastMovesBuffer.Clear();
        // Execute each move one by one with UI updates
        foreach (var move in bestMoves)
        {
            LastMovesBuffer.Add(move);
            await Task.Delay(500); // Small delay to make moves visually distinct
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                _game.ExecuteAIMoves(new List<(int From, int To)> { move });
                OnGameUpdated?.Invoke(); // Update UI after each move
            });
        }
        _lastRollValues = _lastRollBuffer;
        EndTurn();
        _isAIProcessing = false; // AI processing is finished
    }


    public void RollDice()
    {
        if (CurrentTurnState != TurnState.RollingDice)
            throw new InvalidOperationException("Cannot roll dice outside of the RollingDice state.");

        
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
            // If no further moves are available, schedule EndTurn asynchronously
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
        return moveSuccess;
    }
    
    private void EndTurn()
    {
        System.Windows.Application.Current.Dispatcher.BeginInvoke(
            new Action(() =>
            {
                CurrentTurnState = TurnState.EndTurn;
                _game.EndTurn();
                if (_game.IsGameOver)
                {
                    string winner = _game.CurrentPlayer.Color == CheckerColor.White ? "White" : "Black";
                    System.Windows.MessageBox.Show($"Game Over! {winner} player wins!",
                        "Game Over", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
                else
                {
                    StartTurn();
                    HandleTurn();
                }
                OnGameUpdated?.Invoke();
            }),
            System.Windows.Threading.DispatcherPriority.Background);
    }

    public Game GetGameState()
    {
        return _game;
    }
    public int[] GetLastRollValues()
    {
        return _lastRollValues;
    }
    
    public bool IsHumanTurn()
    {
        return _gameMode == GameMode.PlayerVsPlayer ||
               (_gameMode == GameMode.PlayerVsAI && _game.CurrentPlayer.Color == _humanPlayerColor);
    }
}