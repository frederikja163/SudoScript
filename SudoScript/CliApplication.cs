using SudoScript.Core;
using SudoScript.Core.Ast;
using SudoScript.Core.Data;

namespace SudoScript;

public sealed class CliApplication
{
    private const ConsoleColor SelectedHighlight = ConsoleColor.White;
    private readonly Board _board;
    private CellReference _selectedCell;
    private readonly BoardRenderer _boardRenderer;

    public CliApplication(string[] args)
    {
        Console.WriteLine(string.Join(' ', args));
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments provided, try with 'SudoScript [SudoScriptFile]'");
            return;
        }

        using StreamReader reader = new StreamReader(args[0]);
        ProgramNode programNode = Parser.ParseProgram(reader);
        _board = Generator.GetBoardFromAST(programNode); 
        _boardRenderer = new BoardRenderer(_board);
        SelectedCell = (1, 1);
    }

    public CellReference SelectedCell
    {
        get => _selectedCell;
        set
        {
            _selectedCell = value;
            _boardRenderer.ClearHighlights();
            _boardRenderer.SetHighlight(value, SelectedHighlight);
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
                    SelectedCell = new CellReference(SelectedCell.X, SelectedCell.Y - 1);
                    break;
                case ConsoleKey.UpArrow:
                    SelectedCell = new CellReference(SelectedCell.X, SelectedCell.Y + 1);
                    break;
                case ConsoleKey.LeftArrow:
                    SelectedCell = new CellReference(SelectedCell.X - 1, SelectedCell.Y);
                    break;
                case ConsoleKey.RightArrow:
                    SelectedCell = new CellReference(SelectedCell.X + 1, SelectedCell.Y);
                    break;
                case ConsoleKey.Escape:
                    IsRunning = false;
                    break;
            }
        }
    }
}