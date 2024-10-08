using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using StandardLibrary;
using SudoScript.Core;
using SudoScript.Core.Data;
using SudoScript.Core.Test;

namespace SudoScript.Core.PerformanceTests;

public sealed class BoardSolverBenchmarks
{
    private Board _smallBoard;
    private Board _standardBoard;

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
        // Initialize the small and large board here
        _smallBoard = CreateSmallBoard(); // Assuming you have this function
        _standardBoard = Util.CreateStandardEmpty(); // Assuming you have this function
    }

    [Benchmark]
    public List<Board> TestSmallBoard()
    {
        // Test FindSolutions on a small board with a limit of 100 solutions
        return FindSolutions(_smallBoard, limit: 100);
    }

    [Benchmark]
    public List<Board> TestLargeBoard()
    {
        // Test FindSolutions on a large board with a limit of 100 solutions
        return FindSolutions(_largeBoard, limit: 100);
    }

    // You can add more benchmark methods for different board sizes or limits

    // CreateSmallBoard() and CreateLargeBoard() should be helper functions that
    // return pre-set or randomly generated boards for testing.
}

// This is the entry point that runs the benchmarks
public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BoardSolverBenchmarks>();
    }
}