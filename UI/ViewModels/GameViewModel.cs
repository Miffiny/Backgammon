using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Core;
using UI.utils;
using Point = System.Windows.Point;

namespace UI.ViewModels;

public class GameViewModel : INotifyPropertyChanged
{
    public ObservableCollection<PointViewModel> Points { get; set; }
    public ObservableCollection<CheckerViewModel> Checkers { get; set; }
    public DiceViewModel Dice { get; set; }
    public PlayerViewModel CurrentPlayer { get; set; }

    public ICommand RollDiceCommand { get; set; }
    public ICommand MoveCheckerCommand { get; set; }

    public Game _coreGame;  // Reference to the Core Game class

    public GameViewModel(Game coreGame)
    {
        _coreGame = coreGame;
        Dice = new DiceViewModel(_coreGame.Dice);  // Pass the Core Dice object to DiceViewModel
        CurrentPlayer = new PlayerViewModel();  // Initialize with the first player
        Points = new ObservableCollection<PointViewModel>();
        Checkers = new ObservableCollection<CheckerViewModel>();

        RollDiceCommand = new RelayCommand(RollDice);
        MoveCheckerCommand = new RelayCommand(MoveChecker);

        InitializeBoard();
        InitializeCheckers();  // Initialize the checkers from Core
    }

    private void InitializeCheckers()
    {
        // Loop through all the points in the Core's GameBoard
        for (int pointIndex = 0; pointIndex < _coreGame.Board.Points.Length; pointIndex++)
        {
            var corePoint = _coreGame.Board.Points[pointIndex];
            int checkerCount = 0;  // Track how many checkers are already placed at the point

            // Loop through all checkers at this point in the Core
            foreach (var coreChecker in corePoint.Checkers)
            {
                // Create a ViewModel for each checker
                var checkerViewModel = new CheckerViewModel(coreChecker);

                // Set the absolute X position using the PositionMapping
                if (PositionMapping.PointToPositionMap.ContainsKey(pointIndex + 1))  // +1 to convert to 1-based point index
                {
                    Point absolutePosition = PositionMapping.PointToPositionMap[pointIndex + 1];
                    checkerViewModel.XPosition = absolutePosition.X - 35;  // Center the checker
                
                    // Calculate the Y position with stacking offset
                    // Handle top and bottom rows
                    checkerViewModel.YPosition = (pointIndex + 1 <= 12)
                        ? absolutePosition.Y - 35 - (70 * checkerCount)
                        : absolutePosition.Y - 35 + (70 * checkerCount);
                }

                // Add the checker ViewModel to the collection
                Checkers.Add(checkerViewModel);
                checkerCount++;  // Increment the checker count for stacking
            }
        }
    }




    private void InitializeBoard()
    {
        // Populate Points collection from the game board (from the Core)
        for (int i = 0; i < 24; i++)
        {
            Points.Add(new PointViewModel(i));
        }
    }

    private void RollDice(object parameter)
    {
        // Use the ViewModel's Roll method, which in turn calls the Core logic
        Dice.Roll();
        OnPropertyChanged(nameof(Dice));  // Notify UI to update dice values
    }

    private void MoveChecker(object parameter)
    {
        // Logic to move a checker on the board via the Core
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

