using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using StandardLibrary;
using SudoScript.Core;
using SudoScript.Core.Data;
using SudoScript.Core.Test;

namespace SudoScript.Core.PerformanceTests;

public class BoardSolverBenchmarks
{
    private Board _smallBoard;
    private Board _standardBoard;

    public BoardSolverBenchmarks()
    {
        _smallBoard = CreateSmallBoard();
        _standardBoard = Util.CreateStandardEmpty();
    }


    public IEnumerable<object[]> Solvers()
    {
        yield return new object[] { "Base", new Func<Board, int, bool, List<Board>>(Solver.FindSolutions) };
        yield return new object[] { "MultiThread", new Func<Board, int, bool, List<Board>>(SolverMultiThread.FindSolutions) };
        yield return new object[] { "DynamicCandidates", new Func<Board, int, bool, List<Board>>(SolverDynamicCandidatesByUnits.FindSolutions) };
        yield return new object[] { "DynamicCandidates", new Func<Board, int, bool, List<Board>>(SolverDynamicCandidatesByLow.FindSolutions) };
        yield return new object[] { "DynamicCandidates", new Func<Board, int, bool, List<Board>>(SolverDynamicCandidatesByHigh.FindSolutions) };
    }

    public static Board CreateSmallBoard()
    {
        List<Cell> allCells = new List<Cell>();
        List<Unit> allUnits = new List<Unit>();

        for (int x = 1; x <= 6; x++)
        {
            for (int y = 1; y <= 6; y++)
            {
                allCells.Add(new Cell(x, y));
            }
        }

        allUnits.Add(Util.CreateBox(1, 1));
        allUnits.Add(Util.CreateBox(1, 4));
        allUnits.Add(Util.CreateBox(4, 4));
        allUnits.Add(Util.CreateBox(4, 1));

        for (int y = 1; y <= 6; y++)
        {
            List<CellReference> cellList = new List<CellReference>();
            for (int x = 1; x <= 6; x++)
            {
                cellList.Add(new CellReference(x, y));
            }
            allUnits.Add(new Unit(cellList, new List<IRule> { new Unique() }));
        }

        for (int x = 1; x <= 6; x++)
        {
            List<CellReference> cellList = new List<CellReference>();
            for (int y = 1; y <= 6; y++)
            {
                cellList.Add(new CellReference(x, y));
            }
            allUnits.Add(new Unit(cellList, new List<IRule> { new Unique() }));
        }

        return new Board(allCells, allUnits);
    }

    [GlobalSetup]
    public void Setup()
    {
        
    }

    [Benchmark]
    [ArgumentsSource(nameof(Solvers))]
    public List<Board> TestSmallBoard(string solverName, Func<Board, int, bool, List<Board>> solverFunction)
    {
        return solverFunction(_smallBoard, 100000, false);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Solvers))]
    public List<Board> TestLargeBoard(string solverName, Func<Board, int, bool, List<Board>> solverFunction)
    {
        return solverFunction(_standardBoard, 10000, false);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BoardSolverBenchmarks>();
    }
}