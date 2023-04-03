using NUnit.Framework;
using SudoScript.Data;

namespace Tests;

internal sealed class OneRuleTests
{
    [Test]
    public void ValidOneRuleTest()
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
            (8, 8),
            (9, 9),
        },
        new List<IRule>
        {
            new OneRule()
        });
        Board board = Util.CreateStandardEmpty(unit);

        board[1, 1].Digit = 1;
        board[2, 2].Digit = 2;
        board[3, 3].Digit = 3;
        board[4, 4].Digit = 4;
        board[5, 5].Digit = 5;
        board[6, 6].Digit = 6;
        board[7, 7].Digit = 7;
        board[8, 8].Digit = 8;
        board[9, 9].Digit = 9;

        Assert.IsTrue(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
    }

    [Test]
    public void InvalidOneRuleTest()
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
            (8, 8),
            (9, 9),
        },
        new List<IRule>
        {
            new OneRule()
        });
        Board board = Util.CreateStandardEmpty(unit);

        board[1, 1].Digit = 1;
        board[2, 2].Digit = 1;
        board[3, 3].Digit = 1;
        board[4, 4].Digit = 1;
        board[5, 5].Digit = 1;
        board[6, 6].Digit = 1;
        board[7, 7].Digit = 1;
        board[8, 8].Digit = 1;
        board[9, 9].Digit = 1;

        Assert.IsFalse(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
    }

    [Test]
    public void NotFilledOneRule()
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
            (8, 8),
            (9, 9),
        },
        new List<IRule>
        {
            new OneRule()
        });
        Board board = Util.CreateStandardEmpty(unit);

        Assert.IsTrue(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
    }

    [Test]
    public void InvalidUnitTooSmallOneRule()
    {
        Unit unit = new Unit(new List<CellReference>
        {
            (1, 1),
        },
        new List<IRule>
        {
            new OneRule()
        });
        Board board = Util.CreateStandardEmpty(unit);

        Assert.IsFalse(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
    }

    [Test]
    public void InvalidUnitTooBigOneRule()
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
            (8, 8),
            (9, 9),
            (1, 2),
        },
        new List<IRule>
        {
            new OneRule()
        });
        Board board = Util.CreateStandardEmpty(unit);

        Assert.IsFalse(unit.ValidateRules());
        Assert.DoesNotThrow(() => unit.EliminateCandidates());
    }
}
