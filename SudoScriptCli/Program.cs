using SudoScript;
using SudoScript.Ast;
using SudoScript.Data;

namespace SudoScriptCli;

internal static class Program
{
    internal static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments provided, try with 'dotnet run SudoScriptCli [SudoScriptFile]'");
            return;
        }

        using StreamReader reader = new StreamReader(args[0]);
        ProgramNode programNode = Parser.ParseProgram(reader);
        Board board = Generator.GetBoardFromAST(programNode);
        Board solvedBoard = Solver.Solve(board);

        // Somehow visualize the board.
    }
} 