using System;
using System.Diagnostics.CodeAnalysis;
using SudoScript.Core.Data;

namespace SudoScript.Core;

public static class Solver
{
    // Solve using Wave Function collapse algorithm with clone based back tracking.
    // Perhaps a multithreading solver instead of actual back tracking to calculate multiple paths at once.
    public static Board Solve(Board board)
    {
        Board? newBoard = board.Clone();
        if (!SolveRec(newBoard, out newBoard))
        {
            throw new Exception("Board is not solveable");
        }

        return newBoard;
    }

    private static bool SolveRec(Board board, [NotNullWhen(true)] out Board? solvedBoard)
    {
        // Eliminate candidates from all rules until nothing changes.
        while (board.EliminateCandidates());

        // We hit an invalid state, and must backtrack.
        if (!board.ValidateRules())
        {
            solvedBoard = null;
            return false;
        }

        // Get a list of all cells with the least amount of candidates.
        IEnumerable<Cell> orderedCells = board.Cells()
            .OrderBy(c => c.CandidateCount);
        if (orderedCells.FirstOrDefault()?.CandidateCount < 1)
        {
            solvedBoard = null;
            return false;
        }
        // Skip all cells with less than 2 candidates.
        orderedCells = orderedCells.SkipWhile(c => c.CandidateCount <= 1);
        // The first cell contains the smallest amount of candidates.
        int lowestCandidateCount = orderedCells.FirstOrDefault()?.CandidateCount ?? 1;
        // If there are no cells with more than 1 candidate, the board is solved.
        if (lowestCandidateCount == 1)
        {
            solvedBoard = board;
            return true;
        }
        // Take all cells with the least amount of candidates.
        orderedCells = orderedCells.TakeWhile(c => c.CandidateCount == lowestCandidateCount);

        Cell cell = orderedCells.First();
        foreach (int candidate in cell.Candidates())
        {
            // Create a clone of the board for backtracking.
            Board clonedBoard = board.Clone();
            Cell clonedCell = clonedBoard[cell.X, cell.Y];

            // Collapse the cell with a digit.
            clonedCell.Digit = candidate;

            // Call solve on the new board.
            if (SolveRec(clonedBoard, out solvedBoard))
            {
                return true;
            }
        }
        solvedBoard = null;
        return false;
    }

    /// <summary>
    /// Finds a number of solutions to a given board.
    /// </summary>
    /// <param name="board"></param>
    /// <param name="random">Whether the solved boards should be randomized.</param>
    /// <param name="solutionCount">Maximum number of solutions that should be in the returned list. Zero means unlimited.</param>
    /// <returns></returns>
    public static List<Board> FindSolutions(Board board, int limit = 0, bool random = false)
    {
        HashSet<Board> result = SolveRecAll(board, 0, limit, random);
        // If a limit is specified and the result count exceeds it, return only the correct number of solutions.
        if (limit != 0 && result.Count > limit)
        {
            return result.Take(limit).ToList();
        }
        return result.ToList();
    }

    private static HashSet<Board> SolveRecAll(Board board, int solutionCount, int limit, bool random)
    {
        HashSet<Board> solutions = new HashSet<Board>();
        // Eliminate candidates from all rules until nothing changes.
        while (board.EliminateCandidates());
        // We hit an invalid state, and must backtrack.
        if (!board.Validate())
        {
            return solutions;
        }
        // If the board is solved, return it.
        if (board.IsSolved())
        {
            return new HashSet<Board> { board };
        }

        // Pick cell to collapse.
        Cell cell;
        if (random)
        {
            int randomIndex = new Random().Next(0, board.Cells().Count());
            cell = board.Cells().Skip(randomIndex).First();
        }
        else
        {
            IEnumerable<Cell> orderedCells = board.Cells()
                .OrderBy(c => c.CandidateCount);
            // Skip all cells with less than 2 candidates.
            orderedCells = orderedCells.SkipWhile(c => c.CandidateCount <= 1);
            cell = orderedCells.First();
        }

        foreach (int candidate in cell.Candidates())
        {
            // Return if the limit of solutions has been reached

            if ((limit != 0) && ((solutions.Count() + solutionCount) >= limit))
            {
                return solutions;
            }

            // Create a clone of the board for backtracking.
            Board clonedBoard = board.Clone();
            Cell clonedCell = clonedBoard[cell.X, cell.Y];

            // Collapse the cell with a digit.
            clonedCell.Digit = candidate;

            // Call solve on the new board.
            HashSet<Board> subSolutions = SolveRecAll(clonedBoard, (solutionCount + solutions.Count()), limit, random);
            solutions.UnionWith(subSolutions);
        }
        return solutions;
    }

    public static Board GenerateSolveable(Board board)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks if the board can be solved by just using the EliminateCandidates methods from units.
    /// </summary>
    /// <param name="board"></param>
    /// <returns>True if the board can be solved without trial and error guessing.</returns>
    public static bool IsSatisfactory(Board board) // Certain methods for eliminating candidates using inference are not currently implemented. Implementing them would make this function more acurate.
    {
        // Eliminate candidates from all rules untill nothing changes.
        while (board.EliminateCandidates());
        // If the board is solved, it does not require trial and error.
        return board.IsSolved();

    }

    public static bool IsProper(Board board)
    {
        throw new NotImplementedException();
    }
}
