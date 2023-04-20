﻿namespace SudoScript.Data;

public class Unit
{
    private readonly List<CellReference> _cells;
    private readonly List<IRule> _rules;

    public Unit(List<CellReference> cells, List<IRule> rules)
    {
        _cells = cells;
        _rules = rules;
    }

    public Unit() : this(new  List<CellReference>(), new List<IRule>())
    {

    }

    internal Unit(Unit unit)
    {
        _cells = unit._cells;
        _rules = unit._rules;
    }

    public void AddRule(IRule rule)
    {
        _rules.Add(rule);
    }

    public void AddRule(IEnumerable<IRule> rules)
    {
        _rules.AddRange(rules);
    }

    public void AddCell(CellReference cell)
    {
        _cells.Add(cell);
    }

    public void AddUnit(Unit unit)
    {
        _cells.AddRange(unit._cells);
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

    public IEnumerable<CellReference> References()
    {
        return _cells;
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
}
