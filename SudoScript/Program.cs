namespace SudoScript;

internal static class Program
{
    internal static void Main(string[] args)
    {
        Arguments arguments = new Arguments(args);
        CliApplication app = new CliApplication(arguments);
        app.Run();
    }
} 