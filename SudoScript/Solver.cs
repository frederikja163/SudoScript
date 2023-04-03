using ConcurrentCollections;
using SudoScript.Data;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SudoScript;

public static class Solver
{
    // Solve using Wave Function collapse algorithm with clone based back tracking.
    // Perhaps a multithreading solver instead of actual back tracking to calculate multiple paths at once.
    public static Board Solve(Board board, bool isAsync = false)
    {
        Board? newBoard = board.Clone();
        ConcurrentHashSet<Board> solved = new ConcurrentHashSet<Board>();
        if (!SolveRec(newBoard, solved, new Config() { IsAsync = isAsync}))
        {
            throw new Exception("Board is not solveable");
        }

        return solved.First();
    }

    private class Config
    {
        public int SolutionCount { get; init; } = 1;
        public bool IsAsync { get; init; } = false;
    }

    private static bool SolveRec(Board board, ConcurrentHashSet<Board> solvedBoards, Config config)
    {
        // Eliminate candidates from all rules untill nothing changes.
        while (board.EliminateCandidates()) ;

        // We hit an invalid state, and must backtrack.
        if (!board.ValidateRules())
        {
            return false;
        }

        // Get a list of all cells with the least amount of candidates.
        IEnumerable<Cell> orderedCells = board.Cells()
            .OrderBy(c => c.CandidateCount);
        if (orderedCells.FirstOrDefault()?.CandidateCount < 1)
        {
            return false;
        }
        // Skip all cells with less than 2 candidates.
        orderedCells = orderedCells.SkipWhile(c => c.CandidateCount <= 1);
        // The first cell contains the smallest amount of candidates.
        int lowestCandidateCount = orderedCells.FirstOrDefault()?.CandidateCount ?? 1;
        // Take all cells with the least amount of candidates.
        orderedCells = orderedCells.TakeWhile(c => c.CandidateCount == lowestCandidateCount);
        // If there are no cells with more than 1 candidate, the board is solved.
        if (lowestCandidateCount == 1)
        {
            solvedBoards.Add(board);
            return true;
        }

        Cell cell = orderedCells.OrderBy(_ => Random.Shared.Next()).First();
        Loop(config.IsAsync, cell.Candidates(), candidate =>
        {
            if (solvedBoards.Count >= config.SolutionCount)
            {
                return true;
            }
            // Create a clone of the board for backtracking.
            Board clonedBoard = board.Clone();
            Cell clonedCell = clonedBoard[cell.X, cell.Y];

            // Collapse the cell with a digit.
            clonedCell.Digit = candidate;

            // Call solve on the new board.
            SolveRec(clonedBoard, solvedBoards, config);
            return false;
        });

        return solvedBoards.Count >= config.SolutionCount;
    }

    private static void Loop<TSource>(bool isAsync, IEnumerable<TSource> enumerable, Func<TSource, bool> body)
    {
        if (isAsync)
        {
            Parallel.ForEach(enumerable, (i, state) =>
            {
                if (body.Invoke(i))
                {
                    state.Break();
                }
            });
        }
        else
        {

            foreach (TSource item in enumerable)
            {
                if (body.Invoke(item))
                {
                    break;
                }
            }
        }
    }

    public static Board GenerateSolveable(Board board)
    {
        throw new NotImplementedException();
    }

    public static bool IsSatisfactory(Board board)
    {
        throw new NotImplementedException();
    }

    public static bool IsProper(Board board)
    {
        throw new NotImplementedException();
    }
}
