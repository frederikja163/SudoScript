using NUnit.Framework;
using SudoScript.Core.Data;
using Tests;

namespace StandardLibrary.Test;
internal sealed class UniqueTests
{
    [Test]
    public void ValidUniqueRuleUnit()
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
            new Unique()
        });

        Board board = Util.CreateStandardEmpty(unit);
        board[1, 1].Digit = 1;
        board[2, 2].Digit = 2;
        board[3, 3].Digit = 3;
        board[4, 4].Digit = 4;
        board[5, 5].Digit = 5;
        board[6, 6].Digit = 6;
        board[7, 7].Digit = 7;

        Assert.IsTrue(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
    }

    [Test]
    public void InvalidUniqueRuleUnit()
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
            new Unique()
        });

        Board board = Util.CreateStandardEmpty(unit);
        board[1, 1].Digit = 2;
        board[2, 2].Digit = 2;
        board[3, 3].Digit = 3;
        board[4, 4].Digit = 4;
        board[5, 5].Digit = 5;
        board[6, 6].Digit = 6;
        board[7, 7].Digit = 7;

        Assert.IsFalse(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
    }

    [Test]
    public void CandidatesEliminatedUniqueRuleUnit()
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
            new Unique()
        });

        Board board = Util.CreateStandardEmpty(unit);
        board[1, 1].Digit = 1;
        board[2, 2].Digit = 2;
        board[4, 4].Digit = 4;
        board[5, 5].Digit = 5;
        board[7, 7].Digit = 7;

        Assert.IsTrue(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());

        board.EliminateCandidates();
        //Assert candidates are still left
        Assert.IsTrue(board[3, 3].Candidates().Contains(3));
        Assert.IsTrue(board[3, 3].Candidates().Contains(6));
        Assert.IsTrue(board[3, 3].Candidates().Contains(8));
        Assert.IsTrue(board[3, 3].Candidates().Contains(9));

        //Assert candidates are removed
        Assert.IsFalse(board[3, 3].Candidates().Contains(1));
        Assert.IsFalse(board[3, 3].Candidates().Contains(2));
        Assert.IsFalse(board[3, 3].Candidates().Contains(4));
        Assert.IsFalse(board[3, 3].Candidates().Contains(5));
        Assert.IsFalse(board[3, 3].Candidates().Contains(7));
    }
}
