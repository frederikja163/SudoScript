using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudoScript;

public sealed record CellReference(int X, int Y)
{
    public static implicit operator (int X, int Y)(CellReference cellReference) => (cellReference.X, cellReference.Y);
    public static implicit operator CellReference((int X, int Y) cellReference) => new CellReference(cellReference.X, cellReference.Y);
}
