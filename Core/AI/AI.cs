using System.Collections.Concurrent;

namespace Core.AI;

public class AI
{
    private readonly Player _aiPlayer;
    private readonly GameBoard _board;
    private readonly Player[] _players;
    private ConcurrentDictionary<string, List<(int From, int To)>> stateToMoves;
    public int Alpha = Int32.MinValue;
    public int Beta = Int32.MaxValue;
    
    // List of evaluation delegates
    private readonly List<Func<Player, GameBoard, int, int>> evaluationFactors;

    public AI(Player aiPlayer, GameBoard board, Player[] players, string factorConfig)
    {
        _aiPlayer = aiPlayer;
        _board = board;
        _players = players;
        stateToMoves = new ConcurrentDictionary<string, List<(int From, int To)>>();

        evaluationFactors = new List<Func<Player, GameBoard, int, int>>();
        
        foreach (char c in factorConfig)
        {
            switch (c)
            {
                case '0':
                    evaluationFactors.Add(BlotFactor);
                    break;
                case '1':
                    evaluationFactors.Add(PrimeFactor);
                    break;
                case '2':
                    evaluationFactors.Add(HomeCheckersFactor);
                    break;
                case '3':
                    evaluationFactors.Add(CheckerStackPenalty);
                    break;
                case '4':
                    evaluationFactors.Add(DeadCheckerPenalty);
                    break;
            }
        }
    }

    public List<(int From, int To)> GetBestMove(int[] diceValues, int depth)
    {
        // Use _board and _aiPlayer directly to generate unique resulting board states.
        var uniqueStates = GenerateUniqueStates(_board, _aiPlayer, diceValues);
    
        // Determine the baseline opponent bar count from the current board state.
        int baseOpponentBarCount = _board.GetBar(GetOpponent(_aiPlayer).Color).Count;
        
        
    
        // Sort uniqueStates so that states with a higher increase in the opponent's bar count are prioritized.
        uniqueStates = uniqueStates
            .OrderByDescending(state => state.GetBar(GetOpponent(_aiPlayer).Color).Count - baseOpponentBarCount)
            .ToList();

        int bestScore = int.MinValue;
        List<(int From, int To)> bestMoveSequence = new List<(int From, int To)>();
    
        int moveNumber = 1;
        foreach (var state in uniqueStates)
        {
            Console.WriteLine($"Check move {moveNumber} out of {uniqueStates.Count}");
            Console.WriteLine(stateToMoves.Count);
            moveNumber++;
            int score = ExpectiMiniMax(state, depth, false, GetOpponent(_aiPlayer));
            if (score > bestScore)
            {
                Console.WriteLine($"Found better option with score {score}, current count of states is");
                Console.WriteLine(stateToMoves.Count);
                bestScore = score;
                bestMoveSequence = RetrieveMoveSequence(state);
            }
        }
        Console.WriteLine("Total count of unique end states");
        Console.WriteLine(stateToMoves.Count);
        stateToMoves.Clear();
        Alpha = Int32.MinValue;
        Beta = Int32.MaxValue;
        return bestMoveSequence;
    }

    private int ExpectiMiniMax(GameBoard board, int depth, bool maximizingPlayer, Player currentPlayer)
    {
        if (depth == 0)
        {
            return EvaluateBoard(currentPlayer, board);
        }

        var diceOutcomes = GenerateAllDiceOutcomes();
        var tasks = new List<Task<(int value, int frequency)>>();

        foreach (var diceOutcome in diceOutcomes)
        {
            int frequency = (diceOutcome.Length == 2 && diceOutcome[0] != diceOutcome[1]) ? 2 : 1;

            tasks.Add(Task.Run(() =>
            {
                int outcomeValue = MinimaxForDiceOutcome(board, depth, maximizingPlayer, currentPlayer, diceOutcome);
                return (outcomeValue, frequency);
            }));
        }

        Task.WaitAll(tasks.ToArray());

        int totalValue = 0;
        int totalFrequency = 0;

        foreach (var task in tasks)
        {
            var (value, frequency) = task.Result;
            totalValue += frequency * value;
            totalFrequency += frequency;
        }

        return totalFrequency == 0
            ? (maximizingPlayer ? int.MinValue : int.MaxValue)
            : totalValue / totalFrequency;
    }

