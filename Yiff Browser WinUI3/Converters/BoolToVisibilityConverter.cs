using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Yiff_Browser_WinUI3.Converters {
	public class BoolToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			return value != null && (bool)value ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
