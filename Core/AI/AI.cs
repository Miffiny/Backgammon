namespace Core.AI;

public class AI
{
    private Player _aiPlayer;
    private GameBoard _board;
    private Player[] _players;
    private Dictionary<string, List<(int From, int To)>> stateToMoves;

    public AI(Player aiPlayer, GameBoard board, Player[] players)
    {
        _aiPlayer = aiPlayer;
        _board = board;
        _players = players;
        stateToMoves = new Dictionary<string, List<(int From, int To)>>(); // Initialize here
    }

    public List<(int From, int To)> GetBestMove(int[] diceValues, int depth)
    {
        // Use _board and _aiPlayer directly
        var uniqueStates = GenerateUniqueStates(_board, _aiPlayer, diceValues);
        

        int bestScore = int.MinValue;
        List<(int From, int To)> bestMoveSequence = new List<(int From, int To)>();
        
        foreach (var state in uniqueStates)
        {
            int score = 0;
            // Use ExpectMiniMax to evaluate each state
            score = ExpectMiniMax(state, depth, false, _aiPlayer);
            if (score > bestScore)
            {

                bestScore = score;
                bestMoveSequence = RetrieveMoveSequence(state);
            }
        }

        return bestMoveSequence;
    }

    private int ExpectMiniMax(GameBoard board, int depth, bool maximizingPlayer, Player currentPlayer)
    {
        // Base case: evaluate the board if maximum depth is reached
        if (depth == 0)
        {
            int curScore = EvaluateBoard(currentPlayer, board);
            if (curScore > 400) Console.WriteLine(curScore);
            return EvaluateBoard(currentPlayer, board);
        }

        int bestValue = maximizingPlayer ? int.MinValue : int.MaxValue;
        var possibleDiceOutcomes = GenerateAllDiceOutcomes();
    
        foreach (var diceOutcome in possibleDiceOutcomes)
        {
            var possibleStates = GenerateUniqueStates(board, currentPlayer, diceOutcome);

            foreach (var state in possibleStates)
            {
                int value = ExpectMiniMax(state, depth - 1, !maximizingPlayer, GetOpponent(currentPlayer));

                if (maximizingPlayer)
                {
                    bestValue = Math.Max(bestValue, value); // Maximize for the current player
                }
                else
                {
                    bestValue = Math.Min(bestValue, value); // Minimize for the opponent
                }
            }
        }

        return bestValue; // Return the optimal value for this level
    }
    
    private List<(int From, int To)> RetrieveMoveSequence(GameBoard board)
    {
        // Use the state-to-moves mapping generated in GenerateUniqueStates
        string stateHash = HashBoardState(board);

        if (stateToMoves.TryGetValue(stateHash, out var moveSequence))
        {
            return new List<(int From, int To)>(moveSequence);
        }

        // If the state is not found, return an empty list (should not occur if mapping is accurate)
        return new List<(int From, int To)>();
    }

    
    private int EvaluateBoard(Player currentPlayer, GameBoard board)
    {
        // Calculate the total distance for white checkers
        int whiteDistance = 0;
        foreach (var point in board.Points)
        {
            foreach (var checker in point.Checkers)
            {
                if (checker.Color == CheckerColor.White)
                {
                    whiteDistance += 25 - point.Index; // Distance for white
                }
            }
        }

        // Add checkers on the bar for white
        whiteDistance += board.WhiteBar.Count * 25;

        // Calculate the total distance for black checkers
        int blackDistance = 0;
        foreach (var point in board.Points)
        {
            foreach (var checker in point.Checkers)
            {
                if (checker.Color == CheckerColor.Black)
                {
                    blackDistance += point.Index; // Distance for black
                }
            }
        }

        // Add checkers on the bar for black
        blackDistance += board.BlackBar.Count * 25;
        Console.WriteLine("Black distance: ");
        Console.WriteLine(blackDistance);
        Console.WriteLine("White distance: ");
        Console.WriteLine(whiteDistance);
        // Return the difference based on the current player's perspective
        if (currentPlayer.Color == CheckerColor.White) return whiteDistance - blackDistance; // Maximize for white
        return blackDistance - whiteDistance; // Maximize for black
    }

    
    public List<(int From, int To)> GenerateMoves(GameBoard board, Player currentPlayer, int[] diceValues)
    {
        var moves = new List<(int From, int To)>();

        // If the player has checkers on the bar, handle re-entering first
        if (board.GetBar(currentPlayer.Color).Count > 0)
        {
            foreach (int diceValue in diceValues)
            {
                if (currentPlayer.Color == CheckerColor.White)
                {
                    for (int toIndex = 1; toIndex <= 6; toIndex++)
                    {
                        if (GameUtils.IsValidBarMove(toIndex, diceValue, currentPlayer, board))
                        {
                            moves.Add((0, toIndex));
                        }
                    }
                }
                else if (currentPlayer.Color == CheckerColor.Black)
                {
                    for (int toIndex = 19; toIndex <= 24; toIndex++)
                    {
                        if (GameUtils.IsValidBarMove(toIndex, diceValue, currentPlayer, board))
                        {
                            moves.Add((0, toIndex));
                        }
                    }
                }
            }
            return moves; // No other moves can occur until all checkers are re-entered
        }

        // Generate moves for checkers on the board
        foreach (var point in board.Points)
        {
            if (point.Owner == currentPlayer.Color)
            {
                foreach (int diceValue in diceValues)
                {
                    int forwardIndex = point.Index + diceValue;
                    int backwardIndex = point.Index - diceValue;

                    // Handle forward and backward moves based on player color
                    if (currentPlayer.Color == CheckerColor.White)
                    {
                        if (GameUtils.IsMoveValid(point.Index, forwardIndex, diceValue, currentPlayer, board))
                        {
                            moves.Add((point.Index, forwardIndex));
                        }
                    }
                    else if (currentPlayer.Color == CheckerColor.Black)
                    {
                        if (GameUtils.IsMoveValid(point.Index, backwardIndex, diceValue, currentPlayer, board))
                        {
                            moves.Add((point.Index, backwardIndex));
                        }
                    }

                    // Handle bearing off
                    if (GameUtils.CanBearOffFromIndex(point.Index, board, currentPlayer, diceValues))
                    {
                        moves.Add((point.Index, -1));
                    }
                }
            }
        }
        return moves;
    }
    
