﻿using Core;
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
        var whiteBarCheckers = game.Players[0].Bar; // White player checkers (1-6)
        var blackBarCheckers = game.Players[1].Bar; // Black player checkers (7-12)

// Populate white player's bar slots (1-6)
        for (int i = 0; i < whiteBarCheckers.Count; i++)
        {
            var barSlotUI = gameBoard.FindName($"BarSlot_{i + 1}") as StackPanel;
            if (barSlotUI != null)
            {
                double checkerSize = barSlotUI.ActualWidth * 0.4; // Calculate size dynamically
        
                var checkerUI = new Ellipse
                {
                    Width = checkerSize,
                    Height = checkerSize, // Height equals width
                    Fill = Brushes.White, // White checkers
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                barSlotUI.Children.Add(checkerUI);
            }
        }

// Populate black player's bar slots (7-12)
        for (int i = 0; i < blackBarCheckers.Count; i++)
        {
            var barSlotUI = gameBoard.FindName($"BarSlot_{i + 7}") as StackPanel;
            if (barSlotUI != null)
            {
                double checkerSize = barSlotUI.ActualWidth * 0.4; // Calculate size dynamically
        
                var checkerUI = new Ellipse
                {
                    Width = checkerSize,
                    Height = checkerSize, // Height equals width
                    Fill = Brushes.Black, // Black checkers
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                barSlotUI.Children.Add(checkerUI);
            }
        }
    }
    
    
    public void UpdateBearOffZones(Game game, Panel whiteBearOffZone, Panel blackBearOffZone)
    {
        // Clear existing children in bear off zones
        whiteBearOffZone.Children.Clear();
        blackBearOffZone.Children.Clear();

        // Render white checkers in the bear off zone
        double whiteCheckerSize = whiteBearOffZone.ActualHeight * 0.9;
        for (int i = 0; i < game.Players[0].BearOff.Count; i++)
        {
            var checkerUI = new Ellipse
            {
                Width = whiteCheckerSize,
                Height = whiteCheckerSize,
                Fill = Brushes.White,
                Margin = new Thickness(-whiteCheckerSize / 2, 0, 0, 0) // Overlap checkers by half
            };
            whiteBearOffZone.Children.Add(checkerUI);
        }

        // Render black checkers in the bear off zone
        double blackCheckerSize = blackBearOffZone.ActualHeight * 0.9;
        for (int i = 0; i < game.Players[1].BearOff.Count; i++)
        {
            var checkerUI = new Ellipse
            {
                Width = blackCheckerSize,
                Height = blackCheckerSize,
                Fill = Brushes.Black,
                Margin = new Thickness(-blackCheckerSize / 2, 0, 0, 0) // Overlap checkers by half
            };
            blackBearOffZone.Children.Add(checkerUI);
        }
    }

    public void UpdateDice(Game game, TextBlock dice1, TextBlock dice2)
    {
        dice1.Text = $"Dice 1 value: {game.Dice.Value1.ToString()}";
        dice2.Text = $"Dice 2 value: {game.Dice.Value2.ToString()}";
    }

    public void UpdateCurrentPlayer(Game game, TextBlock currentPlayer)
    {
        currentPlayer.Text = $"Current player: {game.CurrentPlayer.Color}";
    }
}