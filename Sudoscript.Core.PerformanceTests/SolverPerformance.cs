﻿using BenchmarkDotNet.Attributes;
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
    private Board _easyBoard;

    public BoardSolverBenchmarks()
    {
        _smallBoard = Util.CreateSmallBoard();
        _standardBoard = Util.CreateStandardEmpty();
        _easyBoard = Util.CreateEasyBoard();
    }


    public IEnumerable<object[]> Solvers()
    {
        yield return new object[] { "Base", new Func<Board, int, bool, List<Board>>(Solver.FindSolutions) };
        yield return new object[] { "MultiThread", new Func<Board, int, bool, List<Board>>(SolverMultiThread.FindSolutions) };
        yield return new object[] { "DynamicCandidates", new Func<Board, int, bool, List<Board>>(SolverDynamicCandidatesByUnits.FindSolutions) };
        yield return new object[] { "DynamicCandidates", new Func<Board, int, bool, List<Board>>(SolverDynamicCandidatesByLow.FindSolutions) };
        yield return new object[] { "DynamicCandidates", new Func<Board, int, bool, List<Board>>(SolverDynamicCandidatesByHigh.FindSolutions) };
    }

    [GlobalSetup]
    public void Setup()
    {
        
    }

    [Benchmark]
    [ArgumentsSource(nameof(Solvers))]
    public List<Board> TestSmallBoard(string solverName, Func<Board, int, bool, List<Board>> solverFunction)
    {
        return solverFunction(_smallBoard, 10000, false);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Solvers))]
    public List<Board> TestLargeBoard(string solverName, Func<Board, int, bool, List<Board>> solverFunction)
    {
        return solverFunction(_standardBoard, 10000, false);
    }

    [Benchmark]
    [ArgumentsSource(nameof(Solvers))]
    public List<Board> TestEasyBoard(string solverName, Func<Board, int, bool, List<Board>> solverFunction)
    {
        return solverFunction(_easyBoard, 0, false);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BoardSolverBenchmarks>();
    }
}