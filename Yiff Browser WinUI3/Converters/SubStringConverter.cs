using Microsoft.UI.Xaml.Data;
using System;

namespace Yiff_Browser_WinUI3.Converters {
	public class SubStringConverter : IValueConverter {
		public int Length { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language) {
			return value.ToString()[..Length];
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
