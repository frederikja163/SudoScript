using SudoScript.Core.Data;

namespace StandardLibrary;

public class AntiKnight : AntiBase
{
    public AntiKnight() : base(_coordinates)
    {
    }

    private static readonly CellReference[] _coordinates =
    {
        (2, 1),
        (1, 2),
        (2, -1),
        (1, -2),
        (-2, -1),
        (-1, -2),
        (-2, 1),
        (-1, 2),
    };

    public override string ToString()
    {
        return nameof(AntiKnight);
    }
}
