using SudoScript.Core.Ast;
using SudoScript.Core.Data;
using System.IO;

namespace SudoScript.Core;

public static class SudoScript
{
    public static Board Solve(string src)
    {
        return Solver.Solve(Load(src));
    }

    public static Board Load(string src)
    {
        return Generator.GetBoardFromAST(GetAst(src));
    }

    public static ProgramNode GetAst(string src)
    {
        return Parser.ParseProgram(src);
    }

    public static Board Solve(StreamReader sr)
    {
        return Solver.Solve(Load(sr));
    }

    public static Board Load(StreamReader sr)
    {
        return Generator.GetBoardFromAST(GetAst(sr));
    }

    public static ProgramNode GetAst(StreamReader sr)
    {
        return Parser.ParseProgram(sr);
    }
}
