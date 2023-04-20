using NUnit.Framework;
using SudoScript.Data;
using Tests;

namespace StandardLibrary.Tests
{
    internal sealed class OddTests
    {
        [Test]
        public void ValidOddUnitTest()
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
                new Odd()
            });
            Board board = Util.CreateStandardEmpty(unit);
            board[1, 1].Digit = 9;
            board[2, 2].Digit = 7;
            board[3, 3].Digit = 5;
            Assert.IsTrue(unit.ValidateRules());
        }
    }
}
