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
        // Eliminate candidates from all rules untill nothing changes.
        while (board.EliminateCandidates()) ;

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
