using SudoScript.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardLibrary
{
    public abstract class AntiBase : IRule
    {
        public bool EliminateCandidates(Unit unit)
        {
            throw new NotImplementedException();
        }

        public bool ValidateRules(Unit unit)
        {
            throw new NotImplementedException();
        }
    }
}
