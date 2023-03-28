using SudoScript.Ast;

namespace SudoScript;

public static class Parser
{
    // Consider implementing this in a style similar to: https://github.com/frederikja163/TuringComplete
    public static ProgramNode ParseProgram(string src) => ParseProgram(src);

    public static ProgramNode ParseProgram(StreamReader reader) => ParseProgram(Tokenizer.GetStream(reader));

    public static ProgramNode ParseProgram(TokenStream stream)
    {
        throw new NotImplementedException();
    }
}
