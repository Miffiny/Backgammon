using Core;
namespace Backgammon.Tests;
public class GameBoardTests
{
    [Fact]
    public void GameBoardInitialization_ShouldCreate24Points()
    {
        // Arrange & Act
        GameBoard gameBoard = new GameBoard();

        // Assert
        Assert.Equal(24, gameBoard.Points.Length);
        for (int i = 0; i < 24; i++)
        {
            Assert.NotNull(gameBoard.Points[i]);
            Assert.Equal(i + 1, gameBoard.Points[i].Index);
            Assert.Empty(gameBoard.Points[i].Checkers);
        }
    }

    [Fact]
    public void MoveChecker_ShouldMoveCheckerFromOnePointToAnother()
    {
        // Arrange
        GameBoard gameBoard = new GameBoard();
        Player player = new Player(CheckerColor.White);
        Checker checker = new Checker(CheckerColor.White, 1);
        gameBoard.Points[0].AddChecker(checker);  // Place checker on point 1
        player.AddChecker(checker);

        // Act
        gameBoard.MoveChecker(player, null, 1, 4);  // Move checker from point 1 to point 4

        // Assert
        Assert.Empty(gameBoard.Points[0].Checkers);  // Point 1 should be empty
        Assert.Single(gameBoard.Points[3].Checkers);  // Point 4 should have 1 checker
        Assert.Equal(4, checker.Position);  // Checker's position should be updated
    }

    [Fact]
    public void MoveChecker_ShouldNotMoveWhenFromIndexIsInvalid()
    {
        // Arrange
        GameBoard gameBoard = new GameBoard();
        Player player = new Player(CheckerColor.White);

        // Act
        gameBoard.MoveChecker(player,  null, 25, 4);  // Invalid fromIndex

        // Assert
        Assert.All(gameBoard.Points, point => Assert.Empty(point.Checkers));  // No checker should be moved
    }

    [Fact]
    public void MoveChecker_ShouldNotMoveWhenToIndexIsInvalid()
    {
        // Arrange
        GameBoard gameBoard = new GameBoard();
        Player player = new Player(CheckerColor.White);
        Checker checker = new Checker(CheckerColor.White, 1);
        gameBoard.Points[0].AddChecker(checker);  // Place checker on point 1
        player.AddChecker(checker);

        // Act
        gameBoard.MoveChecker(player, null, 1, 25);  // Invalid toIndex

        // Assert
        Assert.Single(gameBoard.Points[0].Checkers);  // Point 1 should still have the checker
    }

    [Fact]
    public void MoveChecker_ShouldHitOpponentCheckerIfBlot()
    {
        // Arrange
        GameBoard gameBoard = new GameBoard();
        Player whitePlayer = new Player(CheckerColor.White);
        Player blackPlayer = new Player(CheckerColor.Black);
        Checker whiteChecker = new Checker(CheckerColor.White, 1);
        Checker blackChecker = new Checker(CheckerColor.Black, 4);
        gameBoard.Points[0].AddChecker(whiteChecker);  // Red checker on point 1
        gameBoard.Points[3].AddChecker(blackChecker);  // White checker on point 4
        whitePlayer.AddChecker(whiteChecker);
        blackPlayer.AddChecker(blackChecker);

        // Act
        gameBoard.MoveChecker(whitePlayer, blackPlayer, 1, 4);  // Move red checker to point 4 (blot)

        // Assert
        Assert.Empty(gameBoard.Points[0].Checkers);  // Red checker should be removed from point 1
        Assert.Single(gameBoard.Points[3].Checkers);  // Red checker should be on point 4
        Assert.Equal(whiteChecker, gameBoard.Points[3].Checkers.First());  // The red checker is now on point 4
        Assert.Single(blackPlayer.Bar);  // White checker should be on the bar
        Assert.Equal(blackChecker, blackPlayer.Bar.First());  // White checker is the one on the bar
    }

    [Fact]
    public void IsMoveValid_ShouldReturnTrueForValidMove()
    {
        // Arrange
        GameBoard gameBoard = new GameBoard();
        Player player = new Player(CheckerColor.White);
        Checker checker = new Checker(CheckerColor.White, 1);
        gameBoard.Points[0].AddChecker(checker);  // Place checker on point 1
        player.AddChecker(checker);

        // Act
        bool isValid = gameBoard.IsMoveValid(player, 1, 3, 2);  // Move from point 1 to 3 with dice roll 2

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void IsMoveValid_ShouldReturnFalseWhenFromIndexIsInvalid()
    {
        // Arrange
        GameBoard gameBoard = new GameBoard();
        Player player = new Player(CheckerColor.White);

        // Act
        bool isValid = gameBoard.IsMoveValid(player, 25, 3, 2);  // Invalid fromIndex

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsMoveValid_ShouldReturnFalseWhenToIndexIsInvalid()
    {
        // Arrange
        GameBoard gameBoard = new GameBoard();
        Player player = new Player(CheckerColor.White);

        // Act
        bool isValid = gameBoard.IsMoveValid(player, 1, 25, 2);  // Invalid toIndex

        // Assert
        Assert.False(isValid);
    }

    [Fact]
    public void IsMoveValid_ShouldReturnFalseWhenDestinationIsBlockedByOpponent()
    {
        // Arrange
        GameBoard gameBoard = new GameBoard();
        Player whitePlayer = new Player(CheckerColor.White);
        Player blackPlayer = new Player(CheckerColor.Black);
        Checker whiteChecker = new Checker(CheckerColor.White, 1);
        Checker blackChecker1 = new Checker(CheckerColor.Black, 4);
        Checker blackChecker2 = new Checker(CheckerColor.Black, 4);
        gameBoard.Points[0].AddChecker(whiteChecker);  // Red checker on point 1
        gameBoard.Points[3].AddChecker(blackChecker1);  // Two white checkers on point 4
        gameBoard.Points[3].AddChecker(blackChecker2);
        whitePlayer.AddChecker(whiteChecker);
        blackPlayer.AddChecker(blackChecker1);
        blackPlayer.AddChecker(blackChecker2);

        // Act
        bool isValid = gameBoard.IsMoveValid(whitePlayer, 1, 4, 3);  // Move from point 1 to 4

        // Assert
        Assert.False(isValid);  // Point 4 is blocked by two white checkers
    }
}
