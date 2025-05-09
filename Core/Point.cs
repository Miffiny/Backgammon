namespace Core;

public class Point
{
    public int Index { get; private set; } 
    public List<Checker> Checkers { get; private set; }

    public Point(int index)
    {
        Index = index;
        Checkers = new List<Checker>();
    }

    // Method to add a checker to the point
    public void AddChecker(Checker checker)
    {
        Checkers.Add(checker);
    }

    // Method to remove a checker from the point
    public Checker RemoveChecker()
    {
        if (Checkers.Count > 0)
        {
            Checker checker = Checkers[Checkers.Count - 1];
            Checkers.RemoveAt(Checkers.Count - 1);
            return checker;
        }
        return null!;
    }

    // Property to get the owner of the point
    public CheckerColor? Owner => Checkers.Count > 0 ? Checkers[0].Color : null;

    // Property to check if the point is occupied by a single opponent's checker
    public bool IsBlot(CheckerColor playerColor)
    {
        return Checkers.Count == 1 && Checkers[0].Color != playerColor;
    }
}