using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
		private Stopwatch chrono;
		private int sleepMs;
		public event PropertyChangedEventHandler? PropertyChanged;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public bool IsSolved => solved;

		public int SleepMs
        {
			get => sleepMs;
			set => sleepMs = value;
        }

        public string ElapsedTime
		{
			get
			{
				TimeSpan ts = chrono.Elapsed;
				return String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, 
																	ts.Minutes, 
																	ts.Seconds,
																	ts.Milliseconds / 10);
			}
		}


        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public Grid()
		{
			cells = new Cell[NB_CELLS, NB_CELLS];
			solved = false;
			chrono = new Stopwatch();

			PopulateGrid();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void Clear()
		{
			chrono.Reset();
			for (int r = 0; r < NB_CELLS; r++)
			{
				for (int c = 0; c < NB_CELLS; c++)
				{
					this[r, c] = null;

				}
			}
		}

		public void GenerateRandom(int maxGeneratedNum = 10)
        {
			if (maxGeneratedNum >= NB_CELLS * NB_CELLS) return;

			Clear();
			
			Random rnd = new Random();

			int r, c;

			for(int i = 0; i < maxGeneratedNum; i++)
			{
				/* Found an empty cell */

				do
				{
					r = rnd.Next(0, NB_CELLS);
					c = rnd.Next(0, NB_CELLS);

				} while (this[r, c] != null);

				/* Found a valid number for this cell */

				do
				{
					this[r, c] = rnd.Next(1, NB_CELLS + 1);

				} while (this[r, c] == null);
			}
		}

		public async Task SolveAsync(CancellationTokenSource token)
        {
			if(token.IsCancellationRequested)
            {
				Clear();
				return;
            }

			if (!chrono.IsRunning) chrono.Start();

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
				chrono.Stop();
				return;
			}

			/* Backtracking algorithm */

			for (int num = 1; num <= NB_CELLS; num++)
			{
				if (IsSafe(row, col, num))
				{
					this[row, col] = num;

					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ElapsedTime"));

					await Task.Delay(SleepMs); // Input ?
					Console.WriteLine(SleepMs);

					await SolveAsync(token);

					if (solved || token.IsCancellationRequested)
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
			if (!chrono.IsRunning) chrono.Start();

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
				chrono.Stop();
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ElapsedTime"));
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
			get => cells[row, col].Value;
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
