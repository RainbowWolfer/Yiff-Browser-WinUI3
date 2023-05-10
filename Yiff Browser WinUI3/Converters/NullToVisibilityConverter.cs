using Microsoft.UI.Xaml.Data;
using System;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Converters {
	public class NullToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			return (value is null).ToVisibility();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
