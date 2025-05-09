namespace Core;

public class Game
{
    public Player[] Players { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public Dice Dice { get; private set; }
    public GameBoard Board { get; private set; }
    public int depth;
    public bool IsGameOver { get; private set; }
    private int currentPlayerIndex;
    
    private AI.AI _blackAI; // Add an AI instance
    private AI.AI _whiteAI; // Add an AI instance

    public Game(string whiteAIConfig = "", string blackAIConfig = "")
    {
        Players =
        [
            new Player(CheckerColor.White),
            new Player(CheckerColor.Black)
        ];

        Board = new GameBoard();
        Dice = new Dice();
        currentPlayerIndex = new Random().Next(0, 2);
        CurrentPlayer = Players[currentPlayerIndex];
        IsGameOver = false;
        depth = 0;
        
        InitializeCheckers();  // Call the method to initialize checkers
        _whiteAI = new AI.AI(Players[0], Board, Players, whiteAIConfig);
        _blackAI = new AI.AI(Players[1], Board, Players, blackAIConfig);
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
            var checker = new Checker(player.Color, pointIndex);
            player.AddChecker(checker); 
            Board.Points[pointIndex - 1].AddChecker(checker);
        }
    }
    public void RollDice()
    {
        Dice.Roll();
    }
    
    public List<(int From, int To)> GetBestMoveForCurrentPlayer()
    {
        int[] diceValues = Dice.GetDiceValues();
        
        AI.AI currentAI = (CurrentPlayer.Color == CheckerColor.White) ? _whiteAI : _blackAI;

        // Call AI to get the best move sequence
        return currentAI.GetBestMove(diceValues, depth);
    }
    
    public void ExecuteAIMoves(List<(int From, int To)> moves)
    {
        foreach (var move in moves)
        {
            int fromIndex = move.From;
            int toIndex = move.To;

            if (fromIndex == 0) // Move from the bar
            {
                foreach (int diceValue in Dice.GetDiceValues())
                {
                    if (IsValidBarMove(toIndex, diceValue))
                    {
                        MakeMove(fromIndex, toIndex);
                        break;
                    }
                }
            }
            else if (toIndex == -1) // Bear off
            {
                if (CanBearOffFromIndex(fromIndex))
                {
                    BearOffChecker(fromIndex);
                }
            }
            else // Regular move
            {
                foreach (int diceValue in Dice.GetDiceValues())
                {
                    if (IsMoveValid(fromIndex, toIndex, diceValue))
                    {
                        MakeMove(fromIndex, toIndex);
                        break;
                    }
                }
            }
        }
    }


    public bool IsMoveValid(int fromIndex, int toIndex, int diceValue)
    {
        return GameUtils.IsMoveValid(fromIndex, toIndex, diceValue, CurrentPlayer, Board);
    }
    
