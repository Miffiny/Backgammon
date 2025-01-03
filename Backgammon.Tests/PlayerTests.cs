using System.Linq;
using Core;
using Xunit;

namespace Backgammon.Tests;
public class PlayerTests
{
    

    [Fact]
    public void AllCheckersInHome_ShouldReturnTrueWhenAllCheckersAreInHomeBoard()
    {
        // Arrange
        Player player = new Player(CheckerColor.White);
        player.AddChecker(new Checker(CheckerColor.White, 22));
        player.AddChecker(new Checker(CheckerColor.White, 23));
        player.AddChecker(new Checker(CheckerColor.White, 24));
        int[] homeBoardIndices = { 19, 20, 21, 22, 23, 24 };

        // Act
        bool allInHome = player.AllCheckersInHome(homeBoardIndices);

        // Assert
        Assert.True(allInHome);
    }

    [Fact]
    public void AllCheckersInHome_ShouldReturnFalseWhenAnyCheckerIsNotInHomeBoard()
    {
        // Arrange
        Player player = new Player(CheckerColor.White);
        player.AddChecker(new Checker(CheckerColor.White, 18));  // Outside home board
        player.AddChecker(new Checker(CheckerColor.White, 23));
        int[] homeBoardIndices = { 19, 20, 21, 22, 23, 24 };

        // Act
        bool allInHome = player.AllCheckersInHome(homeBoardIndices);

        // Assert
        Assert.False(allInHome);
    }
}
