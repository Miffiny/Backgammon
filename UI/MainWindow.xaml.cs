using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using UI.utils;
using UI.ViewModels;

namespace UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private CheckerViewModel selectedChecker;

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new GameViewModel(new Core.Game());
    }

    // MouseMove to initiate drag
    private void Ellipse_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            Ellipse draggedEllipse = sender as Ellipse;
            if (draggedEllipse != null)
            {
                DragDrop.DoDragDrop(draggedEllipse, draggedEllipse.DataContext, DragDropEffects.Move);
            }
        }
    }

    private void Ellipse_OnDragOver(object sender, DragEventArgs e)
    {
        Point dropPosition = e.GetPosition(GameBoardCanvas);

        CheckerViewModel draggedChecker = e.Data.GetData(typeof(CheckerViewModel)) as CheckerViewModel;
        if (draggedChecker != null)
        {
            draggedChecker.XPosition = dropPosition.X - 35;
            draggedChecker.YPosition = dropPosition.Y - 35;
        }
    }

    // Drop event to snap the checker to the nearest point and update the relative position
    private void Ellipse_Drop(object sender, DragEventArgs e)
    {
        Point dropPosition = e.GetPosition(GameBoardCanvas);

        CheckerViewModel draggedChecker = e.Data.GetData(typeof(CheckerViewModel)) as CheckerViewModel;
        if (draggedChecker != null)
        {
            // Find the nearest snap point
            Point nearestSnapPoint = FindNearestSnapPoint(dropPosition);

            // Snap the checker to that position visually
            draggedChecker.XPosition = nearestSnapPoint.X - 35;
            draggedChecker.YPosition = nearestSnapPoint.Y - 35;

            // Update the relative point index based on the snapped position
            draggedChecker.Position = PositionMapping.PositionToPointMap[nearestSnapPoint];
        }
    }

    // Helper method to find the nearest snap point
    private Point FindNearestSnapPoint(Point currentPos)
    {
        Point nearestPoint = PositionMapping.PointToPositionMap[1];  // Start with point 1
        double minDistance = GetDistance(currentPos, nearestPoint);

        // Loop through the points and find the closest one
        foreach (var point in PositionMapping.PointToPositionMap.Values)
        {
            double distance = GetDistance(currentPos, point);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestPoint = point;
            }
        }

        return nearestPoint;
    }

    // Calculate the Euclidean distance between two points
    private double GetDistance(Point p1, Point p2)
    {
        return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
    }
}
