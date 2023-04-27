/* Implement a standard Sudoku, givens from:
   sudoku.com
 */

// Rows for default Sudoku.
Row 1
Row 2
Row 3
Row 4
Row 5
Row 6
Row 7
Row 8
Row 9

// Collumns for default Sudoku.
Column 1
Column 2
Column 3
Column 4
Column 5
Column 6
Column 7
Column 8
Column 9

// Boxes for default Sudoku.
Box (1, 1)
Box (4, 1)
Box (7, 1)
Box (1, 4)
Box (4, 4)
Box (7, 4)
Box (1, 7)
Box (4, 7)
Box (7, 7)

// Givens for the easy sudoku from 'sudoku.com'.
givens {
	(9, 1) 1
	(1, 2) 3
	(5, 2) 8
	(7, 2) 9
	(8, 2) 7
	(3, 3) 5
	(6, 3) 4
	(3, 4) 3
	(4, 4) 5
	(7, 4) 2
	(8, 4) 1
	(1, 5) 1
	(9, 5) 8
	(5, 6) 2
	(9, 6) 6
	(1, 7) 7
	(5, 7) 9
	(7, 7) 3
	(8, 7) 2
	(7, 8) 6
	(1, 9) 2
	(2, 9) 9
	(4, 9) 8
}