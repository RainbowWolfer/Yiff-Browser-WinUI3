using Microsoft.UI.Xaml.Data;
using System;

namespace Yiff_Browser_WinUI3.Converters {
	public class NullToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			return value is null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
