using NUnit.Framework;
using SudoScript.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;

namespace StandardLibrary.Test
{
    internal sealed class AntiKnightTests
    {
        [Test]
        public void ValidAnitKnightUnitTest()
        {
            Unit unit = new Unit(new List<CellReference>
        {
            (1, 1),
            (2, 2),
            (3, 3),
            (4, 4),
            (5, 5),
            (6, 6),
            (7, 7),
        }, new List<IRule>
        {
            new AntiKnight()
        }) ;

            Board board = Util.CreateStandardEmpty(unit);
            board[1, 1].Digit = 1;
            board[2, 2].Digit = 2;
            board[4, 4].Digit = 4;
            board[5, 5].Digit = 5;
            board[7, 7].Digit = 7;

            board[1, 4].Digit = 5;
            board[3, 4].Digit = 7;
            board[4, 3].Digit = 1;
            board[3, 6].Digit = 9;
            board[6, 3].Digit = 3;
            board[7, 4].Digit = 2;
            board[7, 6].Digit = 1;
            board[6, 7].Digit = 1;
            board[4, 7].Digit = 2;

            Assert.IsTrue(unit.ValidateRules());
            Assert.IsTrue(board.ValidateRules());
            Assert.DoesNotThrow(() => unit.EliminateCandidates());
        }

        [Test]
        public void InvalidAnitKnightUnitTest()
        {
            Unit unit = new Unit(new List<CellReference>
        {
            (1, 1),
            (2, 2),
            (3, 3),
            (4, 4),
            (5, 5),
            (6, 6),
            (7, 7),
        }, new List<IRule>
        {
            new AntiKnight()
        });

            Board board = Util.CreateStandardEmpty(unit);
            board[1, 1].Digit = 1;
            board[2, 2].Digit = 2;
            board[4, 4].Digit = 4;
            board[5, 5].Digit = 5;
            board[7, 7].Digit = 7;

            board[1, 4].Digit = 5;
            board[3, 4].Digit = 7;
            board[4, 3].Digit = 1;
            board[3, 6].Digit = 9;
            board[6, 3].Digit = 3;
            board[7, 4].Digit = 7;
            board[7, 6].Digit = 5;
            board[6, 7].Digit = 1;
            board[4, 7].Digit = 2;

            Assert.IsFalse(unit.ValidateRules());
            Assert.IsFalse(board.ValidateRules());
            Assert.DoesNotThrow(() => unit.EliminateCandidates());
        }

        [Test]
        public void EliminateCandidatesTest()
        {
            Unit unit = new Unit(new List<CellReference>
        {
            (1, 1),
            (2, 2),
            (3, 3),
            (4, 4),
            (5, 5),
            (6, 6),
            (7, 7),
        }, new List<IRule>
        {
            new AntiKnight()
        });

            Board board = Util.CreateStandardEmpty(unit);
            board[1, 1].Digit = 1;
            board[2, 2].Digit = 2;
            board[4, 4].Digit = 4;
            board[5, 5].Digit = 5;
            board[7, 7].Digit = 7;

            board[1, 4].Digit = 5;
            board[3, 4].Digit = 7;
            board[4, 3].Digit = 1;
            board[3, 6].Digit = 9;
            board[6, 3].Digit = 3;
            board[7, 4].Digit = 7;
            board[7, 6].Digit = 5;
            board[6, 7].Digit = 1;
            board[4, 7].Digit = 2;

            Assert.IsTrue(board[3, 3].Candidates().Contains(5));
            Assert.IsTrue(board[6, 6].Candidates().Contains(2));
            unit.EliminateCandidates();
            Assert.IsFalse(board[3, 3].Candidates().Contains(5));
            Assert.IsFalse(board[6, 6].Candidates().Contains(2));
        }
    }
}
