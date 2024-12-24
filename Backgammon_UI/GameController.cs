// File: GameController.cs
using Core;

namespace Backgammon_UI;
public class GameController
{
    private Game _game;
    
    public enum TurnState
    {
        RollingDice,
        MakingMove,
        EndTurn
    }
    public TurnState CurrentTurnState;
    public event Action? OnGameUpdated;

    public GameController()
    {
        _game = new Game();
    }

    public void NewGame()
    {
        _game = new Game();
        CurrentTurnState = TurnState.RollingDice;
        OnGameUpdated?.Invoke();
    }

    public void StartTurn()
    {
        CurrentTurnState = TurnState.RollingDice;
        OnGameUpdated?.Invoke();
    }

    public void RollDice()
    {
        if (CurrentTurnState != TurnState.RollingDice)
            throw new InvalidOperationException("Cannot roll dice outside of the RollingDice state.");

        _game.RollDice();
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
    }

    public Game GetGameState()
    {
        return _game;
    }
}