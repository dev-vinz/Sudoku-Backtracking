using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GridView
{
    public class SudokuBindingConverter : IValueConverter
	{
		public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            try
            {
				int? iValue = (int?)value;
				if (iValue.HasValue) return iValue.ToString();
				else return null;
			}
            catch
            {
				return null;
            }
			
		}

		public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			String str = (String)value;
			if (str == "") return null;
			else
			{
				try
				{
					return System.Convert.ToInt32(str);
				}
				catch
				{
					return null;
				}
			}
		}
	}
}
