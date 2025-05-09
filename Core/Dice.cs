namespace Core;

public class Dice
{
    public int Value1 { get;  set; }
    public int Value2 { get;  set; }
    public bool IsDouble { get; private set; }
    private int doubleUsageCount;

    private Random random;

    public Dice()
    {
        random = new Random();
    }

    // Method to roll the dice
    public void Roll()
    {
        Value1 = random.Next(1, 7); 
        Value2 = random.Next(1, 7); 
        IsDouble = (Value1 == Value2); 

        doubleUsageCount = 0;
    }

    // Method to get all the dice values (for doubles, returns four of the same value)
    public int[] GetDiceValues()
    {
        if (IsDouble)
        {
            return new[] { Value1, Value1, Value1, Value1 }.Skip(doubleUsageCount).ToArray();
        }
        return [Value1, Value2];
    }

    // Method to use a die value when a move is made
    public void UseDie(int value)
    {
        if (IsDouble && value == Value1)
        {
            doubleUsageCount++;
            if (doubleUsageCount >= 4)
            {
                Value1 = 0;
                Value2 = 0;
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