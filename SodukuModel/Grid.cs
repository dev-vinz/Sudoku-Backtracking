using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuModel
{
	public class Grid
	{
		public static readonly int NB_CELLS = 9;
		public static readonly int SQUARE_SIZE = 3;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private Cell[,] cells;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Grid()
		{
			cells = new Cell[NB_CELLS, NB_CELLS];
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public bool Solve(Cell[][] cells)
		{
			for (int x = 0; x < NB_CELLS; x++)
			{
				for (int y = 0; y < NB_CELLS; y++)
				{
					if (this[x, y] == null)
					{
						for (int n = 1; n <= NB_CELLS; n++)
						{
							if (IsSafe(x, y, n))
							{
								this[x, y] = n;
								if (Solve(cells)) return true;
								else this[x, y] = null;
							}
						}
						return false;
					}
				}
			}
			return true;
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
		private bool IsSafe(int row, int col, int number)
		{
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

			/* Otherwise, grid is safe */

			return true;
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
			set { cells[row, col].Value = value; }
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	}
}
