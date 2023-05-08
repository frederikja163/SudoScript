using NUnit.Framework;
using SudoScript.Core.Data;

namespace StandardLibrary.Test
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
        public void InvalidEvenUnitTest()
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
                new Even()
            });
            Board board = Util.CreateStandardEmpty(unit);
            unit.EliminateCandidates();

            foreach (Cell cell in unit.Cells())
            {
                Assert.That(cell.Candidates(), Is.EqualTo(new List<int>{ 2, 4, 6, 8}));
                Assert.IsFalse(cell.HasCandidate(1));
                Assert.IsFalse(cell.HasCandidate(3));
                Assert.IsFalse(cell.HasCandidate(5));
                Assert.IsFalse(cell.HasCandidate(7));
                Assert.IsFalse(cell.HasCandidate(9));
            }
        }
    }
}
