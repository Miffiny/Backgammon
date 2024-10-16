using System.Windows;

namespace UI.utils;

public static class PositionMapping
{
    // Map from point index (1-24) to absolute canvas coordinates
    public static Dictionary<int, Point> PointToPositionMap { get; } = new Dictionary<int, Point>
    {
        { 1, new Point(1010, 815) },
        { 2, new Point(930, 815) },
        { 3, new Point(850, 815) },
        { 4, new Point(770, 815) },
        { 5, new Point(690, 815) },
        { 6, new Point(610, 815) },
        { 7, new Point(490, 815) },
        { 8, new Point(410, 815) },
        { 9, new Point(330, 815) },
        { 10, new Point(250, 815) },
        { 11, new Point(170, 815) },
        { 12, new Point(90, 815) },
        { 13, new Point(90, 85) },
        { 14, new Point(170, 85) },
        { 15, new Point(250, 85) },
        { 16, new Point(330, 85) },
        { 17, new Point(410, 85) },
        { 18, new Point(490, 85) },
        { 19, new Point(610, 85) },
        { 20, new Point(690, 85) },
        { 21, new Point(770, 85) },
        { 22, new Point(850, 85) },
        { 23, new Point(930, 85) },
        { 24, new Point(1010, 85) }
    };

    // Reverse map from absolute position to point index
    public static Dictionary<Point, int> PositionToPointMap { get; } =
        PointToPositionMap.ToDictionary(kv => kv.Value, kv => kv.Key);
}