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
                var barSlotUI = FindName($"BarSlot_{i}") as Grid;
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
                
                _mainFrontend.HandlePointClick(pointIndex);
            }
        }
        
        private void BarSlot_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Grid barSlotUI)
            {
                // Get the clicked bar slot index
                int barIndex = int.Parse(barSlotUI.Name.Split('_')[1]);

                // Check if the bar slot has a checker
                if (barSlotUI.Children.Count > 0)
                {
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
            _mainFrontend.Initialize(GameBoard, Dice1, Dice2, LastRoll1, LastRoll2, CurrentPlayer, RollDiceButton, WhiteBearOffZone, BlackBearOffZone);
            
            _mainFrontend.UpdateUI();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _mainFrontend.UpdateUI();
        }
        
        private void RollDiceButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrontend.RollDice();
            _mainFrontend.UpdateUI(); // Update the UI after rolling dice
        }
        
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrontend.StopGame();
            StopButton.IsEnabled = false;
            RollDiceButton.IsEnabled = false;
            ResumeButton.IsEnabled = true;
        }

        private void ResumeButton_Click(object sender, RoutedEventArgs e)
        {
            _mainFrontend.ResumeGame();
            RollDiceButton.IsEnabled = true;
            ResumeButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }
        
        private void GameModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GameModeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                string? selectedMode = selectedItem.Content.ToString();

                bool isPvAI = selectedMode == "Player vs AI";
                bool isAIvsAI = selectedMode == "AI vs AI";
                bool isPvP = selectedMode == "Player vs Player";
                bool isAIInvolved = !isPvP;

                // Show/hide side and depth
                SideLabel.Visibility = isPvAI ? Visibility.Visible : Visibility.Collapsed;
                PlayerSideComboBox.Visibility = isPvAI ? Visibility.Visible : Visibility.Collapsed;

                AIDepthLabel.Visibility = isAIInvolved ? Visibility.Visible : Visibility.Collapsed;
                AIDepthComboBox.Visibility = isAIInvolved ? Visibility.Visible : Visibility.Collapsed;

                // Roll Dice button only in PvP and PvAI
                RollDiceButton.Visibility = isAIvsAI ? Visibility.Collapsed : Visibility.Visible;

                // Stop/Resume only in AI vs AI
                StopButton.Visibility = isAIvsAI ? Visibility.Visible : Visibility.Collapsed;
                ResumeButton.Visibility = isAIvsAI ? Visibility.Visible : Visibility.Collapsed;

                bool showAIConfig = !isPvP;

                var whiteFactorElements = new List<UIElement>
                {
                    WhiteFactorsPanel, WhiteFactorsLabel,
                    WhiteFactor0, WhiteFactor1, WhiteFactor2, WhiteFactor3, WhiteFactor4
                };

                var blackFactorElements = new List<UIElement>
                {
                    BlackFactorsPanel, BlackFactorsLabel,
                    BlackFactor0, BlackFactor1, BlackFactor2, BlackFactor3, BlackFactor4
                };

                foreach (var element in whiteFactorElements.Concat(blackFactorElements))
                {
                    element.Visibility = showAIConfig ? Visibility.Visible : Visibility.Collapsed;
                }
                
                GameConfigPanel.SetValue(Grid.ColumnSpanProperty,
                    (isPvP) ? 3 : 1); // Center if middle and right panels are hidden

                GameConfigPanel.SetValue(Grid.ColumnProperty, 0);
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Get selected game mode
            string? selectedMode = (GameModeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedMode))
            {
                selectedMode = "Player vs Player";
            }

            // Get selected side
            string? selectedSide = (PlayerSideComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(selectedSide))
            {
                selectedSide = "Random";
            }

            // Get selected AI depth
            string? selectedDepth = (AIDepthComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            int depth = 0;
            if (!string.IsNullOrEmpty(selectedDepth))
            {
                depth = int.Parse(selectedDepth) - 1;
            }

            // Configure GameController based on selection
            int mode = selectedMode switch
            {
                "Player vs Player" => 0,
                "Player vs AI" => 1,
                "AI vs AI" => 2,
                _ => throw new InvalidOperationException("Invalid game mode selected.")
            };

            int playerColor = selectedSide switch
            {
                "White" => 0,
                "Black" => 1,
                "Random" => (new Random().Next(0, 2) == 0) ? 0 : 1,
                _ => throw new InvalidOperationException("Invalid player side selected.")
            };
            
            // Generate AI factor strings
            string whiteAIConfig = GenerateAIConfigString("White");
            string blackAIConfig = GenerateAIConfigString("Black");

            // Start a new game
            _mainFrontend.StartNewGame(mode, playerColor, depth, whiteAIConfig, blackAIConfig);
        }

        private string GenerateAIConfigString(string color)
        {
            var factors = new List<int>();

            for (int i = 0; i <= 5; i++)
            {
                var checkBox = FindName($"{color}Factor{i}") as CheckBox;
                if (checkBox?.IsChecked == true)
                    factors.Add(i);
            }

            // Concatenate selected factors into a single string
            return string.Join("", factors);
        }
    }
    
    
}