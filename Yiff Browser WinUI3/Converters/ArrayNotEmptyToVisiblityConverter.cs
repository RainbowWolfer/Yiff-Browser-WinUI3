using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Converters {
	public class ArrayNotEmptyToVisiblityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value == null) {
				return Visibility.Collapsed;
			}

			if (value is IEnumerable ie) {
				return ie.IsNotEmpty().ToVisibility();
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
