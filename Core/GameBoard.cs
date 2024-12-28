namespace Core;

public class GameBoard
{
    public Point[] Points { get; private set; }  // An array of 24 points on the board

    public GameBoard()
    {
        // Initialize the 24 points on the board
        Points = new Point[24];
        for (int i = 0; i < 24; i++)
        {
            Points[i] = new Point(i + 1);
        }
    }
    
    public GameBoard Clone()
    {
        // Create a new GameBoard instance
        GameBoard clonedBoard = new GameBoard();

        // Clone each point on the board
        for (int i = 0; i < Points.Length; i++)
        {
            clonedBoard.Points[i] = new Point(Points[i].Index);

            // Clone each checker on the point
            foreach (var checker in Points[i].Checkers)
            {
                clonedBoard.Points[i].AddChecker(new Checker(checker.Color, checker.Position));
            }
        }

        return clonedBoard;
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
            hitChecker.Position = -1; // Indicate that the checker is on the bar

            // Add the hit checker to the opponent's bar, not the current player’s
            opponent.HitChecker(hitChecker);
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