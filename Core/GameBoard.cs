namespace Core;

public class GameBoard
{
    public Point[] Points { get; private set; }  // An array of 24 points on the board
    public List<Checker> WhiteBar { get; private set; } // White checkers on the bar
    public List<Checker> BlackBar { get; private set; } // Black checkers on the bar

    public GameBoard()
    {
        // Initialize the 24 points on the board
        Points = new Point[24];
        for (int i = 0; i < 24; i++)
        {
            Points[i] = new Point(i + 1);
        }
        WhiteBar = new List<Checker>();
        BlackBar = new List<Checker>();
    }
    
    public GameBoard Clone()
    {
        // Create a new GameBoard instance
        var clonedBoard = new GameBoard();

        // Clone all points and their checkers
        for (int i = 0; i < Points.Length; i++)
        {
            var originalPoint = Points[i];
            var clonedPoint = clonedBoard.Points[i];

            foreach (var checker in originalPoint.Checkers)
            {
                // Create a new checker with the same properties
                var clonedChecker = new Checker(checker.Color, checker.Position);
                clonedPoint.AddChecker(clonedChecker);
            }
        }

        // Clone the bars
        foreach (var checker in WhiteBar)
        {
            clonedBoard.WhiteBar.Add(new Checker(checker.Color, checker.Position));
        }
        foreach (var checker in BlackBar)
        {
            clonedBoard.BlackBar.Add(new Checker(checker.Color, checker.Position));
        }

        return clonedBoard;
    }
    
    // Add a checker to the respective bar
    public void AddToBar(Checker checker)
    {
        if (checker.Color == CheckerColor.White)
        {
            WhiteBar.Add(checker);
        }
        else
        {
            BlackBar.Add(checker);
        }
        checker.Position = 0; // Position 0 indicates the bar
    }

    // Remove a checker from the respective bar
    public Checker RemoveFromBar(CheckerColor color)
    {
        if (color == CheckerColor.White && WhiteBar.Count > 0)
        {
            var checker = WhiteBar[WhiteBar.Count - 1];
            WhiteBar.RemoveAt(WhiteBar.Count - 1);
            return checker;
        }
        else if (color == CheckerColor.Black && BlackBar.Count > 0)
        {
            var checker = BlackBar[BlackBar.Count - 1];
            BlackBar.RemoveAt(BlackBar.Count - 1);
            return checker;
        }
        return null!;
    }

    // Get bar contents for a specific color
    public List<Checker> GetBar(CheckerColor color)
    {
        return color == CheckerColor.White ? WhiteBar : BlackBar;
    }

    // Method to move a checker from one point to another
    public void MoveChecker(Player currentPlayer, Player opponent, int fromIndex, int toIndex)
    {
        if (fromIndex < 1 || fromIndex > 24 || toIndex < 1 || toIndex > 24)
        {
            return; // Invalid move
        }

        Point fromPoint = Points[fromIndex - 1];
        Point toPoint = Points[toIndex - 1];

        // Remove the checker from the current point
        Checker checker = fromPoint.RemoveChecker();

        // Handle hitting an opponent's checker
        if (toPoint.IsBlot(currentPlayer.Color))
        {
            Checker hitChecker = toPoint.RemoveChecker();
            hitChecker.Position = 0; // Indicate that the checker is on the bar

            // Add the hit checker to the opponent's bar, not the current player’s
            AddToBar(hitChecker);
        }

        // Add the checker to the new point
        toPoint.AddChecker(checker);
        checker.Position = toIndex;
    }


    // Method to check if a move is valid
    public bool IsMoveValid(Player player, int fromIndex, int toIndex, int diceRoll)
    {
        if (fromIndex < 1 || fromIndex > 24 || toIndex < 1 || toIndex > 24)
        {
            return false; // Invalid indices
        }

        var converted_from = fromIndex - 1;
        var converted_to = toIndex - 1; 
        Point fromPoint = Points[converted_from];
        Point toPoint = Points[converted_to];

        // Check if the destination point is a valid move
        return fromPoint.Owner == player.Color &&
               (toPoint.Owner == null || toPoint.Owner == player.Color || toPoint.IsBlot(player.Color)) &&
               (toIndex == fromIndex + diceRoll || toIndex == fromIndex - diceRoll);
    }
}