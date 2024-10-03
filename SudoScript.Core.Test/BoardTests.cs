using NUnit.Framework;
using SudoScript.Core;
using SudoScript.Core.Data;

namespace SudoScript.Core.Test;

internal sealed class BoardTests
{
    [Test]
    public void ValidateBoardTest()
    {
        Assert.IsTrue(false);
    }

    [Test]
    public void ValidateRulesTest()
    {
        Assert.IsTrue(false);
    }

    [Test]
    public void BoardEqualsTest()
    {
        Board board = Util.CreateStandardEmpty();
        board[5, 1].Digit = 2;
        board[4, 8].Digit = 7;
        board[5, 2].Digit = 3;
        board[8, 4].Digit = 4;

        Board board2 = Util.CreateStandardEmpty();
        board2[5, 1].Digit = 2;
        board2[4, 8].Digit = 7;
        board2[5, 2].Digit = 3;
        board2[8, 4].Digit = 4;

        Assert.That(board2, Is.EqualTo(board));
    }

    [Test]
    public void BoardHashTest()
    {
        Board board = Util.CreateStandardEmpty();
        board[5, 1].Digit = 2;
        board[4, 8].Digit = 7;
        board[5, 2].Digit = 3;
        board[8, 4].Digit = 4;

        Board board2 = Util.CreateStandardEmpty();
        board2[5, 1].Digit = 2;
        board2[4, 8].Digit = 7;
        board2[5, 2].Digit = 3;
        board2[8, 4].Digit = 4;

        Assert.That(board2.GetHashCode(), Is.EqualTo(board.GetHashCode()));
    }

    [Test]
    public void CloneTest()
    {
        Board board = Util.CreateStandardEmpty();
        board[5, 1].Digit = 2;
        board[4, 8].Digit = 7;
        board[5, 2].Digit = 3;
        board[8, 4].Digit = 4;

        Board clonedBoard = board.Clone();

        Assert.That(board, Is.EqualTo(clonedBoard));
    }
}
