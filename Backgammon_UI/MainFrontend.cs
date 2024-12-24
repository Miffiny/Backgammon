﻿using Core;
using System.Windows.Controls;
using System.Windows.Media;

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
        private Button? _rollDiceButton;
        private Panel? _whiteBearOffZone;
        private Panel? _blackBearOffZone;

        private int? _selectedPoint;

        public MainFrontend()
        {
            // Initialize controller and UI updater
            _gameController = new GameController();
            _uiUpdater = new UIUpdater();
        }

        // Delayed initialization to set up UI references
        public void Initialize(Panel gameBoard, TextBlock dice1, TextBlock dice2, TextBlock currentPlayer,
            Button rollDiceButton,  Panel whiteBearOffZone, Panel blackBearOffZone)
        {
            _gameBoard = gameBoard;
            _dice1 = dice1;
            _dice2 = dice2;
            _currentPlayer = currentPlayer;
            _rollDiceButton = rollDiceButton;
            _whiteBearOffZone = whiteBearOffZone;
            _blackBearOffZone = blackBearOffZone;

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
            UpdateUI();
        }

         public void HandlePointClick(int pointIndex)
        {
            if (_selectedPoint == null)
            {
                _selectedPoint = pointIndex;

                // Highlight source point and available moves
                HighlightSourcePoint(pointIndex);
                HighlightAvailableMoves(pointIndex);
            }
            else
            {
                if (_selectedPoint == pointIndex)
                {
                    ResetSelection();
                    return;
                }

                bool moveSuccessful = MakeMove(_selectedPoint.Value, pointIndex);
                if (!moveSuccessful)
                {
                    // Notify user to roll dice first if applicable
                    NotifyUser("Invalid move or roll the dice first.");
                }
                else
                {
                    UpdateUI();
                }
                ResetSelection(); // Reset after move
            }
        }
         
        public void HandleBarSlotClick(int barIndex)
        {
            if (_gameBoard == null)
                return;

            // Determine the color of the checker in the bar slot
            var checkerColor = barIndex <= 6 ? CheckerColor.White : CheckerColor.Black;

            // Compare checker color with the current player
            if (checkerColor != GetGameState().CurrentPlayer.Color)
            {
                NotifyUser("You can only select your own checkers.");
                return;
            }

            if (_selectedPoint == 0)
            {
                ResetSelection();
                return;
            }
            // Set the selected point to the bar index (0 for bar)
            _selectedPoint = 0;

            // Highlight the selected bar slot
            var barSlotUI = _gameBoard.FindName($"BarSlot_{barIndex}") as StackPanel;
            if (barSlotUI != null)
            {
                barSlotUI.Background = Brushes.DarkGray;
            }
            // Highlight available moves for the bar point (index 0)
            HighlightAvailableMoves(0);
        }
        
        public void HandleBearOffZoneClick(int color)
        {
            if (_selectedPoint == null)
            {
                NotifyUser("Please select a point first.");
                return;
            }

            CheckerColor playerColor = (color == 0) ? CheckerColor.White : CheckerColor.Black;

            // Validate the current player matches the bear off zone color
            if (GetGameState().CurrentPlayer.Color != playerColor)
            {
                NotifyUser("You can only bear off your own checkers.");
                return;
            }

            // Check if bearing off is allowed
            if (!GetGameState().CanBearOff())
            {
                NotifyUser("You cannot bear off yet. Move all your checkers into the home board first.");
                return;
            }

            // Check if bearing off is possible from the selected index
            if (!GetGameState().CanBearOffFromIndex(_selectedPoint.Value))
            {
                NotifyUser("You cannot bear off from the selected point.");
                return;
            }

            // Attempt to bear off the checker
            bool moveSuccessful = _gameController.BearOffChecker(_selectedPoint.Value);
            if (moveSuccessful)
            {
                UpdateUI();
            }
            else
            {
                NotifyUser("Failed to bear off the checker.");
                ResetSelection();
            }

            // Reset selection after attempt
            ResetSelection();
        }


        private void HighlightSourcePoint(int pointIndex)
        {
            if (_gameBoard == null)
                return;

            var pointUI = _gameBoard.FindName($"Point_{pointIndex}") as StackPanel;
            if (pointUI != null)
            {
                pointUI.Background = Brushes.DarkGray;
            }
        }

        private void HighlightAvailableMoves(int pointIndex)
        {
            if (_gameBoard == null || _whiteBearOffZone == null || _blackBearOffZone == null)
                return;

            int[] availableEndPoints = GetAvailableEndPoints(pointIndex);
            foreach (int endPointIndex in availableEndPoints)
            {
                var endPointUI = _gameBoard.FindName($"Point_{endPointIndex}") as StackPanel;
                if (endPointUI != null)
                {
                    endPointUI.Background = Brushes.Green;
                }
            }

            // Check if the point can bear off
            if (GetGameState().CanBearOff() && CanBearOffFromIndex(pointIndex))
            {
                if (_gameController.GetGameState().CurrentPlayer.Color == CheckerColor.White)
                {
                    _whiteBearOffZone.Background = Brushes.Green;
                }
                else
                {
                    _blackBearOffZone.Background = Brushes.Green;
                }
            }
        }
        
        private void ResetSelection()
        {
            _selectedPoint = null;

            if (_gameBoard == null || _whiteBearOffZone == null || _blackBearOffZone == null)
                return;

            // Reset all point highlights
            for (int i = 1; i <= 24; i++)
            {
                var pointUI = _gameBoard.FindName($"Point_{i}") as StackPanel;
                if (pointUI != null)
                {
                    pointUI.Background = Brushes.SaddleBrown;
                }
            }

            // Reset all bar slot highlights
            for (int i = 1; i <= 12; i++)
            {
                var barSlotUI = _gameBoard.FindName($"BarSlot_{i}") as StackPanel;
                if (barSlotUI != null)
                {
                    barSlotUI.Background = Brushes.BurlyWood;
                }
            }

            // Reset bear off zones
            _whiteBearOffZone.Background = Brushes.LightGray;
            _blackBearOffZone.Background = Brushes.LightGray;
        }
        public int[] GetAvailableEndPoints(int startIndex)
        {
            return _gameController.GetAvailableEndPoints(startIndex);
        }

        public bool MakeMove(int fromIndex, int toIndex)
        {
            return _gameController.TryMoveChecker(fromIndex, toIndex);
        }
        
        public void NotifyUser(string message)
        {
            // Example: Display a message box or update a notification panel
            System.Windows.MessageBox.Show(message, "Game Notification", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        
        public bool CanBearOffFromIndex(int srcIndex)
        {
            return _gameController.GetGameState().CanBearOffFromIndex(srcIndex);
        }
        
        public void UpdateUI()
        {
            var game = _gameController.GetGameState();
            if (_gameBoard == null || _dice1 == null || _dice2 == null || _currentPlayer == null ||
                _rollDiceButton == null || _whiteBearOffZone == null || _blackBearOffZone == null)
            {
                // Avoid null reference exceptions if not fully initialized
                return;
            }

            _uiUpdater.UpdateGameBoard(game, _gameBoard);
            _uiUpdater.UpdateDice(game, _dice1, _dice2);
            _uiUpdater.UpdateCurrentPlayer(game, _currentPlayer);
            _uiUpdater.UpdateBearOffZones(game, _whiteBearOffZone, _blackBearOffZone);

            // Enable or disable UI elements based on the current turn state
            var state = _gameController.CurrentTurnState;
            _rollDiceButton.IsEnabled = (state == GameController.TurnState.RollingDice);
        }

        private Game GetGameState()
        {
            return _gameController.GetGameState();
        }
    }
}