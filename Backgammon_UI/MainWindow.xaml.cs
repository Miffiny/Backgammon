using System.Windows;
using Core;

namespace Backgammon_UI
{
    public partial class MainWindow : Window
    {
        private MainFrontend _mainFrontend;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize MainFrontend with UI components
            _mainFrontend = new MainFrontend();

            // Listen for Loaded event to initialize the game
            Loaded += OnLoaded;

            // Listen for SizeChanged event to dynamically resize checkers
            SizeChanged += OnSizeChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Initialize MainFrontend with UI components
            _mainFrontend.Initialize(GameBoard, Dice1, Dice2, CurrentPlayer);

            // Populate the board with checkers
            _mainFrontend.UpdateUI();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Call UIUpdater through MainFrontend to dynamically resize checkers
            _mainFrontend.UpdateUI();
        }
    }
}