namespace Core;

public class Dice
{
    public int Value1 { get;  set; }  // The result of the first die
    public int Value2 { get;  set; }  // The result of the second die
    public bool IsDouble { get; private set; }  // Indicates if the roll is a double (both dice have the same value)

    private Random random;

    public Dice()
    {
        random = new Random();
        Roll();
    }

    // Method to roll the dice
    public void Roll()
    {
        Value1 = random.Next(1, 7);  // Rolls a value between 1 and 6 for the first die
        Value2 = random.Next(1, 7);  // Rolls a value between 1 and 6 for the second die
        IsDouble = (Value1 == Value2);  // Checks if both dice rolled the same value
    }

    // Method to get all the dice values (for doubles, return four of the same value)
    public int[] GetDiceValues()
    {
        if (IsDouble)
        {
            return new int[] { Value1, Value1, Value1, Value1 };
        }
        else
        {
            return new int[] { Value1, Value2 };
        }
    }

    // Method to check if there are any moves left based on the dice
    public bool HasMovesLeft()
    {
        return Value1 > 0 || Value2 > 0;
    }

    // Method to use a die value when a move is made
    public void UseDie(int value)
    {
        if (Value1 == value)
        {
            Value1 = 0;
        }
        else if (Value2 == value)
        {
            Value2 = 0;
        }
    }
}