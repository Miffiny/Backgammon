namespace Core;

public class Game
{
    public Player[] Players { get; private set; }  // Two players in the game
    public Player CurrentPlayer { get; private set; }  // The player whose turn it is
    public Dice Dice { get; private set; }  // The dice object representing the current roll
    public GameBoard Board { get; private set; }  // The game board object
    public bool IsGameOver { get; private set; }  // Indicates if the game has ended

    private int currentPlayerIndex;  // Index of the current player (0 or 1)

    public Game(string player1Name, string player2Name)
    {
        // Initialize the players
        Players = new Player[]
        {
            new Player(player1Name, "White"),
            new Player(player2Name, "Black")
        };

        // Initialize the game board and dice
        Board = new GameBoard();
        Dice = new Dice();
        currentPlayerIndex = 0;
        CurrentPlayer = Players[currentPlayerIndex];

        IsGameOver = false;
    }

    // Method to switch to the next player
    private void SwitchTurn()
    {
        currentPlayerIndex = (currentPlayerIndex == 0) ? 1 : 0;
        CurrentPlayer = Players[currentPlayerIndex];
    }

    // Method to roll the dice and begin a player's turn
    public void RollDice()
    {
        Dice.Roll();
    }

    // Method to check if a move is valid
    public bool IsMoveValid(int fromIndex, int toIndex, int diceValue)
    {
        return Board.IsMoveValid(CurrentPlayer, fromIndex, toIndex, diceValue);
    }

    // Method to make a move if valid
    public bool MakeMove(int fromIndex, int toIndex)
    {
        // Get the valid dice values
        int[] diceValues = Dice.GetDiceValues();

        foreach (int value in diceValues)
        {
            if (IsMoveValid(fromIndex, toIndex, value))
            {
                Board.MoveChecker(CurrentPlayer, fromIndex, toIndex);
                Dice.UseDie(value);
                return true;
            }
        }

        return false;
    }

    // Method to check if the current player can bear off
    public bool CanBearOff()
    {
        int[] homeBoardIndices = (CurrentPlayer.Color == "White") ? new int[] { 19, 20, 21, 22, 23, 24 } : new int[] { 1, 2, 3, 4, 5, 6 };
        return CurrentPlayer.AllCheckersInHome(homeBoardIndices);
    }

    // Method to bear off a checker
    public void BearOffChecker(int fromIndex)
    {
        Point point = Board.Points[fromIndex - 1];
        if (point.Owner == CurrentPlayer.Color && CanBearOff())
        {
            Checker checker = point.RemoveChecker();
            CurrentPlayer.BearOffChecker(checker);
        }
    }

    // Method to check if the game has been won
    public void CheckForWinner()
    {
        if (CurrentPlayer.BearOff.Count == 15)
        {
            IsGameOver = true;
        }
    }

    // Method to end the current player's turn and switch to the next player
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
}