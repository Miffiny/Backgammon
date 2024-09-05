namespace Core;

public class Checker
{
    public int Position { get; set; }  // The current position of the checker (1-24 for board points, or special values for bar/bear-off)
    public string Color { get; set; }  // The color of the checker (e.g., "White" or "Black")

    public Checker(string color, int position)
    {
        Color = color;
        Position = position;
    }
}