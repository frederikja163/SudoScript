using SudoScript.Data;
using System.Diagnostics.CodeAnalysis;

namespace SudoScript;

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
        // Take all cells with the least amount of candidates.
        orderedCells = orderedCells.TakeWhile(c => c.CandidateCount == lowestCandidateCount);
        // If there are no cells with more than 1 candidate, the board is solved.
        if (lowestCandidateCount == 1)
        {
            solvedBoard = board;
            return true;
        }
        // TODO: Try with randomized list.

        // Choose one of the cells.
        Cell cell = orderedCells.First();

        // Create a clone of the board for backtracking.
        // TODO: Implement backtracking later.

        // Collapse the cell with a digit.
        int digit = cell.Candidates().First();
        cell.Digit = digit;

        // Eliminate candidates from all rules untill nothing changes.
        while (board.EliminateCandidates()) ;

        // We hit an invalid state, and must backtrack.
        if (!board.ValidateRules())
        {
            solvedBoard = null;
            return false;
        }

        // Call solve on the new board.
        return SolveRec(board, out solvedBoard);
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
