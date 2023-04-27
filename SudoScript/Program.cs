﻿using SudoScript.Core;
using SudoScript.Core.Ast;
using SudoScript.Core.Data;

namespace SudoScript;

internal static class Program
{
    internal static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No arguments provided, try with 'dotnet run SudoScript [SudoScriptFile]'");
            return;
        }

        using StreamReader reader = new StreamReader(args[0]);
        ProgramNode programNode = Parser.ParseProgram(reader);
        Board board = Generator.GetBoardFromAST(programNode);
        Console.WriteLine("Unsolved board.");
        Console.WriteLine(board.ToString());
        Board solvedBoard = Solver.Solve(board);
        Console.WriteLine("Solved board.");
        Console.WriteLine(solvedBoard.ToString());
    }
} 