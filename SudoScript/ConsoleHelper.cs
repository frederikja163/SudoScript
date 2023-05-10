namespace SudoScript;

public static class ConsoleHelper
{
    
    public const ConsoleColor HighlightedCell = ConsoleColor.DarkGray;
    public const ConsoleColor HighlightedUnit = ConsoleColor.DarkRed;
    
    

    public static ConsoleColor GetContrastColor(ConsoleColor backgroundColor)
    {
        return backgroundColor switch
        {
            ConsoleColor.Black => ConsoleColor.White,
            ConsoleColor.DarkBlue => ConsoleColor.White,
            ConsoleColor.DarkGreen => ConsoleColor.White,
            ConsoleColor.DarkCyan => ConsoleColor.White,
            ConsoleColor.DarkRed => ConsoleColor.White,
            ConsoleColor.DarkMagenta => ConsoleColor.White,
            ConsoleColor.DarkYellow => ConsoleColor.White,
            ConsoleColor.Gray => ConsoleColor.Black,
            ConsoleColor.DarkGray => ConsoleColor.Black,
            ConsoleColor.Blue => ConsoleColor.Black,
            ConsoleColor.Green => ConsoleColor.Black,
            ConsoleColor.Cyan => ConsoleColor.Black,
            ConsoleColor.Red => ConsoleColor.Black,
            ConsoleColor.Magenta => ConsoleColor.Black,
            ConsoleColor.Yellow => ConsoleColor.Black,
            ConsoleColor.White => ConsoleColor.Black,
            _ => throw new ArgumentOutOfRangeException(nameof(backgroundColor), backgroundColor, null)
        };
    }
}