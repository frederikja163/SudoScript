using SudoScript.Core.Data;

namespace StandardLibrary;

internal sealed class BattenburgRule : IRule
{

    public bool EliminateCandidates(Unit unit)
    {
        throw new NotImplementedException();
    }

    public bool ValidateRules(Unit unit)
    {
        if (unit is not Battenburg battenBurg)
        {
            throw new Exception();
        }

        if (unit.Board is null)
        {
            throw new NullReferenceException(unit.ToString() + " finds Board to be null");
        }
        int topLeft = battenBurg.TopLeft.Digit;
        int topRight = battenBurg.TopRight.Digit;
        int bottomLeft = battenBurg.BottomLeft.Digit;
        int bottomRight = battenBurg.BottomRight.Digit;

        return DigitsSameOrZero(bottomLeft, topRight) && DigitsSameOrZero(bottomRight,topLeft) && 
            DigitsDifferentOrZero(bottomLeft,bottomRight) && DigitsDifferentOrZero(bottomLeft,topLeft) &&
            DigitsDifferentOrZero(topRight,topLeft) && DigitsDifferentOrZero(topRight,bottomRight);
    }

    private bool DigitsSameOrZero(int a, int b)
    {
        if (a == Cell.EmptyDigit || b == Cell.EmptyDigit)
        {
            return true;
        }
        return a % 2 == b % 2;
    }
    private bool DigitsDifferentOrZero(int a, int b)
    {
        if (a == Cell.EmptyDigit || b == Cell.EmptyDigit)
        {
            return true;
        }
        return a % 2 != b % 2;
    }
}


