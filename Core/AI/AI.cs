namespace Core.AI;

public class AI
{
    private readonly Player _aiPlayer;
    private readonly GameBoard _board;
    private readonly Player[] _players;
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
            // Use ExpectMiniMax to evaluate each state
            int score = ExpectMiniMax(state, depth, false, _aiPlayer);
            if (score > bestScore)
            {
                Console.WriteLine("Found better option, current count of states is");
                Console.WriteLine(stateToMoves.Count);
                bestScore = score;
                bestMoveSequence = RetrieveMoveSequence(state);
            }
        }
        Console.WriteLine("Total count of unique end states");
        Console.WriteLine(stateToMoves.Count);
        stateToMoves.Clear();
        return bestMoveSequence;
    }

    private int ExpectMiniMax(GameBoard board, int depth, bool maximizingPlayer, Player currentPlayer)
    {
        // Base case: if max depth reached, evaluate the board
        if (depth == 0)
        {
            return EvaluateBoard(currentPlayer, board);
        }

        int bestValue = maximizingPlayer ? int.MinValue : int.MaxValue;
        var possibleDiceOutcomes = GenerateAllDiceOutcomes();

        foreach (var diceOutcome in possibleDiceOutcomes)
        {
            var possibleStates = GenerateUniqueStates(board, currentPlayer, diceOutcome);
            double outcomeWeight = (diceOutcome[0] == diceOutcome[1]) ? 0.5 : 1.0; // Halve doubles

            foreach (var state in possibleStates)
            {
                int value = ExpectMiniMax(state, depth - 1, !maximizingPlayer, GetOpponent(currentPlayer));

                // Apply weight if it's a double
                int weightedValue = (int)(value * outcomeWeight);

                if (maximizingPlayer)
                {
                    bestValue = Math.Max(bestValue, weightedValue);
                }
                else
                {
                    bestValue = Math.Min(bestValue, weightedValue);
                }
            }
        }

        return bestValue;
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
                        if (GameUtils.IsValidBarMove(toIndex, diceValue, currentPlayer, board) && !moves.Contains((0, toIndex)))
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
                            if (!moves.Contains((point.Index, forwardIndex)))
                            {
                                moves.Add((point.Index, forwardIndex));
                            }
                        }
                    }
                    else if (currentPlayer.Color == CheckerColor.Black)
                    {
                        if (GameUtils.IsMoveValid(point.Index, backwardIndex, diceValue, currentPlayer, board))
                        {
                            if (!moves.Contains((point.Index, backwardIndex)))
                            {
                                moves.Add((point.Index, backwardIndex));
                            }
                        }
                    }

                    // Handle bearing off
                    if (GameUtils.CanBearOffFromIndex(point.Index, board, currentPlayer, diceValues))
                    {
                        if (!moves.Contains((point.Index, -1)))
                        {
                            moves.Add((point.Index, -1));
                        }
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
                    uniqueStates.Add(currentBoard);
                    currentCallStates.Add(stateHash); // Track this state
                }
                return;
            }

            foreach (var move in moves)
            {
                var simulatedBoard = SimulateMove(currentBoard, currentPlayer, move.From, move.To);
                var newDiceValues = RemoveUsedDie(remainingDice, move.From, move.To, currentPlayer.Color);

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

    // Utility: Hash the board state for efficient uniqueness checking
    private string HashBoardState(GameBoard board)
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
        hashComponents.Add($"{board.WhiteBar.Count}:WBar");
        hashComponents.Add($"{board.BlackBar.Count}:BBar");
        return string.Join("|", hashComponents);
    }
    
    private GameBoard SimulateMove(GameBoard board, Player currentPlayer, int src, int dst)
    {
        // Clone the board with cloned players
        GameBoard simulatedBoard = board.Clone();

        // Handle re-entering from the bar
        if (src == 0)
        {
            Checker checker = simulatedBoard.RemoveFromBar(currentPlayer.Color);
            Point toPoint = simulatedBoard.Points[dst - 1];
        
            // Check if re-entering checker hits an opponent's blot
            if (toPoint.IsBlot(currentPlayer.Color))
            {
                Checker opponentChecker = toPoint.RemoveChecker();
                opponentChecker.Position = 0;
                simulatedBoard.AddToBar(opponentChecker);
            }
        
            toPoint.AddChecker(checker);
            checker.Position = dst;
        }
        else if (dst == -1)
        {
            Checker checker = simulatedBoard.Points[src - 1].RemoveChecker();
            currentPlayer.BearOffChecker(checker);
        }
        else
        {
            Point fromPoint = simulatedBoard.Points[src - 1];
            Point toPoint = simulatedBoard.Points[dst - 1];

            Checker checker = fromPoint.RemoveChecker();

            if (toPoint.IsBlot(currentPlayer.Color))
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
            for (int j = i; j <= 6; j++) // j >= i to avoid duplicate permutations
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
    
    private int[] RemoveUsedDie(int[] diceValues, int src, int dst, CheckerColor color)
    {
        int diceValue;
        if (dst == -1) // Bearing off
        {
            diceValue = (color == CheckerColor.White) ? (25 - src) : src;
        }
        else if (src == 0) // Re-entering from the bar
        {
            if (color == CheckerColor.White)
            {
                diceValue = dst; // White uses dice values directly matching the destination
            }
            else // Black
            {
                diceValue = 25 - dst; // Black uses 25 - destination to calculate the dice value
            }
        }
        else
        {
            diceValue = Math.Abs(dst - src); // Regular moves use absolute difference
        }

        // Remove the used dice value
        var newDiceValues = new List<int>(diceValues);
        if (newDiceValues.Contains(diceValue))
        {
            newDiceValues.Remove(diceValue);
        }
        else if (dst == -1) // Bearing off but exact dice value not found
        {
            // Find the closest higher dice value
            int closestHigherDie = newDiceValues.Where(d => d > diceValue).OrderBy(d => d).FirstOrDefault();
            if (closestHigherDie > 0)
            {
                newDiceValues.Remove(closestHigherDie);
            }
        }

        return newDiceValues.ToArray();
    }

}