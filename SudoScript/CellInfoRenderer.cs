using SudoScript.Core.Data;

namespace SudoScript;

public sealed class CellInfoRenderer
{
    private CellReference? _renderedCell = null;
    private readonly Board _board;
    private readonly int _width;
    private Dictionary<CellReference, List<Unit>> _cellsToUnits = new Dictionary<CellReference, List<Unit>>();
    private int _selectedUnit = 0;

    public CellInfoRenderer(Board board, int width)
    {
        _board = board;
        _width = width;

        foreach (Unit unit in _board.Units)
        {
            foreach (Cell cell in unit.Cells())
            {
                CellReference reference = (cell.X, cell.Y);
                if (!_cellsToUnits.TryGetValue(reference, out List<Unit>? units))
                {
                    units = new List<Unit>();
                    _cellsToUnits.Add(reference, units);
                }
                units.Add(unit);
            }
        }
    }

    public CellReference? RenderedCell
    {
        get => _renderedCell;
        set
        {
            _selectedUnit = 0;
            _renderedCell = value;
            Render();
        }
    }

    private void ClearFrom(int line)
    {
        for (int i = 0; i < line; i++)
        {
            Console.SetCursorPosition(Console.WindowWidth - _width, line);
            Console.WriteLine(new string(' ', _width));
        }
    }

    private List<Unit> SelectedUnits
    {
        get
        {
            // Get the units affecting this cell.
            // Empty list in case there are no rules.
            if (!_cellsToUnits.TryGetValue(_renderedCell, out List<Unit>? units))
            {
                units = new List<Unit>();
                _cellsToUnits.Add(_renderedCell, units);
            }

            return units;
        }
    }
    
    public Unit SelectedUnit
    {
        get => SelectedUnits[_selectedUnit];
    }

    public void MoveSelection(int delta)
    {
        _selectedUnit = (int)MathF.Min(SelectedUnits.Count - 1, MathF.Max(_selectedUnit + delta, 0));
        Render();
    }
    
    public void Render()
    {
        // Clear if no cell is being rendered.
        if (_renderedCell is null)
        {
            ClearFrom(0);
            return;
        }
        
        Cell cell = _board[_renderedCell.X, _renderedCell.Y];
        int line = 0;
        WriteLine($"Digit: {(cell.Digit == Cell.EmptyDigit ? " " : cell.Digit)}");
        WriteLine($"Is Given: {(cell.IsGiven ? "[X]" : "[ ]")}");
        WriteLine($"Candidates: {{{string.Join(", ", cell.Candidates())}}}");

        for (int i = 0; i < SelectedUnits.Count; i++)
        {
            Unit unit = SelectedUnits[i];
            // Only display units with rules.
            if (!unit.Rules().Any())
            {
                continue;
            }

            if (i == _selectedUnit)
            {
                Console.BackgroundColor = ConsoleHelper.HighlightedUnit;
                Console.ForegroundColor = ConsoleHelper.GetContrastColor(ConsoleHelper.HighlightedUnit);
            }
            WriteLine($"Unit: {{");
            WriteLine($"  {string.Join(", ", unit.References())}");
            WriteLine($"  Rules: {{{string.Join(", ", unit.Rules().Select(r => r.GetType().Name))}}}");
            WriteLine($"}}");
            if (i == _selectedUnit)
            {
                Console.ResetColor();
            }
        }

        void WriteLine(string message)
        {
            Console.SetCursorPosition(Console.WindowWidth - _width, line++);
            if (_width < message.Length)
            {
                Console.WriteLine(message[0.._width]);
                WriteLine("  " + message[_width..]);
            }
            else
            {
                Console.WriteLine(message + new string(' ', _width - message.Length));
            }
        }
    }
}