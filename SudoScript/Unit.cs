﻿using System.Collections;

namespace SudoScript;

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

    public Board Board { get; internal set; }

    public IEnumerable<Cell> Cells()
    {
        foreach (CellReference cell in _cells)
        {
            yield return Board[cell.X, cell.Y];
        }
    }

    public void EliminateCandidates()
    {
        foreach (IRule rule in _rules)
        {
            rule.EliminateCandidates(this);
        }
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
