namespace SudoScript;

public sealed class Arguments
{
    public Arguments(string[] args)
    {
        if (args.Length != 0 && File.Exists(args[^1]))
        {
            Path = args[^1];
        }
        else
        {
            Help = true;
        }

        foreach (string arg in args.SkipLast(1))
        {
            switch (arg)
            {
                case "-w":
                case "--watch":
                    Watch = true;
                    break;
                case "-h":
                case "--help":
                    break;
            }
        }

        if (Help)
        {
            Console.WriteLine("USAGE\n\tSudoScript [--watch | -w] [--help | -h] [<path>]");
            Console.WriteLine("\tOpen the <path> in the TUI of SudoScript.");
            Console.WriteLine("OPTIONS");
            Console.WriteLine("\t<path>\n\tPath to open.\n");
            Console.WriteLine("\t--help, -h\n\tPrint this message.\n");
            Console.WriteLine("\t--watch, -w\n\tWatch input file.\n");
        }
    }

    public string? Path { get; }
    public bool Help { get; }
    public bool Watch { get; }
}