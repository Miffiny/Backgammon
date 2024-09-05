using Core;
namespace Backgammon.Tests;

public class CheckerTests
{
    [Fact]
    public void CheckerInitialization_ShouldSetColorAndPosition()
    {
        // Arrange
        string expectedColor = "Red";
        int expectedPosition = 5;

        // Act
        Checker checker = new Checker(expectedColor, expectedPosition);

        // Assert
        Assert.Equal(expectedColor, checker.Color);
        Assert.Equal(expectedPosition, checker.Position);
    }

    [Fact]
    public void CheckerPosition_ShouldBeSettable()
    {
        // Arrange
        Checker checker = new Checker("Red", 5);
        int newPosition = 10;

        // Act
        checker.Position = newPosition;

        // Assert
        Assert.Equal(newPosition, checker.Position);
    }
}