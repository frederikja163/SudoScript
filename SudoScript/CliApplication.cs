using SudoScript.Core;
using SudoScript.Core.Ast;
using SudoScript.Core.Data;

namespace SudoScript;

public sealed class CliApplication
{
    private Board _board;
    private CellReference _selectedCell;
    private BoardRenderer _boardRenderer;
    private CellInfoRenderer _cellInfoRenderer;

    public CliApplication(string[] args)
    {
        Console.Clear();
        Console.WriteLine(string.Join(' ', args));
        if (args.Length == 0)
        {
            throw new ArgumentException("No arguments provided, try with 'SudoScript [SudoScriptFile]'");
        }

        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        Console.CursorVisible = false;
        Console.Write("[F1 Eliminate candidates] [F2 Solve board]");
        
        using StreamReader reader = new StreamReader(args[0]);
        ProgramNode programNode = Parser.ParseProgram(reader, args[0]);
        _board = Generator.GetBoardFromAST(programNode); 
        _boardRenderer = new BoardRenderer(_board);
        _cellInfoRenderer = new CellInfoRenderer(_board,  Console.WindowWidth / 2);
        _selectedCell = (_board.MinX, _board.MinY);
        SelectedCell = _selectedCell;
    }

    public CellReference SelectedCell
    {
        get => _selectedCell;
        set
        {
            _selectedCell = value;
            _boardRenderer.ClearHighlights();
            _boardRenderer.SetHighlight(value, ConsoleHelper.HighlightedCell);
            _cellInfoRenderer.RenderedCell = _selectedCell;
            if (_cellInfoRenderer.SelectedUnit is not null)
            {
                _boardRenderer.SetHighlights(_cellInfoRenderer.SelectedUnit.References()
                    .Where(c => c != SelectedCell), ConsoleHelper.HighlightedUnit);
            }
        }
    }
    
    public bool IsRunning { get; set; }

    public void Run()
    {
        IsRunning = true;
        while (IsRunning)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            switch (keyInfo.Key)
            {
                case ConsoleKey.DownArrow:
                    SelectedCell = new CellReference(SelectedCell.X,  (int)MathF.Max(SelectedCell.Y - 1, _board.MinY));
                    break;
                case ConsoleKey.UpArrow:
                    SelectedCell = new CellReference(SelectedCell.X, (int)MathF.Min(SelectedCell.Y + 1, _board.MaxY));
                    break;
                case ConsoleKey.LeftArrow:
                    SelectedCell = new CellReference( (int)MathF.Max(SelectedCell.X - 1, _board.MinX), SelectedCell.Y);
                    break;
                case ConsoleKey.RightArrow:
                    SelectedCell = new CellReference((int)MathF.Min(SelectedCell.X + 1, _board.MaxX), SelectedCell.Y);
                    break;
                case ConsoleKey.Q:
                case ConsoleKey.Escape:
                    IsRunning = false;
                    break;
                // case ConsoleKey.D0:
                case ConsoleKey.D1:
                case ConsoleKey.D2:
                case ConsoleKey.D3:
                case ConsoleKey.D4:
                case ConsoleKey.D5:
                case ConsoleKey.D6:
                case ConsoleKey.D7:
                case ConsoleKey.D8:
                case ConsoleKey.D9:
                    int key = keyInfo.Key - ConsoleKey.D0;
                    if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift))
                    {
                        if (_board[SelectedCell.X, SelectedCell.Y].HasCandidate(key))
                        {
                            _board[SelectedCell.X, SelectedCell.Y].EliminateCandidate(key);
                        }
                        else
                        {
                            _board[SelectedCell.X, SelectedCell.Y].AddCandidate(key);
                        }
                    }
                    else
                    {
                        _board[SelectedCell.X, SelectedCell.Y].Digit = key;
                    }

                    _cellInfoRenderer.Render();
                    _boardRenderer.RenderCell(SelectedCell);
                    break;
                case ConsoleKey.Tab:
                    if (_cellInfoRenderer.SelectedUnit is not null)
                    {
                        _boardRenderer.ClearHighlights(_cellInfoRenderer.SelectedUnit.References()
                            .Where(c => c != SelectedCell));
                    }
                    _cellInfoRenderer.MoveSelection(keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : +1);
                    
                    if (_cellInfoRenderer.SelectedUnit is not null)
                    {
                        _boardRenderer.SetHighlights(_cellInfoRenderer.SelectedUnit.References()
                            .Where(c => c != SelectedCell), ConsoleHelper.HighlightedUnit);
                    }
                    break;
                // Eliminate candidates.
                case ConsoleKey.F1:
                    _board.EliminateCandidates();
                    SelectedCell = SelectedCell;
                    break;
                // Solve board.
                case ConsoleKey.F2:
                    _board = Solver.Solve(_board);
                    _boardRenderer = new BoardRenderer(_board);
                    _cellInfoRenderer = new CellInfoRenderer(_board, Console.WindowWidth / 2);
                    SelectedCell = SelectedCell;
                    break;
            }
        }
    }
}