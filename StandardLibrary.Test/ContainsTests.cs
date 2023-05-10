using NUnit.Framework;
using SudoScript.Core.Data;

namespace StandardLibrary.Test;

public sealed class ContainsTests
{
    [Test]
    public void ValidContainsUnitTest()
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
            new Contains(9)
        }) ;

        Board board = Util.CreateStandardEmpty(unit);
        board[1, 1].Digit = 1;
        board[2, 2].Digit = 2;
        board[4, 4].Digit = 4;
        board[5, 5].Digit = 9;
        board[7, 7].Digit = 7;

        Assert.IsTrue(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
    }

    [Test]
    public void InvalidContainsUnitTest()
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
            new Contains(8)
        });

        Board board = Util.CreateStandardEmpty(unit);
        board[1, 1].Digit = 1;
        board[2, 2].Digit = 2;
        board[4, 4].Digit = 4;
        board[5, 5].Digit = 9;
        board[7, 7].Digit = 7;

        Assert.IsFalse(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
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
            new Contains(8)
        });

        Board board = Util.CreateStandardEmpty(unit);
        board[1, 1].Digit = 1;
        board[2, 2].Digit = 2;
        board[4, 4].Digit = 4;
        board[5, 5].Digit = 9;
        board[7, 7].Digit = 7;
            
        Assert.IsFalse(unit.ValidateRules());
        unit.EliminateCandidates();
        Assert.IsTrue(board[3, 3].Candidates().Contains(8));
        Assert.IsTrue(board[6, 6].Candidates().Contains(8));

        board[3, 3].EliminateCandidate(8);
        unit.EliminateCandidates();

        Assert.IsTrue(board[6, 6].CandidateCount == 1);
        Assert.IsTrue(board[6, 6].Candidates().Contains(8));
    }
}