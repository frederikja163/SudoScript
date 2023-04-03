namespace SudoScript.Data;

public class Unit
{
    private readonly IReadOnlyList<CellReference> _cells;
    private readonly IReadOnlyList<IRule> _rules;

    public Unit(IReadOnlyList<CellReference> cells, IReadOnlyList<IRule> rules)
    {
        _cells = cells;
        _rules = rules;
    }

    public Unit(Unit unit)
    {
        _cells = unit._cells;
        _rules = unit._rules;
    }

    public Board? Board { get; internal set; }

    public IEnumerable<Cell> Cells()
    {
        if (Board is null)
        {
            yield break;
        }

        foreach (CellReference cell in _cells)
        {
            yield return Board[cell.X, cell.Y];
        }
    }

    public bool EliminateCandidates()
    {
        bool somethingEliminated = false;
        foreach (IRule rule in _rules)
        {
            if (rule.EliminateCandidates(this))
            {
                somethingEliminated = true;
            }
        }
        return somethingEliminated;
    }

    public bool ValidateRules()
    {
        foreach (IRule rule in _rules)
        {
            if (!rule.ValidateRules(this))
            {
                return false;
            }
        }
        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Unit unit &&
            _cells.Equals(unit._cells);
    }
}
