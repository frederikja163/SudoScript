using NUnit.Framework;
using SudoScript.Core.Data;
using Tests;

namespace StandardLibrary.Test
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
            board[4, 4].Digit = 5;
            Assert.IsTrue(unit.ValidateRules());
        }
        [Test]
        public void InvalidOddUnitTest()
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
            board[2, 2].Digit = 6;
            board[3, 3].Digit = 5;
            board[4, 4].Digit = 1;
            Assert.IsFalse(unit.ValidateRules());
        }
        [Test]
        public void EliminateCandidatesTest()
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
            unit.EliminateCandidates();

            foreach (Cell cell in unit.Cells())
            {
                Assert.That(cell.Candidates(), Is.EqualTo(new List<int> { 1, 3, 5, 7, 9 }));
                Assert.IsFalse(cell.HasCandidate(2));
                Assert.IsFalse(cell.HasCandidate(4));
                Assert.IsFalse(cell.HasCandidate(6));
                Assert.IsFalse(cell.HasCandidate(8));
            }
        }
    }
}
