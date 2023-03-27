namespace SudoScript;

public sealed class Cell
{
    public Cell(int x, int y, int given)
    {
        X = x;
        Y = y;
        IsGiven = true;
        _digit = given;
        Candidates = new HashSet<int>();
    }

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        Candidates = new HashSet<int>()
        {
        1, 2, 3, 4, 5, 6, 7, 8, 9,
        };
    }

    public int X { get; }
    public int Y { get; }

    public bool IsGiven { get; }

    private int _digit = 0;

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
        }
    }

    public HashSet<int> Candidates { get; }
}
