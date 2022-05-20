using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GridView
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private static readonly int INNER_WIDTH = 3;
		private static readonly int OUTER_WIDTH = INNER_WIDTH * INNER_WIDTH;

		private static readonly int THIN = 1;
		private static readonly int THICK = 3;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public SudokuModel.Grid GridModel => (SudokuModel.Grid)DataContext;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public MainWindow()
		{
			InitializeComponent();
			InitializeGridModel();
			InitializeSudokuTable();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private void InitializeGridModel()
		{
			DataContext = new SudokuModel.Grid();
		}

		private void InitializeSudokuTable()
		{
			UniformGrid? grid = new UniformGrid
			{
				Rows = SudokuModel.Grid.NB_CELLS,
				Columns = SudokuModel.Grid.NB_CELLS
			};

			for (int i = 0; i < SudokuModel.Grid.NB_CELLS; i++)
			{
				for (int j = 0; j < SudokuModel.Grid.NB_CELLS; j++)
				{
					Border border = CreateBorder(i, j);
					border.Child = CreateTextBox(i, j);
					grid.Children.Add(border);
				}
			}

			SudokuTable.Child = grid;
		}

		private static Border CreateBorder(int i, int j)
		{
			int left = j % INNER_WIDTH == 0 ? THICK : THIN;
			int top = i % INNER_WIDTH == 0 ? THICK : THIN;
			int right = j == OUTER_WIDTH - 1 ? THICK : 0;
			int bottom = i == OUTER_WIDTH - 1 ? THICK : 0;

			return new Border
			{
				BorderThickness = new Thickness(left, top, right, bottom),
				BorderBrush = Brushes.Black
			};
		}

		private TextBox CreateTextBox(int i, int j)
		{
			TextBox textBox = new TextBox
			{
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				TextAlignment = TextAlignment.Center,
				VerticalContentAlignment = VerticalAlignment.Center,
				FontSize = 30
			};

			textBox.TextChanged += TextChangedEventHandler;

			SudokuBindingConverter sudokuBindingConverter = new SudokuBindingConverter();

			Binding binding = new Binding
			{
				Source = GridModel,
				Path = new PropertyPath($"[{i},{j}]"),
				Mode = BindingMode.TwoWay,
				Converter = sudokuBindingConverter
			};

			textBox.SetBinding(TextBox.TextProperty, binding);

			return textBox;
		}

		private void TextChangedEventHandler(object sender, TextChangedEventArgs args)
		{

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



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        private void solveButton_Click(object sender, RoutedEventArgs e)
        {
			// TO DO
        }
    }
}