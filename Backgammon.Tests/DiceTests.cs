using Xunit;

namespace Backgammon.Tests;

using Core;

public class DiceTests
{
    [Fact]
    public void DiceInitialization_ShouldRollValuesBetween1And6()
    {
        // Arrange
        Dice dice = new Dice();

        // Act
        int value1 = dice.Value1;
        int value2 = dice.Value2;

        // Assert
        Assert.InRange(value1, 1, 6);
        Assert.InRange(value2, 1, 6);
    }

    [Fact]
    public void DiceRoll_ShouldGenerateValidValuesAndDetectDoubles()
    {
        // Arrange
        Dice dice = new Dice();

        // Act
        dice.Roll();

        // Assert
        Assert.InRange(dice.Value1, 1, 6);
        Assert.InRange(dice.Value2, 1, 6);
        if (dice.Value1 == dice.Value2)
        {
            Assert.True(dice.IsDouble);
        }
        else
        {
            Assert.False(dice.IsDouble);
        }
    }

    [Fact]
    public void GetDiceValues_ShouldReturnFourValuesWhenDoubles()
    {
        // Arrange
        Dice dice = new Dice();
        dice.Roll();
        if (!dice.IsDouble)
        {
            // Force a double for testing
            dice = new Dice();
            dice.Roll();
            while (!dice.IsDouble)
            {
                dice.Roll();
            }
        }

        // Act
        int[] values = dice.GetDiceValues();

        // Assert
        Assert.Equal(4, values.Length);
        Assert.All(values, v => Assert.Equal(dice.Value1, v));
    }

    [Fact]
    public void GetDiceValues_ShouldReturnTwoValuesWhenNotDouble()
    {
        // Arrange
        Dice dice = new Dice();
        dice.Roll();
        if (dice.IsDouble)
        {
            // Force a non-double roll for testing
            dice = new Dice();
            dice.Roll();
            while (dice.IsDouble)
            {
                dice.Roll();
            }
        }

        // Act
        int[] values = dice.GetDiceValues();

        // Assert
        Assert.Equal(2, values.Length);
        Assert.Contains(dice.Value1, values);
        Assert.Contains(dice.Value2, values);
    }

    [Fact]
    public void UseDie_ShouldSetUsedDieValueToZero()
    {
        // Arrange
        Dice dice = new Dice();
        dice.Roll();

        // Act
        int dieValue = dice.Value1;
        dice.UseDie(dieValue);

        // Assert
        Assert.Equal(0, dice.Value1);  // The first die should be set to 0 after usage
    }

    [Fact]
    public void UseDie_ShouldNotAffectTheOtherDie()
    {
        // Arrange
        Dice dice = new Dice();
        dice.Roll();

        // Act
        int dieValue = dice.Value1;
        int otherDieValue = dice.Value2;
        dice.UseDie(dieValue);

        // Assert
        Assert.Equal(0, dice.Value1);  // Used die should be zero
        Assert.Equal(otherDieValue, dice.Value2);  // Other die should remain unchanged
    }

    [Fact]
    public void HasMovesLeft_ShouldReturnTrueIfAtLeastOneDieHasValue()
    {
        // Arrange
        Dice dice = new Dice();
        dice.Roll();

        // Act
        bool hasMovesLeft = dice.HasMovesLeft();

        // Assert
        Assert.True(hasMovesLeft);  // At least one of the dice should have a value > 0
    }

    [Fact]
    public void HasMovesLeft_ShouldReturnFalseIfBothDiceAreZero()
    {
        // Arrange
        Dice dice = new Dice();
        dice.Roll();

        // Act
        dice.UseDie(dice.Value1);  // Use first die
        dice.UseDie(dice.Value2);  // Use second die
        bool hasMovesLeft = dice.HasMovesLeft();

        // Assert
        Assert.False(hasMovesLeft);  // Both dice should be 0
    }
}
