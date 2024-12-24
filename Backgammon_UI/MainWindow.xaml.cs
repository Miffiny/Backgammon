using System.Windows;
using System.Windows.Controls;

namespace Backgammon_UI
{
    public partial class MainWindow
    {
        private MainFrontend _mainFrontend;
        

        public MainWindow()
        {
            InitializeComponent();

            // Initialize MainFrontend with UI components
            _mainFrontend = new MainFrontend();
            
            // Attach event handlers to points
            for (int i = 1; i <= 24; i++)
            {
                var pointUI = FindName($"Point_{i}") as StackPanel;
                if (pointUI != null)
                {
                    pointUI.MouseLeftButtonDown += Point_Click;
                }
            }
            
            // Attach event handlers to bar slots
            for (int i = 1; i <= 12; i++)
            {
                var barSlotUI = FindName($"BarSlot_{i}") as StackPanel;
                if (barSlotUI != null)
                {
                    barSlotUI.MouseLeftButtonDown += BarSlot_Click;
                }
            }

            // Listen for Loaded event to initialize the game
            Loaded += OnLoaded;

            // Listen for SizeChanged event to dynamically resize checkers
            SizeChanged += OnSizeChanged;
        }
        
        private void Point_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is StackPanel pointUI)
            {
                // Get the clicked point index
                int pointIndex = int.Parse(pointUI.Name.Split('_')[1]);

                // Delegate the click handling to MainFrontend
                _mainFrontend.HandlePointClick(pointIndex);
            }
        }
        
        private void BarSlot_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is StackPanel barSlotUI)
            {
                // Get the clicked bar slot index
                int barIndex = int.Parse(barSlotUI.Name.Split('_')[1]);

                // Check if the bar slot has a checker
                if (barSlotUI.Children.Count > 0)
                {
                    // Delegate handling to MainFrontend
                    _mainFrontend.HandleBarSlotClick(barIndex);
                }
            }
        }
        
        private void WhiteBearOffZone_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mainFrontend.HandleBearOffZoneClick(0);
        }

        private void BlackBearOffZone_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mainFrontend.HandleBearOffZoneClick(1);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Initialize MainFrontend with UI components
            _mainFrontend.Initialize(GameBoard, Dice1, Dice2, CurrentPlayer, RollDiceButton, WhiteBearOffZone, BlackBearOffZone);

            // Populate the board with checkers
            _mainFrontend.UpdateUI();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Call UIUpdater through MainFrontend to dynamically resize checkers
            _mainFrontend.UpdateUI();
        }
        
        private void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrontend.RollDice();
            _mainFrontend.UpdateUI(); // Update the UI after rolling dice
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrontend.StartNewGame();
            _mainFrontend.UpdateUI(); // Update the UI for the new game
        }

    }
}