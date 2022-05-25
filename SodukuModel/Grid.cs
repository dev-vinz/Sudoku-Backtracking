using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuModel
{
	public class Grid : INotifyPropertyChanged
	{
		public static readonly int NB_CELLS = 9;
		public static readonly int SQUARE_SIZE = 3;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private Cell[,] cells;
		private bool solved;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public bool IsSolved => solved;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Grid()
		{
			cells = new Cell[NB_CELLS, NB_CELLS];
			solved = false;

			PopulateGrid();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public event PropertyChangedEventHandler? PropertyChanged;

		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public async Task SolveAsync()
        {
			int row = -1;
			int col = -1;
			bool emptyCellsLeft = false;

			/* Check if we have empty cells */

			for (int r = 0; r < NB_CELLS; r++)
			{
				for (int c = 0; c < NB_CELLS; c++)
				{
					if (this[r, c] == null)
					{
						row = r;
						col = c;

						emptyCellsLeft = true;
						break;
					}
				}

				if (emptyCellsLeft) break;
			}

			/* No empty cells left */

			if (!emptyCellsLeft)
			{
				solved = true;
				return;
			}

			/* Backtracking algorithm */

			for (int num = 1; num <= NB_CELLS; num++)
			{
				if (IsSafe(row, col, num))
				{
					this[row, col] = num;

					await Task.Delay(10); // Input ?

					await SolveAsync();

					if (solved)
					{
						return;
					}
					else
					{
						this[row, col] = null;
					}
				}
			}

			solved = false;
		}

		/// <summary>
		/// Solve the sudoku using backtracking
		/// </summary>
		/// <returns>If the grid is solved</returns>
		public void Solve()
		{
			int row = -1;
			int col = -1;
			bool emptyCellsLeft = false;

			/* Check if we have empty cells */

			for (int r = 0; r < NB_CELLS; r++)
			{
				for (int c = 0; c < NB_CELLS; c++)
				{
					if (this[r, c] == null)
					{
						row = r;
						col = c;

						emptyCellsLeft = true;
						break;
					}
				}

				if (emptyCellsLeft) break;
			}

			/* No empty cells left */

			if (!emptyCellsLeft)
			{
				solved = true;
				return;
			}

			/* Backtracking algorithm */

			for (int num = 1; num <= NB_CELLS; num++)
			{
				if (IsSafe(row, col, num))
				{
					this[row, col] = num;

					Solve();

					if (solved)
					{
						return;
					}
					else
					{
						this[row, col] = null;
					}
				}
			}

			solved = false;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <summary>
		/// Checks if the number we are trying to place is already present in row, column or box
		/// </summary>
		/// <param name="row">The row</param>
		/// <param name="col">The column</param>
		/// <param name="number">The number we are trying to place</param>
		/// <returns>If the grid is safe or not</returns>
		private bool IsSafe(int row, int col, int? number)
		{
			/* Check if the number has a value */

			if (!number.HasValue) return true; // true because null is safe (empty)

			/* Check if the value is in ]0, 9] */

			if (number < 1 || number > 9) return false;

			/* Check rows and columns, if the value is already there */

			for (int k = 0; k < NB_CELLS; k++)
			{
				if (this[row, k] == number) return false;
				if (this[k, col] == number) return false;
			}

			/* Check square boxes */

			int gridRowStart = row - row % SQUARE_SIZE;
			int gridColStart = col - col % SQUARE_SIZE;

			for (int r = gridRowStart; r < gridRowStart + SQUARE_SIZE; r++)
			{
				for (int c = gridColStart; c < gridColStart + SQUARE_SIZE; c++)
				{
					if (this[r, c] == number) return false;
				}
			}

			/* Otherwise, row, column and box is safe */

			return true;
		}

		/// <summary>
		/// Fill the grid with empty cells
		/// </summary>
		private void PopulateGrid()
		{
			for (int x = 0; x < NB_CELLS; x++)
			{
				for (int y = 0; y < NB_CELLS; y++)
				{
					cells[x, y] = new Cell();
				}
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          STATIC METHODS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         ABSTRACT METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public int? this[int row, int col]
		{
			get { return cells[row, col].Value; }
			set 
			{
				cells[row, col].Value = null;

				if (IsSafe(row, col, value))
				{
					cells[row, col].Value = value;
				}

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	}
}
