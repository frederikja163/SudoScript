namespace SudoScript;

internal static class Program
{
    internal static void Main(string[] args)
    {
        CliApplication app = new CliApplication(args);
        app.Run();
    }
} 