using SudoScript.Core.Data;

namespace SudoScript;

public sealed class CellInfoRenderer
{
    private CellReference? _renderedCell;
    private readonly Board _board;
    private readonly int _width;
    private readonly Dictionary<CellReference, List<Unit>> _cellsToUnits = new Dictionary<CellReference, List<Unit>>();
    private int _selectedUnit;

    public CellInfoRenderer(Board board, int width)
    {
        _board = board;
        _width = width;

        foreach (Unit unit in _board.Units)
        {
            if (!unit.Rules().Any())
            {
                continue;
            }
            
            foreach (Cell cell in unit.Cells())
            {
                CellReference reference = (cell.X, cell.Y);
                if (!_cellsToUnits.TryGetValue(reference, out List<Unit>? units))
                {
                    units = new List<Unit>();
                    _cellsToUnits.Add(reference, units);
                }
                if (!units.Contains(unit)) {
                    units.Add(unit);
                }
            }
        }
    }

    public CellReference? RenderedCell
    {
        get => _renderedCell;
        set
        {
            _selectedUnit = -1;
            _renderedCell = value;
            Render();
        }
    }

    private List<Unit> SelectedUnits
    {
        get
        {
            if (_renderedCell is null)
            {
                return new List<Unit>();
            }
            
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
    
    public Unit? SelectedUnit
    {
        get => SelectedUnits.Any() && _selectedUnit != -1 ? SelectedUnits[_selectedUnit] : null;
    }

    public void MoveSelection(int delta)
    {
        _selectedUnit = (int)MathF.Min(SelectedUnits.Count - 1, MathF.Max(_selectedUnit + delta, -1));
        Render();
    }
    
    public void Render()
    {
        int line = 0;
        
        // Clear if no cell is being rendered.
        if (_renderedCell is null)
        {
            ClearRemaining();
            return;
        }

        if (!_board.TryGetCell(_renderedCell, out Cell? cell))
        {
            WriteLine("No cell selected.");
            ClearRemaining();
            return;
        }
        
        WriteLine($"Digit: {(cell.Digit == Cell.EmptyDigit ? " " : cell.Digit)}");
        WriteLine($"Is Given: {(cell.IsGiven ? "[X]" : "[ ]")}");
        WriteLine($"Candidates: {{{string.Join(", ", cell.Candidates())}}}");

        for (int i = 0; i < SelectedUnits.Count; i++)
        {
            Unit unit = SelectedUnits[i];   
            if (i == _selectedUnit)
            {
                Console.BackgroundColor = ConsoleHelper.HighlightedUnit;
                Console.ForegroundColor = ConsoleHelper.GetContrastColor(ConsoleHelper.HighlightedUnit);
            }
            WriteLine(unit.ToString());
            if (i == _selectedUnit)
            {
                Console.ResetColor();
            }
        }

        ClearRemaining();

        void WriteLine(string message)
        {
            string[] lines = message.Split('\n');
            foreach (string str in lines.Select(s => s.Replace("\r", "")))
            {
                if (line + 3 >= Console.WindowHeight)
                {
                    return;
                }

                int tabCount = 0;
                while (tabCount < str.Length && str[tabCount] == '\t')
                {
                    tabCount += 1;
                }
                string msg = str.Replace("\t", "    ");
                
                Console.SetCursorPosition(Console.WindowWidth - _width, line++);
                if (_width < msg.Length)
                {
                    Console.WriteLine(msg[0.._width]);
                    WriteLine(new string('\t', tabCount) + msg[_width..]);
                }
                else
                {
                    Console.WriteLine(msg + new string(' ', _width - msg.Length));
                }
            }
        }

        void ClearRemaining()
        {
            while (line + 3 < Console.WindowHeight)
            {
                WriteLine("");
            }
        }
    }
}