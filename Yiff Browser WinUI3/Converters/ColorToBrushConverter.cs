using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Windows.UI;

namespace Yiff_Browser_WinUI3.Converters {
	public class ColorToBrushConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value is Color color) {
				return new SolidColorBrush(color);
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
