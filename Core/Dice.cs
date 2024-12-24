namespace Core;

public class Dice
{
    public int Value1 { get;  set; }  // The result of the first die
    public int Value2 { get;  set; }  // The result of the second die
    public bool IsDouble { get; private set; }  // Indicates if the roll is a double (both dice have the same value)
    private int doubleUsageCount; // Tracks how many times the double value has been used

    private Random random;

    public Dice()
    {
        random = new Random();
    }

    // Method to roll the dice
    public void Roll()
    {
        Value1 = random.Next(1, 7);  // Rolls a value between 1 and 6 for the first die
        Value2 = random.Next(1, 7);  // Rolls a value between 1 and 6 for the second die
        IsDouble = (Value1 == Value2);  // Checks if both dice rolled the same value

        doubleUsageCount = 0; // Reset the double usage count after each roll
    }

    // Method to get all the dice values (for doubles, return four of the same value)
    public int[] GetDiceValues()
    {
        if (IsDouble)
        {
            return new[] { Value1, Value1, Value1, Value1 }.Skip(doubleUsageCount).ToArray();
        }
        return [Value1, Value2];
    }

    // Method to check if there are any moves left based on the dice
    public bool HasMovesLeft()
    {
        if (IsDouble)
        {
            return doubleUsageCount < 4; // Moves left if the double value hasn't been used four times
        }
        return Value1 > 0 || Value2 > 0;
    }

    // Method to use a die value when a move is made
    public void UseDie(int value)
    {
        if (IsDouble && value == Value1)
        {
            doubleUsageCount++; // Increment the double usage count
            if (doubleUsageCount >= 4)
            {
                Value1 = 0; // Mark the die as fully used
                Value2 = 0; // Synchronize for consistency
            }
        }
        else if (Value1 == value)
        {
            Value1 = 0;
        }
        else if (Value2 == value)
        {
            Value2 = 0;
        }
    }
}