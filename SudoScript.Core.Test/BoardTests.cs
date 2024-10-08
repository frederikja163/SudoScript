using NUnit.Framework;
using SudoScript.Core;
using SudoScript.Core.Data;

namespace SudoScript.Core.Test;

internal sealed class BoardTests
{
    [Test]
    public void ValidateBoardTest()
    {
        Board board = Util.CreateStandardEmpty();
        board[5, 1].Digit = 2;
        board[4, 8].Digit = 7;
        board[5, 2].Digit = 2;
        board[8, 4].Digit = 4;

        Assert.That(board.Validate(), Is.False);
        board[5, 2].Digit = 3;
        Assert.That(board.Validate(), Is.True);

        board[9, 9].EliminateCandidate(1);
        board[9, 9].EliminateCandidate(2);
        board[9, 9].EliminateCandidate(3);
        board[9, 9].EliminateCandidate(4);
        board[9, 9].EliminateCandidate(5);
        board[9, 9].EliminateCandidate(6);
        board[9, 9].EliminateCandidate(7);
        board[9, 9].EliminateCandidate(8);
        board[9, 9].EliminateCandidate(9);

        Assert.That(board.Validate(), Is.False);
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
    public void BoardNotEqualsTest()
    {
        Board board = Util.CreateStandardEmpty();
        board[1, 1].Digit = 2;
        board[4, 8].Digit = 7;
        board[5, 3].Digit = 3;
        board[8, 4].Digit = 4;

        Board board2 = Util.CreateStandardEmpty();
        board2[1, 1].Digit = 1;
        board2[4, 8].Digit = 7;
        board2[5, 3].Digit = 3;
        board2[8, 4].Digit = 4;

        Assert.That(board2, Is.Not.EqualTo(board));
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
    public void BoardHashNotEqualTest()
    {
        Board board = Util.CreateStandardEmpty();
        board[5, 1].Digit = 2;
        board[4, 8].Digit = 7;
        board[5, 2].Digit = 3;
        board[8, 4].Digit = 4;

        Board board2 = Util.CreateStandardEmpty();
        board2[5, 1].Digit = 2;
        board2[4, 9].Digit = 7;
        board2[5, 2].Digit = 3;
        board2[8, 4].Digit = 4;

        Assert.That(board2.GetHashCode(), Is.Not.EqualTo(board.GetHashCode()));
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
