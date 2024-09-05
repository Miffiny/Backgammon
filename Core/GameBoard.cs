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

    // Method to move a checker from one point to another
    public void MoveChecker(Player player, int fromIndex, int toIndex)
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
        if (toPoint.IsBlot(player.Color))
        {
            Checker hitChecker = toPoint.RemoveChecker();
            hitChecker.Position = -1; // Indicate that the checker is on the bar
            player.HitChecker(hitChecker);
        }

        // Add the checker to the destination point
        checker.Position = toIndex;
        toPoint.AddChecker(checker);
    }

    // Method to check if a move is valid
    public bool IsMoveValid(Player player, int fromIndex, int toIndex, int diceRoll)
    {
        if (fromIndex < 1 || fromIndex > 24 || toIndex < 1 || toIndex > 24)
        {
            return false; // Invalid indices
        }

        Point fromPoint = Points[fromIndex - 1];
        Point toPoint = Points[toIndex - 1];

        // Check if the destination point is a valid move
        return fromPoint.Owner == player.Color &&
               (toPoint.Owner == null || toPoint.Owner == player.Color || toPoint.IsBlot(player.Color)) &&
               (toIndex == fromIndex + diceRoll || toIndex == fromIndex - diceRoll);
    }
}