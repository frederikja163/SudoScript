namespace SudoScript.Core.Data;

public sealed class Cell
{
    public const int EmptyDigit = 0;
    private readonly HashSet<int> _candidates;
    
    public Cell(int x, int y, int given)
    {
        X = x;
        Y = y;
        IsGiven = true;
        _digit = given;
        _candidates = new HashSet<int>() { _digit };
    }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        _digit = EmptyDigit;
        _candidates = new HashSet<int>()
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9,
        };
    }

    public Cell(Cell cell)
    {
        X = cell.X;
        Y = cell.Y;
        _digit = cell.Digit;
        IsGiven = cell.IsGiven;
        _candidates = cell._candidates.ToHashSet();
    }

    public int X { get; }
    public int Y { get; }

    public bool IsGiven { get; }

    private int _digit;

    public int Digit
    {
        get => _digit;
        set
        {
            if (IsGiven)
            {
                throw new InvalidOperationException("Cannot set the digit of a cell that is a given.");
            }
            _digit = value;
            if (value != EmptyDigit)
            {
                _candidates.Clear();
                _candidates.Add(value);
            }
        }
    }


    public int CandidateCount => _candidates.Count;

    public bool EliminateCandidate(IEnumerable<int> candidates)
    {
        bool somethingEliminated = false;
        foreach (int candidate in candidates)
        {
            if (_candidates.Contains(candidate))
            {
                somethingEliminated = true;
                _candidates.Remove(candidate);
            }
        }
        if (_candidates.Count == 1)
        {
            Digit = _candidates.First();
        }
        else if (_candidates.Count != 1)
        {
            Digit = EmptyDigit;
        }
        return somethingEliminated;
    }
    public bool EliminateCandidate(int candidate)
    {
        bool somethingEliminated = false;
        if (_candidates.Contains(candidate))
        {
            somethingEliminated = true;
            _candidates.Remove(candidate);
        }
        if (_candidates.Count == 1)
        {
            Digit = _candidates.First();
        }
        else if (_candidates.Count != 1)
        {
            Digit = EmptyDigit;
        }
        return somethingEliminated;
    }

    public bool EliminateCandidate(Func<int, bool> predicate)
    {
        return EliminateCandidate(_candidates.Where(predicate));
    }

    public void AddCandidate(int candidate)
    {
        _candidates.Add(candidate);
        if (_candidates.Count == 1)
        {
            Digit = _candidates.First();
        }
        else if (_candidates.Count != 1)
        {
            Digit = EmptyDigit;
        }
    }

    public bool HasCandidate(int candidate)
    {
        return _candidates.Contains(candidate);
    }

    public IEnumerable<int> Candidates()
    {
        return _candidates;
    }

    public override string ToString()
    {
        return ToString($"(X, Y) Digit");
    }

    public string ToString(string format)
    {
        return format.Replace("Digit", _digit == EmptyDigit ? "." : _digit.ToString())
            .Replace("X", X.ToString())
            .Replace("Y", Y.ToString());
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + X.GetHashCode();
            hash = hash * 23 + Y.GetHashCode();
            hash = hash * 23 + Digit.GetHashCode();
            return hash;
        }
    }

}
