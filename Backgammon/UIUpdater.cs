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

        for (int i = 1; i <= 12; i++)
        {
            var barSlotUI = gameBoard.FindName($"BarSlot_{i}") as Grid;
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
                double checkerSize = pointUI.ActualWidth * 0.5;

                foreach (var checker in point.Checkers)
                {
                    var checkerUI = new Ellipse
                    {
                        Width = checkerSize,
                        Height = checkerSize, // Height equals width
                        Fill = checker.Color == CheckerColor.White ? Brushes.White : Brushes.Black,
                        Margin = new Thickness(2) // Defines the gap between checkers
                    };
                    pointUI.Children.Add(checkerUI);
                }
            }
        }

        // Populate bar slots
        var whiteBarCheckers = game.Board.WhiteBar;
        var blackBarCheckers = game.Board.BlackBar;

// Populate white player's bar slots (1-6)
        for (int i = 0; i < whiteBarCheckers.Count; i++)
        {
            var barSlotUI = gameBoard.FindName($"BarSlot_{i + 1}") as Grid;
            if (barSlotUI != null)
            {
                double checkerSize = barSlotUI.ActualWidth * 0.5; // Calculate size dynamically
        
                var checkerUI = new Ellipse
                {
                    Width = checkerSize,
                    Height = checkerSize,
                    Fill = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                barSlotUI.Children.Add(checkerUI);
            }
        }
        
        for (int i = 0; i < blackBarCheckers.Count; i++)
        {
            var barSlotUI = gameBoard.FindName($"BarSlot_{i + 7}") as Grid;
            if (barSlotUI != null)
            {
                double checkerSize = barSlotUI.ActualWidth * 0.5;
        
                var checkerUI = new Ellipse
                {
                    Width = checkerSize,
                    Height = checkerSize,
                    Fill = Brushes.Black, 
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                barSlotUI.Children.Add(checkerUI);
            }
        }
    }
    
    public void ResetHighlights(Panel gameBoard, Panel whiteBearOffZone, Panel blackBearOffZone)
    {
        for (int i = 1; i <= 24; i++)
        {
            var pointUI = gameBoard.FindName($"Point_{i}") as StackPanel;
            if (pointUI != null) pointUI.Background = Brushes.SaddleBrown;
        }

        for (int i = 1; i <= 12; i++)
        {
            var barSlotUI = gameBoard.FindName($"BarSlot_{i}") as Grid;
            if (barSlotUI != null) barSlotUI.Background = Brushes.BurlyWood;
        }

        whiteBearOffZone.Background = Brushes.LightGray;
        blackBearOffZone.Background = Brushes.LightGray;
    }

    
    public void HighlightLastMoves(List<(int From, int To)> moves, Panel gameBoard, Panel whiteBearOffZone, Panel blackBearOffZone)
    {
        foreach (var move in moves)
        {
            if (move.From == 0)
            {
                var endPoint = gameBoard.FindName($"Point_{move.To}") as StackPanel;
                if (endPoint != null) endPoint.Background = Brushes.LightGreen;
            }
            else if (move.To == -1) // Highlight bear off zone
            {
                if (move.From >= 19 && move.From <= 24) whiteBearOffZone.Background = Brushes.Green;
                else blackBearOffZone.Background = Brushes.Green;
            }
            else // Highlight regular points
            {
                var endPoint = gameBoard.FindName($"Point_{move.To}") as StackPanel;
                if (endPoint != null) endPoint.Background = Brushes.LightGreen;

                var startPoint = gameBoard.FindName($"Point_{move.From}") as StackPanel;
                if (startPoint != null) startPoint.Background = Brushes.Yellow;
            }
        }
    }
    
    
    public void UpdateBearOffZones(Game game, Panel whiteBearOffZone, Panel blackBearOffZone)
    {
        whiteBearOffZone.Children.Clear();
        blackBearOffZone.Children.Clear();
        
        double whiteCheckerSize = whiteBearOffZone.ActualHeight * 0.9;
        for (int i = 0; i < 15 - game.Players[0].Checkers.Count; i++)
        {
            var checkerUI = new Ellipse
            {
                Width = whiteCheckerSize,
                Height = whiteCheckerSize,
                Fill = Brushes.White,
                Margin = new Thickness(-whiteCheckerSize / 2, 0, 0, 0)
            };
            whiteBearOffZone.Children.Add(checkerUI);
        }
        
        double blackCheckerSize = blackBearOffZone.ActualHeight * 0.9;
        for (int i = 0; i < 15 - game.Players[1].Checkers.Count; i++)
        {
            var checkerUI = new Ellipse
            {
                Width = blackCheckerSize,
                Height = blackCheckerSize,
                Fill = Brushes.Black,
                Margin = new Thickness(-blackCheckerSize / 2, 0, 0, 0)
            };
            blackBearOffZone.Children.Add(checkerUI);
        }
    }

    public void UpdateDice(Game game, TextBlock dice1, TextBlock dice2)
    {
        dice1.Text = $"Dice 1 value: {game.Dice.Value1.ToString()}";
        dice2.Text = $"Dice 2 value: {game.Dice.Value2.ToString()}";
    }
    
    public void UpdateLastRoll(int[] lastRollValues, TextBlock lastRoll1, TextBlock lastRoll2)
    {
        lastRoll1.Text = lastRollValues.Length > 0 ? $"Last Roll 1: {lastRollValues[0]}" : "Last Roll 1: -";
        lastRoll2.Text = lastRollValues.Length > 1 ? $"Last Roll 2: {lastRollValues[1]}" : "Last Roll 2: -";
    }

    public void UpdateCurrentPlayer(Game game, TextBlock currentPlayer)
    {
        currentPlayer.Text = $"Current player: {game.CurrentPlayer.Color}";
    }
}