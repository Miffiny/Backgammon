namespace Core;

public class Game
{
    public Player[] Players { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public Dice Dice { get; private set; }
    public GameBoard Board { get; private set; }
    public bool IsGameOver { get; private set; }
    private int currentPlayerIndex;

    public Game()
    {
        Players =
        [
            new Player(CheckerColor.White),
            new Player(CheckerColor.Black)
        ];

        Board = new GameBoard();
        Dice = new Dice();
        currentPlayerIndex = 0;
        CurrentPlayer = Players[currentPlayerIndex];
        IsGameOver = false;
        
        InitializeCheckers();  // Call the method to initialize checkers
    }

    // Method to initialize checkers on the board
    private void InitializeCheckers()
    {
        // Initialize white checkers (Player 0 - White)
        AddCheckersToPoint(Players[0], 1, 2);    // 2 white checkers at point 1
        AddCheckersToPoint(Players[0], 12, 5);   // 5 white checkers at point 12
        AddCheckersToPoint(Players[0], 17, 3);   // 3 white checkers at point 17
        AddCheckersToPoint(Players[0], 19, 5);   // 5 white checkers at point 19

        // Initialize black checkers (Player 1 - Black)
        AddCheckersToPoint(Players[1], 24, 2);   // 2 black checkers at point 24
        AddCheckersToPoint(Players[1], 13, 5);   // 5 black checkers at point 13
        AddCheckersToPoint(Players[1], 8, 3);    // 3 black checkers at point 8
        AddCheckersToPoint(Players[1], 6, 5);    // 5 black checkers at point 6
    }
    
    // Helper method to add checkers to a specific point on the board
    private void AddCheckersToPoint(Player player, int pointIndex, int count)
    {
        for (int i = 0; i < count; i++)
        {
            var checker = new Checker(player.Color, pointIndex);  // Create checker with player's color
            player.AddChecker(checker);  // Add checker to player's list
            Board.Points[pointIndex - 1].AddChecker(checker);  // Add checker to the respective point on the board
        }
    }
    public void RollDice()
    {
        Dice.Roll();
    }

    public bool IsMoveValid(int fromIndex, int toIndex, int diceValue)
    {
        if (fromIndex < 1 || fromIndex > 24 || toIndex < 1 || toIndex > 24)
        {
            return false; // Out of bounds
        }

        Point fromPoint = Board.Points[fromIndex - 1];
        Point toPoint = Board.Points[toIndex - 1];

        // Enforce moving in the correct direction
        if (CurrentPlayer.Color == CheckerColor.White && toIndex <= fromIndex)
        {
            return false;  // White moves forward (to higher indices)
        }
        if (CurrentPlayer.Color == CheckerColor.Black && toIndex >= fromIndex)
        {
            return false;  // Black moves backward (to lower indices)
        }

        // If checker is on the bar, player can only re-enter in the opponent's home board
        if (CurrentPlayer.Bar.Count > 0)
        {
            return IsValidBarMove(toIndex, diceValue);
        }

        return Board.IsMoveValid(CurrentPlayer, fromIndex, toIndex, diceValue);
    }

    public bool MakeMove(int fromIndex, int toIndex)
    {
        int[] diceValues = Dice.GetDiceValues();

        foreach (int value in diceValues)
        {
            if (IsMoveValid(fromIndex, toIndex, value))
            {
                // Pass both the current player and the opponent to the MoveChecker method
                Board.MoveChecker(CurrentPlayer, GetOpponent(), fromIndex, toIndex);
                Dice.UseDie(value);
                return true;
            }
        }

        return false;
    }

// Method to get the opponent player
    private Player GetOpponent()
    {
        return Players[(currentPlayerIndex == 0) ? 1 : 0];
    }

    private bool IsValidBarMove(int toIndex, int diceValue)
    {
        // If the player has checkers on the bar, restrict movement to entering in the opponent's home area
        if (CurrentPlayer.Color == CheckerColor.White && (toIndex < 1 || toIndex > 6))
        {
            return false;  // White re-enters only in opponent's home board (points 1-6)
        }

        if (CurrentPlayer.Color == CheckerColor.White && (toIndex < 19 || toIndex > 24))
        {
            return false;  // Black re-enters only in opponent's home board (points 19-24)
        }

        // Ensure the point is either empty or has only one opponent checker (blot)
        Point toPoint = Board.Points[toIndex - 1];
        return toPoint.Owner == null || toPoint.IsBlot(CurrentPlayer.Color);
    }

    public bool CanBearOff()
    {
        int[] homeBoardIndices = (CurrentPlayer.Color == CheckerColor.White) ? new int[] { 19, 20, 21, 22, 23, 24 } : new int[] { 1, 2, 3, 4, 5, 6 };
        return CurrentPlayer.AllCheckersInHome(homeBoardIndices);
    }

    public void BearOffChecker(int fromIndex)
    {
        Point point = Board.Points[fromIndex - 1];
        if (point.Owner == CurrentPlayer.Color && CanBearOff())
        {
            Checker checker = point.RemoveChecker();
            CurrentPlayer.BearOffChecker(checker);
        }
    }

    public void CheckForWinner()
    {
        if (CurrentPlayer.BearOff.Count == 15)
        {
            IsGameOver = true;
            Console.WriteLine($"{CurrentPlayer.Color} player has won the game!");
        }
    }

    public void EndTurn()
    {
        if (!IsGameOver)
        {
            CheckForWinner();
            if (!IsGameOver)
            {
                SwitchTurn();
                RollDice();
            }
        }
    }

    private void SwitchTurn()
    {
        currentPlayerIndex = (currentPlayerIndex == 0) ? 1 : 0;
        CurrentPlayer = Players[currentPlayerIndex];
    }
}
