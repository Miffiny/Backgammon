namespace Core;

public class Player
{
    public CheckerColor Color { get; private set; }  // The color representing the player's checkers
    public List<Checker> Checkers { get; private set; }  // The player's checkers currently in play on the board

    public Player(CheckerColor color)
    {
        Color = color;
        Checkers = new List<Checker>();
    }

    // Method to move a checker from the bar back onto the board
    /*
    public Checker ReEnterFromBar()
    {
        if (Bar.Count > 0)
        {
            Checker checker = Bar[Bar.Count - 1];
            Bar.RemoveAt(Bar.Count - 1);
            return checker;
        }
        return null!;
    }*/

    // Method to bear off a checker
    public void BearOffChecker(Checker checker)
    {
        Checkers.Remove(checker);
    }

    // Method to add a checker to the bar
    /*
    public void HitChecker(Checker checker)
    {
        Checkers.Remove(checker);
        Bar.Add(checker);
    }
    */

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