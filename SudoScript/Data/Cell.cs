namespace SudoScript.Data;

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

    public void EliminateCandidate(IEnumerable<int> candidates)
    {

        _candidates.RemoveWhere(c => candidates.Contains(c));
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
        return Digit.ToString();
    }
}
