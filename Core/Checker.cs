namespace Core;

// Core/Checker.cs
public class Checker
{
    public int Position { get; set; }
    public CheckerColor Color { get; set; }

    public Checker(CheckerColor color, int position)
    {
        Color = color;
        Position = position;
    }
}