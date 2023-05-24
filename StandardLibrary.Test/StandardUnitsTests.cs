using NUnit.Framework;
using SudoScript.Core.Data;

namespace StandardLibrary.Test;

internal class StandardUnitsTests
{
    [Test]
    public void ValidateRow()
    {
        Row row = new Row(1);
        List<Cell> cells = new List<Cell>
        {
            new Cell(1,1),
            new Cell(2,1),
            new Cell(3,1),
            new Cell(4,1),
            new Cell(5,1),
            new Cell(6,1),
            new Cell(7,1),
            new Cell(8,1),
            new Cell(9,1)
        };
        Board board = new Board(cells, new List<Unit>() { row });
        List<Cell> rowCells = row.Cells().ToList();
        Assert.IsNotNull(rowCells);
        Assert.IsNotEmpty(rowCells);
        Assert.IsTrue(rowCells.Count == 9);
        for (int i = 0; i < rowCells.Count - 1; i++) //We do not want to check the last cell since it has already been compared to the previous cell
        {
            Assert.IsTrue(rowCells[i].X != rowCells[i + 1].X);
            Assert.IsTrue(rowCells[i].Y == rowCells[i + 1].Y);
        }
    }

    [Test]
    public void ValidateColumn()
    {
        Column column = new Column(1);
        List<Cell> cells = new List<Cell>
        {
            new Cell(1,1),
            new Cell(1,2),
            new Cell(1,3),
            new Cell(1,4),
            new Cell(1,5),
            new Cell(1,6),
            new Cell(1,7),
            new Cell(1,8),
            new Cell(1,9)
        };
        Board board = new Board(cells, new List<Unit>() { column});
        List<Cell> columnCells = column.Cells().ToList();
        Assert.IsNotNull(columnCells);
        Assert.IsNotEmpty(columnCells);
        Assert.IsTrue(columnCells.Count == 9);
        for (int i = 0; i < columnCells.Count - 1; i++) //We do not want to check the last cell since it has already been compared to the previous cell
        {
            Assert.IsTrue(columnCells[i].Y != columnCells[i + 1].Y);
            Assert.IsTrue(columnCells[i].X == columnCells[i + 1].X);
        }
    }
    [Test]
    public void ValidateBox()
    {
        Box box = new Box(1,1);
        List<Cell> cells = new List<Cell>
        {
            new Cell(1,1),
            new Cell(1,2),
            new Cell(1,3),
            new Cell(2,1),
            new Cell(2,2),
            new Cell(2,3),
            new Cell(3,1),
            new Cell(3,2),
            new Cell(3,3)
        };
        Board board = new Board(cells, new List<Unit>() { box });
        List<Cell> boxCells = box.Cells().ToList();
        Assert.IsNotNull(boxCells);
        Assert.IsNotEmpty(boxCells);
        Assert.IsTrue(boxCells.Count == 9);
        for (int i = 0; i < boxCells.Count - 1; i++) //We do not want to check the last cell since it has already been compared to the previous cell
        {
            Assert.IsTrue(boxCells[i].Y != boxCells[i + 1].Y || boxCells[i].X != boxCells[i+1].X);
        }
    }
}