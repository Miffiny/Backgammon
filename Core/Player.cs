namespace Core;

public class Player
{
    public CheckerColor Color { get; private set; } 
    public List<Checker> Checkers { get; private set; } 

    public Player(CheckerColor color)
    {
        Color = color;
        Checkers = new List<Checker>();
    }

    // Method to bear off a checker
    public void BearOffChecker(Checker checker)
    {
        Checkers.Remove(checker);
    }

    // Method to add a checker to the player's list of checkers
    public void AddChecker(Checker checker)
    {
        Checkers.Add(checker);
    }

    // Property to check if all checkers are in the home board
    public bool AllCheckersInHome(int[] homeBoardIndices)
    {
        foreach (var checker in Checkers)
        {
            if (!homeBoardIndices.Contains(checker.Position))
            {
                return false;
            }
        }
        return true;
    }
}