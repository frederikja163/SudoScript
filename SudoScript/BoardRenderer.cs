using System.Text;
using SudoScript.Core.Data;

namespace SudoScript;

public sealed class BoardRenderer
{
    private readonly Board _board;
    private readonly Dictionary<CellReference, ConsoleColor> _cellColors = new Dictionary<CellReference, ConsoleColor>();

    public BoardRenderer(Board board)
    {
        _board = board;
    }

    private void RenderBoard()
    {
        Console.SetCursorPosition(0, 1);
        for (int row = _board.MaxX; row >= _board.MinX; row--)
        {
            Console.Write(row + "|");
            // TODO: Maybe cache as many cells as possible here, and write them at once with fewer calls to Console.Write();
            for (int col = _board.MinX; col <= _board.MaxX; col++)
            {
                Cell cell = _board[col, row];
                if (_cellColors.TryGetValue((col, row), out ConsoleColor backgroundColor))
                {
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = ConsoleHelper.GetContrastColor(backgroundColor);
                }
                else
                {
                    Console.ResetColor();
                }

                Console.Write((cell.Digit == Cell.EmptyDigit ? "." : cell.Digit.ToString()) + " ");
            }
            Console.ResetColor();
            Console.WriteLine();
        }

        Console.WriteLine("-+" + new string('-', (_board.MaxX - _board.MinX + 1) * 2));
        Console.Write(" |");
        for (int col = _board.MinX; col <= _board.MaxX; col++)
        {
            Console.Write(col + " ");
        }
        Console.WriteLine();
    }

    public void RenderCell(CellReference cellReference)
    {
        (int left, int top) = Console.GetCursorPosition();
        Cell cell = _board[cellReference.X, cellReference.Y];
        if (!_cellColors.TryGetValue(cellReference, out ConsoleColor backgroundColor))
        {
            backgroundColor = ConsoleColor.Black;
        }
        
        Console.SetCursorPosition(cellReference.X * 2, _board.MaxY - _board.MinY - cellReference.Y + 2);
        Console.BackgroundColor = backgroundColor;
        Console.ForegroundColor = ConsoleHelper.GetContrastColor(backgroundColor);
        Console.Write((cell.Digit == Cell.EmptyDigit ? "." : cell.Digit.ToString()) + " ");
        Console.ResetColor();
        Console.SetCursorPosition(left, top);
    }

    public void ClearHighlights()
    {
        _cellColors.Clear();
        RenderBoard();
    }

    public void SetHighlight(CellReference cellReference, ConsoleColor color = (ConsoleColor)-1)
    {
        _cellColors[cellReference] = color;
        RenderCell(cellReference);
    }
}