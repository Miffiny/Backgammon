using System.Linq;
using Core;
using Xunit;

namespace Backgammon.Tests;
public class PlayerTests
{
    [Fact]
    public void PlayerInitialization_ShouldSetNameAndColorAndInitializeLists()
    {
        // Arrange
        CheckerColor expectedColor = CheckerColor.White;

        // Act
        Player player = new Player(expectedColor);

        // Assert
        Assert.Equal(expectedColor, player.Color);
        Assert.Empty(player.Checkers);
        Assert.Empty(player.Bar);
        Assert.Empty(player.BearOff);
    }

    [Fact]
    public void AddChecker_ShouldAddCheckerToCheckersList()
    {
        // Arrange
        Player player = new Player(CheckerColor.White);
        Checker checker = new Checker(CheckerColor.White, 1);

        // Act
        player.AddChecker(checker);

        // Assert
        Assert.Single(player.Checkers);
        Assert.Equal(checker, player.Checkers.First());
    }

    [Fact]
    public void HitChecker_ShouldMoveCheckerFromCheckersToBar()
    {
        // Arrange
        Player player = new Player(CheckerColor.White);
        Checker checker = new Checker(CheckerColor.White, 1);
        player.AddChecker(checker);

        // Act
        player.HitChecker(checker);

        // Assert
        Assert.Empty(player.Checkers);
        Assert.Single(player.Bar);
        Assert.Equal(checker, player.Bar.First());
    }

    [Fact]
    public void ReEnterFromBar_ShouldMoveCheckerFromBarToBoard()
    {
        // Arrange
        Player player = new Player(CheckerColor.White);
        Checker checker = new Checker(CheckerColor.White, 1);
        player.HitChecker(checker);  // Puts checker on the bar

        // Act
        Checker reEnteredChecker = player.ReEnterFromBar();

        // Assert
        Assert.Empty(player.Bar);
        Assert.Equal(checker, reEnteredChecker);
    }

    [Fact]
    public void ReEnterFromBar_WhenBarIsEmpty_ShouldReturnNull()
    {
        // Arrange
        Player player = new Player(CheckerColor.White);

        // Act
        Checker reEnteredChecker = player.ReEnterFromBar();

        // Assert
        Assert.Null(reEnteredChecker);
    }

    [Fact]
    public void BearOffChecker_ShouldMoveCheckerFromCheckersToBearOff()
    {
        // Arrange
        Player player = new Player(CheckerColor.White);
        Checker checker = new Checker(CheckerColor.White, 1);
        player.AddChecker(checker);

        // Act
        player.BearOffChecker(checker);

        // Assert
        Assert.Empty(player.Checkers);
        Assert.Single(player.BearOff);
        Assert.Equal(checker, player.BearOff.First());
    }

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