    private int MinimaxForDiceOutcome(GameBoard board, int depth, bool maximizingPlayer, Player currentPlayer, int[] diceOutcome)
    {
        var possibleStates = GenerateUniqueStates(board, currentPlayer, diceOutcome);

        if (possibleStates.Count == 0)
        {
            // Apply a moderate penalty since the player skips turn
            int penalty = EvaluateBoard(currentPlayer, board);
            penalty = maximizingPlayer ? penalty - 10 : penalty + 10; //avg is 8.166 but opponent may hit you and worsen position
            return penalty;
        }

        if (maximizingPlayer)
        {
            int value = int.MinValue;
            // Generate all deterministic moves for the given dice outcome.
            foreach (var state in possibleStates)
            {
                int childValue = ExpectiMiniMax(state, depth - 1, false, GetOpponent(currentPlayer));
                value = Math.Max(value, childValue);
                if (value >= Beta) // β cutoff: if value exceeds Beta, prune.
                {
                    return value;
                }
                Alpha = Math.Max(Alpha, value);
            }
            return value;
        }
        else
        {
            int value = int.MaxValue;
            foreach (var state in possibleStates)
            {
                int childValue = ExpectiMiniMax(state, depth - 1, true, GetOpponent(currentPlayer));
                value = Math.Min(value, childValue);
                if (value <= Alpha) // α cutoff: if value is less than Alpha, prune.
                {
                    return value;
                }
                Beta = Math.Min(Beta, value);
            }
            return value;
        }
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
        int whiteDistance = board.WhiteBar.Count * 25;
        int blackDistance = board.BlackBar.Count * 25;

        // Iterate directly over board points
        var points = board.Points;
        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            if (point.Owner == CheckerColor.White)
                whiteDistance += (25 - (i + 1)) * point.Checkers.Count;
            else if (point.Owner == CheckerColor.Black)
                blackDistance += (i + 1) * point.Checkers.Count;
        }

        // Apply evaluation factors independently to each player's score
        Player opponent = GetOpponent(currentPlayer);
        foreach (var factor in evaluationFactors)
        {
            if (currentPlayer.Color == CheckerColor.White)
            {
                whiteDistance += factor(currentPlayer, board, whiteDistance);
                blackDistance += factor(opponent, board, blackDistance);
            }
            else
            {
                whiteDistance += factor(opponent, board, whiteDistance);
                blackDistance += factor(currentPlayer, board, blackDistance);
            }
        }

        // Return evaluation from current player's perspective
        return _aiPlayer.Color == CheckerColor.White
            ? blackDistance - whiteDistance
            : whiteDistance - blackDistance;
    }
    
    private int BlotFactor(Player player, GameBoard board, int currentScore)
    {
        int blotCount = 0;
        var points = board.Points;

        // Efficiently count blots without LINQ or extra allocations
        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            if (point.Checkers.Count == 1 && point.Checkers[0].Color == player.Color)
                blotCount++;
        }

        // Compute penalty factor (e.g., 1 blot => factor 0.9, 2 blots => factor 0.8)
        double factor = 1.0 + blotCount * 0.02;

        // Apply factor multiplicatively to current score
        return (int)(currentScore * factor);
    }
    
    private int PrimeFactor(Player player, GameBoard board, int currentScore)
    {
        var points = board.Points;
        int consecutiveBlocks = 0;
        int maxConsecutiveBlocks = 0;

        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            if (point.Owner == player.Color && point.Checkers.Count >= 2)
            {
                consecutiveBlocks++;
                // Keep track of longest consecutive blocks
                if (consecutiveBlocks > maxConsecutiveBlocks)
                    maxConsecutiveBlocks = consecutiveBlocks;
            }
            else
            {
                consecutiveBlocks = 0;
            }
        }

        double factor = 1.0;
        // Apply positive factor if there are at least 2 consecutive blocks
        if (maxConsecutiveBlocks >= 2)
        {
            factor = 1.0 - (0.05 * (maxConsecutiveBlocks - 1));
        }
        return (int)(currentScore * factor);
    }
    
    private int HomeCheckersFactor(Player player, GameBoard board, int currentScore)
    {
        int homeCheckerCount = 0;
        int start = player.Color == CheckerColor.White ? 18 : 0; // points 19-24 (White), 1-6 (Black)

        var points = board.Points;

        // Efficiently iterate only over home board points
        for (int i = start; i < start + 6; i++)
        {
            var point = points[i];
            if (point.Owner == player.Color)
                homeCheckerCount += point.Checkers.Count;
        }

        double factor = 1.00 - homeCheckerCount * 0.01;

        return (int)(currentScore * factor);
    }
    
    private int CheckerStackPenalty(Player player, GameBoard board, int currentScore)
    {
        var points = board.Points;
        int penaltyCount = 0;

        // Count the number of checkers exceeding stack size of 5 per point
        for (int i = 0; i < points.Length; i++)
        {
            var point = points[i];
            if (point.Owner == player.Color && point.Checkers.Count > 5)
            {
                penaltyCount += (point.Checkers.Count - 5);
            }
        }

        // Apply penalty factor: 0.02 per extra checker
        double factor = 1.0 + penaltyCount * 0.02;

        return (int)(currentScore * factor);
    }
    
    private int DeadCheckerPenalty(Player player, GameBoard board, int currentScore)
    {
        var points = board.Points;
        int penaltyCount = 0;
        
        if (player.Color == CheckerColor.Black)
        {
            for (int i = 0; i < 2; i++)
            {
                var point = points[i];
                if (point.Checkers.Count > 3) penaltyCount += point.Checkers.Count - 3;
            }
        }
        
        if (player.Color == CheckerColor.White)
        {
            for (int i = 22; i < 24; i++)
            {
                var point = points[i];
                if (point.Checkers.Count > 3) penaltyCount += point.Checkers.Count - 3;
            }
        }

        // Apply penalty factor: 0.03 per extra checker
        double factor = 1.0 + penaltyCount * 0.03;

        return (int)(currentScore * factor);
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
                    stateToMoves.TryAdd(stateHash, newMoves);
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
                stateToMoves.TryRemove(stateHash, out _);
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