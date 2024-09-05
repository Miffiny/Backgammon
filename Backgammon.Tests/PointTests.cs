using Core;

namespace Backgammon.Tests;

public class PointTests
    {
        [Fact]
        public void PointInitialization_ShouldSetIndexAndInitializeCheckers()
        {
            // Arrange
            int expectedIndex = 1;

            // Act
            Point point = new Point(expectedIndex);

            // Assert
            Assert.Equal(expectedIndex, point.Index);
            Assert.Empty(point.Checkers);
        }

        [Fact]
        public void AddChecker_ShouldAddCheckerToPoint()
        {
            // Arrange
            Point point = new Point(1);
            Checker checker = new Checker("Red", 1);

            // Act
            point.AddChecker(checker);

            // Assert
            Assert.Single(point.Checkers);
            Assert.Equal(checker, point.Checkers.First());
        }

        [Fact]
        public void RemoveChecker_ShouldRemoveAndReturnLastChecker()
        {
            // Arrange
            Point point = new Point(1);
            Checker checker1 = new Checker("Red", 1);
            Checker checker2 = new Checker("Red", 1);
            point.AddChecker(checker1);
            point.AddChecker(checker2);

            // Act
            Checker removedChecker = point.RemoveChecker();

            // Assert
            Assert.Equal(checker2, removedChecker);
            Assert.Single(point.Checkers);
        }

        [Fact]
        public void RemoveChecker_OnEmptyPoint_ShouldReturnNull()
        {
            // Arrange
            Point point = new Point(1);

            // Act
            Checker removedChecker = point.RemoveChecker();

            // Assert
            Assert.Null(removedChecker);
        }

        [Fact]
        public void Owner_ShouldReturnNullWhenNoCheckers()
        {
            // Arrange
            Point point = new Point(1);

            // Act
            string owner = point.Owner;

            // Assert
            Assert.Null(owner);
        }

        [Fact]
        public void Owner_ShouldReturnColorOfFirstChecker()
        {
            // Arrange
            Point point = new Point(1);
            Checker checker = new Checker("Red", 1);
            point.AddChecker(checker);

            // Act
            string owner = point.Owner;

            // Assert
            Assert.Equal("Red", owner);
        }

        [Fact]
        public void IsBlot_ShouldReturnTrueWhenSingleOpponentCheckerExists()
        {
            // Arrange
            Point point = new Point(1);
            Checker checker = new Checker("Red", 1);
            point.AddChecker(checker);

            // Act
            bool isBlot = point.IsBlot("Blue");

            // Assert
            Assert.True(isBlot);
        }

        [Fact]
        public void IsBlot_ShouldReturnFalseWhenNoCheckers()
        {
            // Arrange
            Point point = new Point(1);

            // Act
            bool isBlot = point.IsBlot("Red");

            // Assert
            Assert.False(isBlot);
        }

        [Fact]
        public void IsBlot_ShouldReturnFalseWhenMoreThanOneChecker()
        {
            // Arrange
            Point point = new Point(1);
            point.AddChecker(new Checker("Red", 1));
            point.AddChecker(new Checker("Red", 1));

            // Act
            bool isBlot = point.IsBlot("Blue");

            // Assert
            Assert.False(isBlot);
        }

        [Fact]
        public void IsBlot_ShouldReturnFalseWhenCheckerBelongsToPlayer()
        {
            // Arrange
            Point point = new Point(1);
            point.AddChecker(new Checker("Red", 1));

            // Act
            bool isBlot = point.IsBlot("Red");

            // Assert
            Assert.False(isBlot);
        }
    }