    public bool HasAvailableMoves()
    {
        int[] diceValues = Dice.GetDiceValues();

        // Check moves from the bar
        if (Board.GetBar(CurrentPlayer.Color).Count > 0)
        {
            foreach (int diceValue in diceValues)
            {
                for (int toIndex = 1; toIndex <= 6; toIndex++)
                {
                    if (CurrentPlayer.Color == CheckerColor.White && IsValidBarMove(toIndex, diceValue))
                    {
                        return true;
                    }
                }

                for (int toIndex = 19; toIndex <= 24; toIndex++)
                {
                    if (CurrentPlayer.Color == CheckerColor.Black && IsValidBarMove(toIndex, diceValue))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Check if the player can bear off
        if (CanBearOff())
        {
            foreach (var point in Board.Points)
            {
                if (point.Owner == CurrentPlayer.Color)
                {
                    if (CanBearOffFromIndex(point.Index)) return true;
                }
            }
        }

        // Check regular moves
        foreach (var point in Board.Points)
        {
            if (point.Owner == CurrentPlayer.Color)
            {
                int startIndex = point.Index;

                foreach (int diceValue in diceValues)
                {
                    int forwardIndex = startIndex + diceValue;
                    int backwardIndex = startIndex - diceValue;

                    if (IsMoveValid(startIndex, forwardIndex, diceValue) ||
                        IsMoveValid(startIndex, backwardIndex, diceValue))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

        public int[] GetAvailableEndPoints(int startIndex)
        {
            var availableEndPoints = new List<int>();

            if (startIndex == 0) // Bar index
            {
                int[] diceValues = Dice.GetDiceValues();

                foreach (int diceValue in diceValues)
                {
                    if (CurrentPlayer.Color == CheckerColor.White)
                    {
                        for (int toIndex = 1; toIndex <= 6; toIndex++)
                        {
                            if (IsValidBarMove(toIndex, diceValue))
                            {
                                availableEndPoints.Add(toIndex);
                            }
                        }
                    }
                    else if (CurrentPlayer.Color == CheckerColor.Black)
                    {
                        for (int toIndex = 19; toIndex <= 24; toIndex++)
                        {
                            if (IsValidBarMove(toIndex, diceValue))
                            {
                                availableEndPoints.Add(toIndex);
                            }
                        }
                    }
                }
            }
            else
            {
                var startPoint = Board.Points[startIndex - 1];
                if (startPoint.Owner != CurrentPlayer.Color)
                {
                    return availableEndPoints.ToArray();
                }

                int[] diceValues = Dice.GetDiceValues();
                foreach (int diceValue in diceValues)
                {
                    int forwardIndex = startIndex + diceValue;
                    int backwardIndex = startIndex - diceValue;

                    if (IsMoveValid(startIndex, forwardIndex, diceValue))
                    {
                        availableEndPoints.Add(forwardIndex);
                    }

                    if (IsMoveValid(startIndex, backwardIndex, diceValue))
                    {
                        availableEndPoints.Add(backwardIndex);
                    }
                }
            }

            return availableEndPoints.ToArray();
        }

        public bool MakeMove(int fromIndex, int toIndex)
        {
            int[] diceValues = Dice.GetDiceValues();

            if (fromIndex == 0) // Moving from the bar
            {
                foreach (int diceValue in diceValues)
                {
                    if (IsValidBarMove(toIndex, diceValue))
                    {
                        Point toPoint = Board.Points[toIndex - 1];
                        Checker checker = Board.RemoveFromBar(CurrentPlayer.Color);
                        
                        if (toPoint.IsBlot(Players[currentPlayerIndex].Color))
                        {
                            Checker hitChecker = toPoint.RemoveChecker();
                            hitChecker.Position = 0; // Indicate that the checker is on the bar
                            
                            Board.AddToBar(hitChecker);
                        }
                        Board.Points[toIndex - 1].AddChecker(checker);
                        checker.Position = toIndex;
                        Dice.UseDie(diceValue);
                        return true;
                    }
                }
            }
            else
            {
                foreach (int value in diceValues)
                {
                    if (IsMoveValid(fromIndex, toIndex, value))
                    {
                        Board.MoveChecker(CurrentPlayer, GetOpponent(), fromIndex, toIndex);
                        Dice.UseDie(value);
                        return true;
                    }
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
        // Check if the player has checkers on the bar and restrict movement to entering in the opponent's home area
        if (CurrentPlayer.Color == CheckerColor.White)
        {
            if (toIndex < 1 || toIndex > 6)
            {
                return false;
            }
            
            if (toIndex != diceValue)
            {
                return false;
            }
        }
        else if (CurrentPlayer.Color == CheckerColor.Black)
        {
            // Black re-enters only in opponent's home board (points 19-24)
            if (toIndex < 19 || toIndex > 24)
            {
                return false;
            }
            
            if (toIndex != 25 - diceValue)
            {
                return false;
            }
        }

        // Ensure the point is either empty or has only one opponent checker (blot)
        Point toPoint = Board.Points[toIndex - 1];
        return toPoint.Owner != GetOpponent().Color || toPoint.IsBlot(CurrentPlayer.Color);
    }

    public bool CanBearOff()
    {
        int[] homeBoardIndices = (CurrentPlayer.Color == CheckerColor.White) ? new int[] { 19, 20, 21, 22, 23, 24 } : new int[] { 1, 2, 3, 4, 5, 6 };
        int[] vals = Dice.GetDiceValues();
        foreach (int val in vals)
        {
            if (val != 0) return CurrentPlayer.AllCheckersInHome(homeBoardIndices);
        }
        return false;
    }
    
    public bool CanBearOffFromIndex(int srcIndex)
    {
        // Define home board indices and offset for white
        int[] homeBoardIndices = (CurrentPlayer.Color == CheckerColor.White) 
            ? [19, 20, 21, 22, 23, 24]
            : [1, 2, 3, 4, 5, 6];

        // Validate source index is within the home board
        if (!homeBoardIndices.Contains(srcIndex))
        {
            return false;
        }

        if (Board.Points[srcIndex - 1].Checkers.Count == 0)
        {
            return false;
        }
        
        int[] diceValues = Dice.GetDiceValues();
        
        foreach (int diceValue in diceValues)
        {
            if (CurrentPlayer.Color == CheckerColor.White && srcIndex == 24 - diceValue + 1)
            {
                return true;
            }
            if (CurrentPlayer.Color == CheckerColor.Black && srcIndex == diceValue)
            {
                return true;
            }
        }

        // Check if bearing off is possible from a lower index
        foreach (int diceValue in diceValues)
        {
            int targetIndex = (CurrentPlayer.Color == CheckerColor.White) 
                ? 25 - diceValue 
                : diceValue;

            // Ensure no checkers exist on higher indices
            foreach (var point in Board.Points)
            {
                if ((CurrentPlayer.Color == CheckerColor.White && point.Index < srcIndex) ||
                    (CurrentPlayer.Color == CheckerColor.Black && point.Index > srcIndex))
                {
                    if (point.Owner == CurrentPlayer.Color && point.Checkers.Count > 0)
                    {
                        return false;
                    }
                }
            }

            // Allow bearing off from a lower index
            if (CurrentPlayer.Color == CheckerColor.Black && srcIndex < targetIndex && homeBoardIndices.Contains(srcIndex))
            {
                return true;
            }
            if (CurrentPlayer.Color == CheckerColor.White && srcIndex > targetIndex && homeBoardIndices.Contains(srcIndex))
            {
                return true;
            }
        }

        return false;
    }


    public bool BearOffChecker(int fromIndex)
    {
        Point point = Board.Points[fromIndex - 1];
        if (point.Owner == CurrentPlayer.Color && CanBearOffFromIndex(fromIndex))
        {
            Checker checker = point.RemoveChecker();
            CurrentPlayer.BearOffChecker(checker);

            int[] diceValues = Dice.GetDiceValues();
            int dieToUse;

            if (CurrentPlayer.Color == CheckerColor.White)
            {
                // Calculate target dice value for direct match
                int targetDie = 25 - fromIndex;
                dieToUse = diceValues.FirstOrDefault(d => d == targetDie);

                if (dieToUse == 0) // No direct match, find the lowest die higher than the required length
                {
                    dieToUse = diceValues.Where(d => d > targetDie).OrderBy(d => d).FirstOrDefault();
                }
            }
            else
            {
                // Calculate target dice value for direct match
                int targetDie = fromIndex;
                dieToUse = diceValues.FirstOrDefault(d => d == targetDie);

                if (dieToUse == 0) // No direct match, find the lowest die higher than the required length
                {
                    dieToUse = diceValues.Where(d => d > targetDie).OrderBy(d => d).FirstOrDefault();
                }
            }

            if (dieToUse > 0)
            {
                Dice.UseDie(dieToUse);
                return true;
            }
        }
        return false;
    }


    public void CheckForWinner()
    {
        // Check if all checkers of the current player are off the board
        bool allCheckersOffBoard = true;

        // Verify no checkers exist on the board
        foreach (var point in Board.Points)
        {
            if (point.Owner == CurrentPlayer.Color)
            {
                allCheckersOffBoard = false;
                break;
            }
        }

        // Verify no checkers exist on the bar
        if (allCheckersOffBoard)
        {
            var playerBar = CurrentPlayer.Color == CheckerColor.White ? Board.WhiteBar : Board.BlackBar;
            if (playerBar.Count > 0)
            {
                allCheckersOffBoard = false;
            }
        }

        // Declare winner if no checkers are on the board or bar
        if (allCheckersOffBoard)
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
                Dice.Value1 = 0;
                Dice.Value2 = 0;
                SwitchTurn();
            }
        }
    }

    public void SwitchTurn()
    {
        currentPlayerIndex = (currentPlayerIndex == 0) ? 1 : 0;
        CurrentPlayer = Players[currentPlayerIndex];
    }
}
