using NUnit.Framework;
using StandardLibrary;
using SudoScript.Core;
using SudoScript.Core.Data;
using System.Data;

namespace SudoScript.Core.Test;

internal sealed class SolverTests
{
    [Test()]
    public void CanSolveEmptySudoku()
    {
        Board board = Util.CreateStandardEmpty();
        Assert.DoesNotThrow(() => board = Solver.Solve(board));
        Assert.IsTrue(board.ValidateRules());
        Assert.IsFalse(board.Cells().Any(c => c.Digit == Cell.EmptyDigit));
    }

    [Test]
    public void CanSolveGeneratedSudoku()
    {
        Board board = Util.CreateEasyBoard();

        Console.WriteLine(board.ToString());

        Console.WriteLine("-------------------------------------------------");
        Assert.DoesNotThrow(() => board = Solver.Solve(board));
        Assert.IsTrue(board.ValidateRules());
        Assert.IsFalse(board.Cells().Any(c => c.Digit == Cell.EmptyDigit));

        Console.WriteLine(board.ToString());
    }

    [Test]
    public void IsSatisfactoryTest()
    {
        Board board = Util.CreateEasyBoard();

        Assert.IsTrue(Solver.IsSatisfactory(board));
    }

    [Test]
    public void IsNotSatisfactoryTest()
    {
        Board board = Util.CreateStandardEmpty();

        Assert.IsFalse(Solver.IsSatisfactory(board));
    }

    [Test]
    public void FindSolutionsTwoUniqueCellsTest()
    {

        Board board = new(new List<Cell>{ new Cell(1, 1), new Cell(1, 2) }, 
            new List<Unit> {
                new Unit(new List<CellReference> { 
                    new CellReference(1, 1), 
                    new CellReference(1,2) }, 
                new List<IRule> { new Unique { } })});

        List<Board>? boardList = Solver.FindSolutions(board);

        Assert.IsNotNull(boardList);
        Assert.That(boardList.Count(), Is.EqualTo(72));

        List<Board>? randomBoardList = Solver.FindSolutions(board, 0, true);

        Assert.IsNotNull(randomBoardList);
        Assert.That(boardList.Count(), Is.EqualTo(randomBoardList.Count()));
    }

    [Test]
    public void FindNumberOfSolutionsTest()
    {
        Board board = new(new List<Cell> { new Cell(1, 1), new Cell(1, 2) },
            new List<Unit> {
                new Unit(new List<CellReference> {
                    new CellReference(1, 1),
                    new CellReference(1,2) },
                new List<IRule> { new Unique { } })});

        List<Board>? boardList = Solver.FindSolutions(board, 25);

        Assert.IsNotNull(boardList);
        Assert.That(boardList.Count(), Is.EqualTo(25));
    }
}
