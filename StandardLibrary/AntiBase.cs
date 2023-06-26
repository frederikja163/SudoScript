using SudoScript.Core.Data;

namespace StandardLibrary;

public abstract class AntiBase : IRule
{
    protected AntiBase(CellReference[] coordinates)
    {
        _coordinates = coordinates;
    }

    public bool EliminateCandidates(Unit unit)
    {
        bool somethingEliminated = false;
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit != Cell.EmptyDigit)
            {
                continue;
            }
            
            foreach (Cell visibleCell in GetVisibleCells(cell, unit))
            {
                if (cell.EliminateCandidate(visibleCell.Digit))
                {
                    somethingEliminated = true;
                }
            }
        }
        return somethingEliminated;
    }

    public bool ValidateRules(Unit unit)
    {
        foreach (Cell cell in unit.Cells())
        {
            if (cell.Digit == Cell.EmptyDigit)
            {
                continue;
            }
            IEnumerable<Cell> visibleCells = GetVisibleCells(cell, unit);
            foreach (Cell seenCell in visibleCells)
            {
                if (cell.Digit == seenCell.Digit)
                {
                    return false;
                }
            }
        }
        return true;
    }
    private readonly CellReference[] _coordinates;

    private IEnumerable<Cell> GetVisibleCells(Cell cell, Unit unit)
    {
        if (unit.Board is null)
        {
            throw new Exception(this + "- unit.Board is null");
        }
        for (int i = 0; i < _coordinates.Length; i++)
        {
            if (unit.Board.TryGetCell(_coordinates[i].X + cell.X, _coordinates[i].Y + cell.Y, out Cell? visibleCell))
            {
                yield return visibleCell;
            }
        }
    }
}
