using System.Diagnostics;
using System.Text;
using SudoScript.Core.Data;

namespace SudoScript;

public sealed class BoardRenderer
{
    private readonly Board _board;
    private readonly Dictionary<CellReference, ConsoleColor> _cellColors = new();

    public BoardRenderer(Board board)
    {
        _board = board;
    }

    private void RenderBoard()
    {
        Console.SetCursorPosition(0, 0);
        for (int row = _board.MaxY; row >= _board.MinY; row--)
        {
            Console.Write(row + "|");
            // TODO: Maybe cache as many cells as possible here, and write them at once with fewer calls to Console.Write();
            for (int col = _board.MinX; col <= _board.MaxX; col++)
            {
                if (_cellColors.TryGetValue((col, row), out ConsoleColor backgroundColor))
                {
                    Console.BackgroundColor = backgroundColor;
                    Console.ForegroundColor = ConsoleHelper.GetContrastColor(backgroundColor);
                }
                else
                {
                    Console.ResetColor();
                }

                Console.Write(_board.TryGetCell(col, row, out Cell? cell) ? cell.ToString("Digit ") : "  ");
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
        if (_cellColors.TryGetValue(cellReference, out ConsoleColor backgroundColor))
        {
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = ConsoleHelper.GetContrastColor(backgroundColor);
        }
        else
        {
            Console.ResetColor();
        }
        Console.SetCursorPosition((cellReference.X - _board.MinX) * 2 + 2, _board.MaxY - _board.MinY - (cellReference.Y - _board.MinY));
        Console.Write(_board.TryGetCell(cellReference.X, cellReference.Y, out Cell? cell) ? cell.ToString("Digit ") : "  ");
        
        Console.ResetColor();
    }

    public void ClearHighlights()
    {
        _cellColors.Clear();
        RenderBoard();
    }

    public void SetHighlight(CellReference cellReference, ConsoleColor color)
    {
        _cellColors[cellReference] = color;
        RenderCell(cellReference);
    }

    public void ClearHighlight(CellReference cellReference)
    {
        if (_cellColors.ContainsKey(cellReference))
        {
            _cellColors.Remove(cellReference);
        }
        RenderCell(cellReference);
    }

    public void SetHighlights(IEnumerable<CellReference> references, ConsoleColor color)
    {
        foreach (CellReference cellReference in references)
        {
            SetHighlight(cellReference, color);
        }
    }
    
    public void ClearHighlights(IEnumerable<CellReference> references)
    {
        foreach (CellReference reference in references)
        {
            ClearHighlight(reference);
        }
    }
}