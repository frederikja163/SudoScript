using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using SudoScript.Core;
using SudoScript.Core.Data;

namespace SudoScript.Core.PerformanceTests;

public sealed class BoardSolverBenchmarks
{
    private Board _smallBoard;
    private Board _largeBoard;

    public static Board CreateSmallBoard()
    {

    }

    [GlobalSetup]
    public void Setup()
    {
        // Initialize the small and large board here
        _smallBoard = CreateSmallBoard(); // Assuming you have this function
        _largeBoard = CreateLargeBoard(); // Assuming you have this function
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