namespace Core;

public static class GameUtils
{
    public static bool IsMoveValid(int fromIndex, int toIndex, int diceValue, Player CurrentPlayer, GameBoard Board)
    {
        if (Board.GetBar(CurrentPlayer.Color).Count > 0)
        {
            // If checker is on the bar, player can only re-enter in the opponent's home board
            if (fromIndex == 0) return IsValidBarMove(toIndex, diceValue, CurrentPlayer, Board);
            return false;
        }
        
        // Enforce moving in the correct direction
        if (CurrentPlayer.Color == CheckerColor.White && toIndex <= fromIndex)
        {
            return false;  // White moves forward (to higher indices)
        }
        if (CurrentPlayer.Color == CheckerColor.Black && toIndex >= fromIndex)
        {
            return false;  // Black moves backward (to lower indices)
        }
        
        return Board.IsMoveValid(CurrentPlayer, fromIndex, toIndex, diceValue);
    }
    
    public static bool IsValidBarMove(int toIndex, int diceValue, Player CurrentPlayer, GameBoard Board)
    {
        // Check if the player has checkers on the bar and restrict movement to entering in the opponent's home area
        if (CurrentPlayer.Color == CheckerColor.White)
        {
            // White re-enters only in opponent's home board (points 1-6)
            if (toIndex < 1 || toIndex > 6)
            {
                return false;
            }

            // Ensure the destination matches the dice value for white
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

            // Ensure the destination matches the dice value for black
            if (toIndex != 25 - diceValue)
            {
                return false;
            }
        }

        // Ensure the point is either empty or has only one opponent checker (blot)
        Point toPoint = Board.Points[toIndex - 1];
        return toPoint.Owner == CurrentPlayer.Color || toPoint.Owner == null || toPoint.IsBlot(CurrentPlayer.Color);
    }
    
    public static bool CanBearOffFromIndex(int srcIndex, GameBoard Board, Player CurrentPlayer, int[] diceValues)
    {
        if (!CanBearOff(CurrentPlayer, Board, diceValues)) return false;
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

        // Check if the srcIndex matches any dice value for bearing off directly
        foreach (int diceValue in diceValues)
        {
            if (CurrentPlayer.Color == CheckerColor.White && srcIndex == 24 - diceValue + 1) // Adjusted for white's range
            {
                return true;
            }
            if (CurrentPlayer.Color == CheckerColor.Black && srcIndex == diceValue) // Direct match for black
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
                        return false; // Higher index occupied by the current player
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
    
    public static bool CanBearOff(Player CurrentPlayer, GameBoard Board, int[] vals)
    {
        int[] homeBoardIndices = (CurrentPlayer.Color == CheckerColor.White) 
            ? [19, 20, 21, 22, 23, 24]
            : [1, 2, 3, 4, 5, 6];
    
        // Check if all checkers of the current player are within home board indices
        foreach (var point in Board.Points)
        {
            foreach (var checker in point.Checkers)
            {
                if (checker.Color == CurrentPlayer.Color && !homeBoardIndices.Contains(checker.Position))
                {
                    return false; // A checker is outside the home board
                }
            }
        }
    
        foreach (int val in vals)
        {
            if (val != 0) return true;
        }
        return false;
    }

    
    public static int[] GetAvailableEndPoints(int startIndex, Player CurrentPlayer, GameBoard Board, int[] diceValues)
        {
            var availableEndPoints = new List<int>();

            if (startIndex == 0) // Bar index
            {
                foreach (int diceValue in diceValues)
                {
                    if (CurrentPlayer.Color == CheckerColor.White)
                    {
                        for (int toIndex = 1; toIndex <= 6; toIndex++)
                        {
                            if (IsValidBarMove(toIndex, diceValue, CurrentPlayer, Board))
                            {
                                availableEndPoints.Add(toIndex);
                            }
                        }
                    }
                    else if (CurrentPlayer.Color == CheckerColor.Black)
                    {
                        for (int toIndex = 19; toIndex <= 24; toIndex++)
                        {
                            if (IsValidBarMove(toIndex, diceValue, CurrentPlayer, Board))
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
                
                foreach (int diceValue in diceValues)
                {
                    int forwardIndex = startIndex + diceValue;
                    int backwardIndex = startIndex - diceValue;

                    if (IsMoveValid(startIndex, forwardIndex, diceValue, CurrentPlayer, Board))
                    {
                        availableEndPoints.Add(forwardIndex);
                    }

                    if (IsMoveValid(startIndex, backwardIndex, diceValue, CurrentPlayer, Board))
                    {
                        availableEndPoints.Add(backwardIndex);
                    }
                }
            }
            return availableEndPoints.ToArray();
        }
}