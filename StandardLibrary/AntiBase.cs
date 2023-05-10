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
            List<Cell> seenCells = ListSeenCells(cell, unit);
            foreach (Cell seenCell in seenCells)
            {
                if (cell.EliminateCandidate(seenCell.Digit))
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
            List<Cell> seenCells = ListSeenCells(cell, unit);
            foreach (Cell seenCell in seenCells)
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

    private List<Cell> ListSeenCells(Cell cell, Unit unit)
    {
        List<Cell> list = new List<Cell>();
        if (unit.Board is null)
        {
            throw new Exception(this.ToString() + "- unit.Board is null");
        }
        for (int i = 0; i < _coordinates.Length; i++)
        {
            if (unit.Board.Cells().Any(c => c.X == cell.X + _coordinates[i].X && c.Y == cell.Y + _coordinates[i].Y))
            {
                list.Add(unit.Board[cell.X + _coordinates[i].X, cell.Y + _coordinates[i].Y]);
            }
        }
        return list;
    }
}
