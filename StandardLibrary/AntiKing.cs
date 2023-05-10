using SudoScript.Core.Data;

namespace StandardLibrary;

public class AntiKing : AntiBase
{
    public AntiKing() : base(_coordinates)
    {
    }

    private static readonly CellReference[] _coordinates =
    {
        (-1, 1), 
        (0, 1),
        (1, 1),
        (1, 0),
        (-1, 0),
        (-1, -1),
        (0, -1),
        (1, -1),
    };

    public override string ToString()
    {
        return nameof(AntiKing);
    }
}
