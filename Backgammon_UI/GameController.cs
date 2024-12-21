// File: GameController.cs
using Core;

namespace Backgammon_UI;
public class GameController
{
    private Game _game;

    public event Action? OnGameUpdated;

    public GameController()
    {
        _game = new Game();
    }

    public void NewGame()
    {
        _game = new Game();
        OnGameUpdated?.Invoke();
    }

    public void RollDice()
    {
        _game.RollDice();
        OnGameUpdated?.Invoke();
    }

    public bool TryMoveChecker(int fromIndex, int toIndex)
    {
        bool moveSuccess = _game.MakeMove(fromIndex, toIndex);
        OnGameUpdated?.Invoke();
        return moveSuccess;
    }

    public Game GetGameState()
    {
        return _game;
    }
}