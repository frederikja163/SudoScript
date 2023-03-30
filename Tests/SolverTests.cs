using NUnit.Framework;
using SudoScript;
using SudoScript.Data;

namespace Tests;

internal sealed class SolverTests
{
    [Test()]
    public void CanSolveEmptySudoku()
    {
        Board board = Util.CreateStandardEmpty();
        Assert.DoesNotThrow(() => Solver.Solve(board));
        Assert.IsTrue(board.ValidateRules());
        Assert.IsFalse(board.Cells().Any(c => c.Digit == Cell.EmptyDigit));
    }
}
