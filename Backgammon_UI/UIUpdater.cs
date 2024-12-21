using Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Backgammon_UI;
public class UIUpdater
{
    public void UpdateGameBoard(Game game, Panel gameBoard)
    {
        // Clear all points and bar slots
        for (int i = 1; i <= 24; i++)
        {
            var pointUI = gameBoard.FindName($"Point_{i}") as StackPanel;
            if (pointUI != null)
            {
                pointUI.Children.Clear();

                // Apply vertical mirroring for bottom row points
                if (i >= 1 && i <= 12) // Bottom row points
                {
                    pointUI.LayoutTransform = new ScaleTransform(1, -1); // Flip vertically
                }
                else
                {
                    pointUI.LayoutTransform = null; // Reset for top row
                }
            }
        }

        for (int i = 1; i <= 8; i++)
        {
            var barSlotUI = gameBoard.FindName($"BarSlot_{i}") as StackPanel;
            if (barSlotUI != null)
            {
                barSlotUI.Children.Clear();
            }
        }

        // Populate checkers on points
        foreach (var point in game.Board.Points)
        {
            var pointUI = gameBoard.FindName($"Point_{point.Index}") as StackPanel;
            if (pointUI != null)
            {
                double checkerSize = pointUI.ActualWidth * 0.5; // Calculate size dynamically

                foreach (var checker in point.Checkers)
                {
                    var checkerUI = new Ellipse
                    {
                        Width = checkerSize,
                        Height = checkerSize, // Height equals width
                        Fill = checker.Color == CheckerColor.White ? Brushes.White : Brushes.Black,
                        Margin = new Thickness(2) // Defines the gap between checkers
                    };
                    pointUI.Children.Add(checkerUI); // Add checkers normally
                }
            }
        }

        // Populate bar slots
        var barCheckers = game.Players[0].Bar.Concat(game.Players[1].Bar).ToList();
        for (int i = 0; i < barCheckers.Count; i++)
        {
            var barSlotUI = gameBoard.FindName($"BarSlot_{i + 1}") as StackPanel;
            if (barSlotUI != null)
            {
                double checkerSize = barSlotUI.ActualWidth * 0.7; // Calculate size dynamically

                var checker = barCheckers[i];
                var checkerUI = new Ellipse
                {
                    Width = checkerSize,
                    Height = checkerSize, // Height equals width
                    Fill = checker.Color == CheckerColor.White ? Brushes.White : Brushes.Black,
                    Margin = new Thickness(2)
                };
                barSlotUI.Children.Add(checkerUI);
            }
        }
    }

    public void UpdateDice(Game game, TextBlock dice1, TextBlock dice2)
    {
        dice1.Text = game.Dice.Value1.ToString();
        dice2.Text = game.Dice.Value2.ToString();
    }

    public void UpdateCurrentPlayer(Game game, TextBlock currentPlayer)
    {
        currentPlayer.Text = $"Player: {game.CurrentPlayer.Color}";
    }
}
