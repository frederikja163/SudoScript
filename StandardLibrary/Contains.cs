using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SudoScript.Data;

namespace StandardLibrary
{
    public class Contains : IRule
    {
        public Contains(int number)
        {
            Number = number;
        }

        internal int Number { get; private init; }
        public bool EliminateCandidates(Unit unit)
        {
            throw new NotImplementedException();
        }

        public bool ValidateRules(Unit unit)
        {
            bool contains = false;
            foreach (Cell cell in unit.Cells())
            {
                if (cell.Digit == Number)
                {
                    contains = true;
                }
            }
            return contains;
        }
    }
}
