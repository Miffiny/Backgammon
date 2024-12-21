using Core;
using System.Windows.Controls;

namespace Backgammon_UI
{
    public class MainFrontend
    {
        private GameController _gameController;
        private UIUpdater _uiUpdater;

        private Panel? _gameBoard;
        private TextBlock? _dice1;
        private TextBlock? _dice2;
        private TextBlock? _currentPlayer;

        private int? _selectedPoint;

        public MainFrontend()
        {
            // Initialize controller and UI updater
            _gameController = new GameController();
            _uiUpdater = new UIUpdater();
        }

        // Delayed initialization to set up UI references
        public void Initialize(Panel gameBoard, TextBlock dice1, TextBlock dice2, TextBlock currentPlayer)
        {
            _gameBoard = gameBoard;
            _dice1 = dice1;
            _dice2 = dice2;
            _currentPlayer = currentPlayer;

            // Subscribe to game updates
            _gameController.OnGameUpdated += UpdateUI;

            StartNewGame();
        }

        public void StartNewGame()
        {
            _gameController.NewGame();
        }

        public void RollDice()
        {
            _gameController.RollDice();
        }

        public void HandlePointClick(int pointIndex)
        {
            if (_selectedPoint == null)
            {
                _selectedPoint = pointIndex;
            }
            else
            {
                _gameController.TryMoveChecker(_selectedPoint.Value, pointIndex);
                _selectedPoint = null;
            }
        }

        public void UpdateUI()
        {
            var game = _gameController.GetGameState();
            if (_gameBoard == null || _dice1 == null || _dice2 == null || _currentPlayer == null)
            {
                // Avoid null reference exceptions if not fully initialized
                return;
            }

            _uiUpdater.UpdateGameBoard(game, _gameBoard);
            _uiUpdater.UpdateDice(game, _dice1, _dice2);
            _uiUpdater.UpdateCurrentPlayer(game, _currentPlayer);
        }

        public Game GetGameState()
        {
            return _gameController.GetGameState();
        }
    }
}