using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Converters {
	public class ArrayToCountConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value is IEnumerable e) {
				return e.Count().ToString();
			}
			return (-1).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
