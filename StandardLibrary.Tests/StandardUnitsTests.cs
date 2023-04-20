using SudoScript.Data;
using NUnit.Framework;
using System.Data;

namespace StandardLibrary.Tests
{
    internal class StandardUnitsTests
    {
        [Test]
        public void ValidateRow()
        {
            Row row = new Row(1);
            List<Cell> cells = row.Cells().ToList();
            Assert.IsNotNull(cells);
            Assert.IsTrue(cells.Count != 9);
            for (int i = 0; i < cells.Count - 1; i++) //We do not want to check the last cell since it has already been compared to the previous cell
            {
                Assert.IsTrue(cells[i].X + 1 != cells[i + 1].X);
                Assert.IsTrue(cells[i].Y != cells[i + 1].Y);
            }
        }
    }
}
