using NUnit.Framework;
using SudoScript.Data;
using Tests;

namespace StandardLibrary.Tests
{
    internal sealed class EvenTests
    {
        [Test]
        public void ValidEvenUnitTest()
        {
            Unit unit = new Unit(new List<CellReference>
            {
                (1, 1),
                (2, 2),
                (3, 3),
                (4, 4),
                (5, 5),
                (6, 6),
                (7, 7),
            }, new List<IRule>
            {
                new Even()
            });
            Board board = Util.CreateStandardEmpty(unit);
            board[1, 1].Digit = 8;
            board[2, 2].Digit = 4;
            board[3, 3].Digit = 4;
            board[4, 4].Digit = 2;
            Assert.IsTrue(unit.ValidateRules());
        }
        [Test]
        public void InValidEvenUnitTest()
        {
            Unit unit = new Unit(new List<CellReference>
            {
                (1, 1),
                (2, 2),
                (3, 3),
                (4, 4),
                (5, 5),
                (6, 6),
                (7, 7),
            }, new List<IRule>
            {
                new Even()
            });
            Board board = Util.CreateStandardEmpty(unit);
            board[1, 1].Digit = 9;
            board[2, 2].Digit = 4;
            board[3, 3].Digit = 4;
            board[4, 4].Digit = 2;
            Assert.IsFalse(unit.ValidateRules());
        }
    }
}