    public List<GameBoard> GenerateUniqueStates(GameBoard board, Player currentPlayer, int[] diceValues)
    {
        var uniqueStates = new List<GameBoard>(); // Local to this method
        var currentCallStates = new HashSet<string>(); // Track states added during this call

        void Recurse(GameBoard currentBoard, int[] remainingDice, List<(int From, int To)> currentMoves)
        {
            var moves = GenerateMoves(currentBoard, currentPlayer, remainingDice);
            if (moves.Count == 0)
            {
                string stateHash = HashBoardState(currentBoard);
                if (stateToMoves.ContainsKey(stateHash))
                {
                    stateToMoves[stateHash] = new List<(int From, int To)>(currentMoves);
                    uniqueStates.Add(currentBoard);
                    currentCallStates.Add(stateHash); // Track this state
                }
                return;
            }

            foreach (var move in moves)
            {
                var simulatedBoard = SimulateMove(currentBoard, currentPlayer, move.From, move.To);
                var newDiceValues = RemoveUsedDie(remainingDice, Math.Abs(move.From - move.To));

                var newMoves = new List<(int From, int To)>(currentMoves) { move };
                string stateHash = HashBoardState(simulatedBoard);

                if (!stateToMoves.ContainsKey(stateHash))
                {
                    stateToMoves[stateHash] = newMoves;
                    currentCallStates.Add(stateHash); // Track this state
                    Recurse(simulatedBoard, newDiceValues, newMoves);
                }
            }
        }

        Recurse(board, diceValues, new List<(int From, int To)>());

        // Remove intermediate states added during this call that are not in uniqueStates
        foreach (var stateHash in currentCallStates)
        {
            if (!uniqueStates.Any(state => HashBoardState(state) == stateHash))
            {
                stateToMoves.Remove(stateHash);
            }
        }

        return uniqueStates;
    }


    

    // Utility: Remove the used die from the remaining dice values
    private static int[] RemoveUsedDie(int[] diceValues, int usedValue)
    {
        var newDiceValues = new List<int>(diceValues);
        newDiceValues.Remove(usedValue);
        return newDiceValues.ToArray();
    }

    // Utility: Hash the board state for efficient uniqueness checking
    private static string HashBoardState(GameBoard board)
    {
        var hashComponents = new List<string>();
        foreach (var point in board.Points)
        {
            var checkers = point.Checkers;
            if (checkers.Count > 0)
            {
                hashComponents.Add($"{point.Index}:{checkers.Count}:{checkers[0].Color}");
            }
        }
        return string.Join("|", hashComponents);
    }
    
    private GameBoard SimulateMove(GameBoard board, Player currentPlayer, int src, int dst)
    {
        // Clone players to ensure independence
        Player[] clonedPlayers = _players.Select(p => p.Clone()).ToArray();

        // Clone the board with cloned players
        GameBoard simulatedBoard = board.Clone(clonedPlayers);

        Player clonedCurrentPlayer = clonedPlayers.First(p => p.Color == currentPlayer.Color);
        Player clonedOpponent = clonedPlayers.First(p => p.Color != currentPlayer.Color);

        // Handle re-entering from the bar
        if (src == 0)
        {
            Checker checker = board.RemoveFromBar(currentPlayer.Color);
            simulatedBoard.Points[dst - 1].AddChecker(checker);
            checker.Position = dst;
        }
        else if (dst == -1)
        {
            Checker checker = simulatedBoard.Points[src - 1].RemoveChecker();
            clonedCurrentPlayer.BearOffChecker(checker);
        }
        else
        {
            Point fromPoint = simulatedBoard.Points[src - 1];
            Point toPoint = simulatedBoard.Points[dst - 1];

            Checker checker = fromPoint.RemoveChecker();

            if (toPoint.IsBlot(clonedCurrentPlayer.Color))
            {
                Checker opponentChecker = toPoint.RemoveChecker();
                opponentChecker.Position = 0;
                simulatedBoard.AddToBar(opponentChecker);
            }

            toPoint.AddChecker(checker);
            checker.Position = dst;
        }

        return simulatedBoard;
    }



// Helper method to get the opponent player
    private Player GetOpponent(Player currentPlayer)
    {
        return (currentPlayer.Color == CheckerColor.White)
            ? _players.First(p => p.Color == CheckerColor.Black)
            : _players.First(p => p.Color == CheckerColor.White);
    }
    private List<int[]> GenerateAllDiceOutcomes()
    {
        var outcomes = new List<int[]>();

        for (int i = 1; i <= 6; i++)
        {
            for (int j = i; j <= 6; j++) // Ensure j >= i to avoid duplicate permutations
            {
                if (i == j)
                {
                    // Doubles produce four identical dice values
                    outcomes.Add(new[] { i, i, i, i });
                }
                else
                {
                    // Add a single combination for non-doubles
                    outcomes.Add(new[] { i, j });
                }
            }
        }
        return outcomes;
    }

}


