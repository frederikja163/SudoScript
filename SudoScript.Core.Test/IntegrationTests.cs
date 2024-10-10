using NUnit.Framework;
using SudoScript.Core.Data;

namespace SudoScript.Core.Test;

internal sealed class IntegrationTests
{
    private static string Evil = @"/* Implement a standard Sudoku, givens from:
   sudoku.com
 */

// Rows for default Sudoku.
Row 1
Row 2
Row 3
Row 4
Row 5
Row 6
Row 7
Row 8
Row 9

// Collumns for default Sudoku.
Column 1
Column 2
Column 3
Column 4
Column 5
Column 6
Column 7
Column 8
Column 9

// Boxes for default Sudoku.
Box (1, 1)
Box (4, 1)
Box (7, 1)
Box (1, 4)
Box (4, 4)
Box (7, 4)
Box (1, 7)
Box (4, 7)
Box (7, 7)

// Givens for the easy sudoku from 'sudoku.com'.
givens {
	(9, 1) 1
	(1, 2) 3
	(5, 2) 8
	(7, 2) 9
	(8, 2) 7
	(3, 3) 5
	(6, 3) 4
	(3, 4) 3
	(4, 4) 5
	(7, 4) 2
	(8, 4) 1
	(1, 5) 1
	(9, 5) 8
	(5, 6) 2
	(9, 6) 6
	(1, 7) 7
	(5, 7) 9
	(7, 7) 3
	(8, 7) 2
	(7, 8) 6
	(1, 9) 2
	(2, 9) 9
	(4, 9) 8
}";

	private static readonly string Normal = @"
/* Implement a standard Sudoku, givens from:
   sudoku.com
 */

// Rows for default Sudoku.
Row 1
Row 2
Row 3
Row 4
Row 5
Row 6
Row 7
Row 8
Row 9

// Collumns for default Sudoku.
Column 1
Column 2
Column 3
Column 4
Column 5
Column 6
Column 7
Column 8
Column 9

// Boxes for default Sudoku.
Box (1, 1)
Box (4, 1)
Box (7, 1)
Box (1, 4)
Box (4, 4)
Box (7, 4)
Box (1, 7)
Box (4, 7)
Box (7, 7)

// Givens for the easy sudoku from 'sudoku.com'.
givens {
	// 1st row.
	(2, 1) 3
	(4, 1) 6
	// 2nd row.
	(4, 2) 9
	(6, 2) 5
	(7, 2) 8
	(8, 2) 6
	// 3rd row.
	(3, 3) 5
	(6, 3) 4
	(7, 3) 1
	(9, 3) 2
	// 4th row.
	(1, 4) 3
	(2, 4) 4
	(7, 4) 9
	(9, 4) 6
	// 5th row.
	(6, 5) 1
	(8, 5) 2
	// 6th row.
	(1, 6) 2
	(3, 6) 6
	(4, 6) 4
	// 7th row.
	(2, 7) 9
	(4, 7) 1
	(5, 7) 8
	// 8th row.
	(1, 8) 6
	(3, 8) 8
	(4, 8) 5
	(5, 8) 4
	(7, 8) 7
	(8, 8) 9
	(9, 8) 1
	// 9th row.
	(1, 9) 5
	(2, 9) 1
	(3, 9) 3
	(4, 9) 7
	(5, 9) 2
	(7, 9) 6
	(8, 9) 4
}";

    [Test]
    public void ParserGeneratorNormalTest()
    {
        Board board = SudoScript.Load(Normal);

        Assert.That(board.Units.Count(), Is.EqualTo(28));
        Assert.That(board.Cells().Count(), Is.EqualTo(81));
        Assert.That(board[2, 1].Digit, Is.EqualTo(3));
        Assert.True(board[2, 1].IsGiven);
        Assert.That(board[1, 1].Digit, Is.EqualTo(Cell.EmptyDigit));
        Assert.That(!board[1, 1].IsGiven);
    }

    [Test]
    public void ParserGeneratorEvilTest()
    {
        Board board = SudoScript.Load(Evil);

        Assert.That(board.Units.Count(), Is.EqualTo(28));
        Assert.That(board.Cells().Count(), Is.EqualTo(81));
        Assert.That(board[9, 1].Digit, Is.EqualTo(1));
        Assert.True(board[9, 1].IsGiven);
        Assert.That(board[1, 1].Digit, Is.EqualTo(Cell.EmptyDigit));
        Assert.That(!board[1, 1].IsGiven);
    }

    [Test]
    public void SolveNormalTest()
    {
        Board solvedBoard = SudoScript.Solve(Normal);

        Assert.True(solvedBoard.IsSolved());
    }

    [Test]
    public void SolveEvilTest()
    {
        Board solvedBoard = SudoScript.Solve(Evil);

        Assert.True(solvedBoard.IsSolved());
    }

	[Test]
	public void WildValidTest()
	{
		Board board = Util.CreateWildBoard();

		Assert.That(board, Is.Not.Null);
		Assert.That(board.Validate(), Is.True);
		//Assert.That(Solver.Solve(board!).IsSolved(), Is.True);
		//Assert.That(Solver.FindSolutions(board, 3).Count, Is.EqualTo(3));
    }
}
