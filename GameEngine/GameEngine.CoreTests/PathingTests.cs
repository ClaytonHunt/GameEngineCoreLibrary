using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameEngine.Core.UnitTests
{
    [TestClass]
    public class PathingTests
    {
        [TestMethod]
        public void Given_CreatePath_When_StartPointIsDefined_Then_StartPointShouldBeReturned()
        {
            // Arrange
            TestablePathing astar = new TestablePathing(null);
            var startPoint = new Node(1, 1);
            var endPoint = new Node(5, 1);

            // Act
            var path = astar.CreatePath(startPoint, endPoint);

            // Assert
            Assert.IsTrue(path.Contains(startPoint));
        }

        [TestMethod]
        public void Given_CreatePath_When_StartPointIsDefined_Then_AddWalkableSquaresToOpen()
        {
            // Arrange
            TestablePathing astar = new TestablePathing(null);
            var startPoint = new Node(1, 1);

            // Act
            var actual = astar.TestGetNeighbors(startPoint).Count;

            // Assert
            Assert.AreEqual(4, actual);
        }

        [TestMethod]
        public void Given_CreatePath_When_StartingPointIsDefined_Then_OpenNodesWithStartPointAsParentShouldHaveTenForGScore()
        {
            // Arrange
            TestablePathing astar = new TestablePathing(null);
            var startPoint = new Node(1, 1);
            var endPoint = new Node(5, 1);

            // Act
            var actual = astar.CreatePath(startPoint, endPoint)
                .Where(x => x.Walkable
                            && x.State == NodeState.Closed
                            && x.Parent == startPoint)
                .First()
                .GivenCost;

            // Assert
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void Given_CreatePath_When_StartPointAndEndPointsAreDefined_Then_EasternOpenNodeWithStartPointAsParentShouldHave30ForHScore()
        {
            // Arrange
            TestablePathing astar = new TestablePathing(null);
            var startPoint = new Node(1, 1);
            var endPoint = new Node(5, 1);

            // Act
            var actual = astar.CreatePath(startPoint, endPoint)
                .Where(x => x.Walkable
                            && x.State == NodeState.Closed
                            && x.Parent == startPoint
                            && x.X == 2)
                .First()
                .HeuristicCost;

            // Assert
            Assert.AreEqual(30, actual);
        }

        [TestMethod]
        public void Given_CreatePath_When_StraightHorizontalPathCrossesQuadrants_Then_FindValidPath()
        {
            // Arrange
            TestablePathing astar = new TestablePathing(null, -10, -10, 10,10);
            var startPoint = new Node(-1, 1);
            var endPoint = new Node(1, 1);

            // Act
            var actual = astar.CreatePath(startPoint, endPoint).Last();

            // Assert
            Assert.AreEqual(1, actual.X);
        }

        [TestMethod]
        public void Given_CreatePath_When_StraightVerticalPathCrossesQuadrants_Then_FindValidPath()
        {
            // Arrange
            TestablePathing astar = new TestablePathing(null, -10,-10,10,10);
            var startPoint = new Node(1, -1);
            var endPoint = new Node(1, 1);

            // Act
            var actual = astar.CreatePath(startPoint, endPoint).Last();

            // Assert
            Assert.AreEqual(1, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_DiagonalPathCrossesQuadrants_Then_FindValidPath()
        {
            // Arrange
            TestablePathing astar = new TestablePathing(null,-10,-10,10,10);
            var startPoint = new Node(-1, -1);
            var endPoint = new Node(1, 1);

            // Act
            var actual = astar.CreatePath(startPoint, endPoint).Last();

            // Assert
            Assert.AreEqual(1, actual.X);
            Assert.AreEqual(1, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_OppisiteDiagonalPathCrossesQuadrants_Then_FindValidPath()
        {
            // Arrange
            TestablePathing astar = new TestablePathing(null, -10,-10,10,10);
            var startPoint = new Node(1, 1);
            var endPoint = new Node(-1, -1);

            // Act
            var actual = astar.CreatePath(startPoint, endPoint).Last();

            // Assert
            Assert.AreEqual(-1, actual.X);
            Assert.AreEqual(-1, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_SingleObstructionIsInDirectPath_Then_FindValidPath()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 1);
            var endPoint = new Node(5, 1);

            // Act
            obstructions.Add(new Node(3, 1) { Walkable = false });
            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(5, actual.X);
            Assert.AreEqual(1, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_DoubleObstructionIsInDirectPath_Then_FindValidPath()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 1);
            var endPoint = new Node(5, 1);

            // Act
            obstructions.Add(new Node(3, 1) { Walkable = false });
            obstructions.Add(new Node(3, 0) { Walkable = false });
            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(5, actual.X);
            Assert.AreEqual(1, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_ObstructionCoverTwoSides_Then_FindValidPath()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 4);
            var endPoint = new Node(5, 4);

            // Act
            obstructions.Add(new Node(3, 6) { Walkable = false });
            obstructions.Add(new Node(3, 5) { Walkable = false });
            obstructions.Add(new Node(3, 4) { Walkable = false });
            obstructions.Add(new Node(3, 3) { Walkable = false });
            obstructions.Add(new Node(3, 2) { Walkable = false });
            obstructions.Add(new Node(4, 2) { Walkable = false });
            obstructions.Add(new Node(5, 2) { Walkable = false });
            obstructions.Add(new Node(6, 2) { Walkable = false });
            obstructions.Add(new Node(7, 2) { Walkable = false });
            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(5, actual.X);
            Assert.AreEqual(4, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_ObstructionCoverThreeSides_Then_FindValidPath()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 4);
            var endPoint = new Node(5, 4);

            // Act
            obstructions.Add(new Node(3, 6) { Walkable = false });
            obstructions.Add(new Node(3, 5) { Walkable = false });
            obstructions.Add(new Node(3, 4) { Walkable = false });
            obstructions.Add(new Node(3, 3) { Walkable = false });
            obstructions.Add(new Node(3, 2) { Walkable = false });
            obstructions.Add(new Node(4, 2) { Walkable = false });
            obstructions.Add(new Node(5, 2) { Walkable = false });
            obstructions.Add(new Node(6, 2) { Walkable = false });
            obstructions.Add(new Node(7, 2) { Walkable = false });
            obstructions.Add(new Node(4, 6) { Walkable = false });
            obstructions.Add(new Node(5, 6) { Walkable = false });
            obstructions.Add(new Node(6, 6) { Walkable = false });
            obstructions.Add(new Node(7, 6) { Walkable = false });
            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(5, actual.X);
            Assert.AreEqual(4, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_ObstructionCoverFourSides_Then_ReturnStartingPoint()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 4);
            var endPoint = new Node(5, 4);

            // Act
            obstructions.Add(new Node(3, 6) { Walkable = false });
            obstructions.Add(new Node(3, 5) { Walkable = false });
            obstructions.Add(new Node(3, 4) { Walkable = false });
            obstructions.Add(new Node(3, 3) { Walkable = false });
            obstructions.Add(new Node(3, 2) { Walkable = false });
            obstructions.Add(new Node(4, 2) { Walkable = false });
            obstructions.Add(new Node(5, 2) { Walkable = false });
            obstructions.Add(new Node(6, 2) { Walkable = false });
            obstructions.Add(new Node(7, 2) { Walkable = false });
            obstructions.Add(new Node(4, 6) { Walkable = false });
            obstructions.Add(new Node(5, 6) { Walkable = false });
            obstructions.Add(new Node(6, 6) { Walkable = false });
            obstructions.Add(new Node(7, 6) { Walkable = false });
            obstructions.Add(new Node(7, 3) { Walkable = false });
            obstructions.Add(new Node(7, 4) { Walkable = false });
            obstructions.Add(new Node(7, 5) { Walkable = false });
            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(2, actual.X);
            Assert.AreEqual(4, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_EndPointIsObstruction_Then_FindValidPath()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 4);
            var endPoint = new Node(5, 4);

            // Act
            obstructions.Add(new Node(5, 4) { Walkable = false });
            
            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(4, actual.X);
            Assert.AreEqual(4, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_EndpointIsObstructedByTwoAndMultipleTurns_Then_DontJitterBug()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 4);
            var endPoint = new Node(5, 4);

            // Act
            obstructions.Add(new Node(2, 3) { Walkable = false });
            obstructions.Add(new Node(2, 4) { Walkable = false });
            obstructions.Add(new Node(2, 5) { Walkable = false });

            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result[1];

            // Assert
            Assert.AreEqual(1, actual.X);
            Assert.AreEqual(3, actual.Y);

            // Arrange
            startPoint = new Node(1, 3);

            // Act
            astar = new TestablePathing(obstructions);
            result = astar.CreatePath(startPoint, endPoint);
            actual = result[1];

            // Assert
            Assert.AreNotEqual(startPoint.Y, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_StartPointIsObstructedOnNorthSouthAndWest_Then_FindEasternEndPoint()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 1);
            var endPoint = new Node(4, 1);

            // Act
            obstructions.Add(new Node(1, 0) { Walkable = false });
            obstructions.Add(new Node(0, 1) { Walkable = false });
            obstructions.Add(new Node(1, 2) { Walkable = false });

            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(4, actual.X);
            Assert.AreEqual(1, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_When_StartPointIsObstructedOnNorthSouthAndWest_Then_FindSouthernEndPoint()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 1);
            var endPoint = new Node(1, 4);

            // Act
            obstructions.Add(new Node(1, 0) { Walkable = false });
            obstructions.Add(new Node(0, 1) { Walkable = false });
            obstructions.Add(new Node(1, 2) { Walkable = false });

            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(1, actual.X);
            Assert.AreEqual(4, actual.Y);
        }

        [TestMethod]
        public void Given_CreatePath_WhenEndPointIsObstructedAndBlockedOnTwoSides__Then_FindEndPoint()
        {
            // Arrange
            List<Node> obstructions = new List<Node>();
            TestablePathing astar = new TestablePathing(obstructions);
            var startPoint = new Node(1, 2);
            var endPoint = new Node(6, 2);

            // Act
            obstructions.Add(new Node(3, 1) { Walkable = false });
            obstructions.Add(new Node(3, 2) { Walkable = false });
            obstructions.Add(new Node(3, 3) { Walkable = false });
            obstructions.Add(new Node(4, 1) { Walkable = false });
            obstructions.Add(new Node(5, 1) { Walkable = false });
            obstructions.Add(new Node(6, 2) { Walkable = false });

            var result = astar.CreatePath(startPoint, endPoint);
            var actual = result.Last();

            // Assert
            Assert.AreEqual(6, actual.X);
            Assert.AreEqual(1, actual.Y);
        }
    }

    public class TestablePathing : Pathing
    {
        public IList<Node> TestGetNeighbors(Node parent)
        {
            return GetNeighbors(parent);
        }

        public TestablePathing(List<Node> obstructions = null, int lowerXLimit = 0, int lowerYLimit = 0, int upperXLimit = 32, int upperYLimit = 32, int range = 1) 
            : base(obstructions, lowerXLimit, lowerYLimit, upperXLimit, upperYLimit, range) { }
    }
}
