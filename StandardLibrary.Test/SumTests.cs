using NUnit.Framework;
using SudoScript.Core.Data;
using Tests;

namespace StandardLibrary.Test
{
    internal sealed class SumTests
    {
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
                new Sum(20)
            });
            Board board = Util.CreateStandardEmpty(unit);
            board[1, 1].Digit = 5;
            board[2, 2].Digit = 5;
            board[3, 3].Digit = 5;

            Assert.IsTrue((board[4, 4].Candidates().ToArray().Length) == 9);
            Assert.IsFalse((board[4, 4].Candidates().ToArray().Length) < 9);
            unit.EliminateCandidates();

            for (int i = 4; i <= 7; i++) 
            {
                for (int j = 1; j <= 2; j++)
                {
                    Assert.That(board[i, i].Candidates(), Has.Member(j));
                }
                for (int j = 3; j <=9; j++)
                {
                    Assert.That(board[i, i].Candidates(), Has.No.Member(j));
                }
            }
        }

        [Test]
        public void ValidSumUnit()
        {
            Unit unit = new Unit(new List<CellReference>
            {
                (1, 1),
                (2, 2),
                (3, 3),
            }, new List<IRule>
            {
                new Sum(15)
            });
            Board board = Util.CreateStandardEmpty(unit);
            board[1, 1].Digit = 5;
            board[2, 2].Digit = 5;
            board[3, 3].Digit = 5;

            Assert.IsTrue(unit.ValidateRules());

            board[3, 3].Digit = 4;
            Assert.IsFalse(unit.ValidateRules());
        }
    }
}
