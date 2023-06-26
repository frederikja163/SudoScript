using NUnit.Framework;
using SudoScript.Core.Data;
namespace StandardLibrary.Test;

internal sealed class BattenburgTest
{
    [Test]
    public void BattenburgContainsFourCells()
    {
        Unit unit = new Battenburg(1,1);
        Util.CreateStandardEmpty(unit);


        Assert.AreEqual(4, unit.Cells().Count());
    }

    [Test]
    public void BattenburgValidateRules()
    {
        Unit unit = new Battenburg(1, 1);
        Board board = Util.CreateStandardEmpty(unit);

        board[1, 1].Digit = 9;
        Assert.IsTrue(unit.ValidateRules());
        board[1, 2].Digit = Cell.EmptyDigit;
        Assert.IsTrue(unit.ValidateRules());
        board[1, 2].Digit = 1;
        Assert.IsFalse(unit.ValidateRules());
        board[1, 2].Digit = 2;
        Assert.IsTrue(unit.ValidateRules());
        board[2, 1].Digit = 2;
        Assert.IsTrue(unit.ValidateRules());
        board[2, 2].Digit = 4;
        Assert.IsFalse(unit.ValidateRules());
        board[2, 2].Digit = 3;
        Assert.IsTrue(unit.ValidateRules());
    }
}

