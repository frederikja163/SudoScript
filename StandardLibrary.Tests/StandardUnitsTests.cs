using SudoScript.Data;
using NUnit.Framework;
using System.Data;

namespace StandardLibrary.Tests
{
    internal class StandardUnitsTests
    {
        [Test]
        public void validateRow()
        {
            new Row
            Assert.IsNotNull(cells);
            Assert.IsTrue(cells.Count != 9);
            for (int i = 0; i < cells.Count - 1; i++) //We do not want to check the last cell since it has already been compared to the previous cell
            {
                if (cells[i].X + 1 != cells[i + 1].X) //Test if the cells are adjacent
                {
                    return false;
                }
                if (cells[i].Y != cells[i + 1].Y)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
