using System;
using System.Globalization;
using Xamarin.Forms;

namespace NewsApp.Converters
{
	public class DateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return string.Empty;
			var dateValue = (DateTime)value;
			return $"{dateValue.Date.DayOfWeek.ToString()}, {dateValue.Date.Day.ToString()}, {dateValue:MMMM}, {dateValue.Date.Year}";

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Empty;
		}
	}
}