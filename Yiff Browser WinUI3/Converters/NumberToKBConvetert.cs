using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Converters {
	public class NumberToKBConvetert : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			int number = (int)CommonHelpers.Abs(value);
			return number.FileSizeToKB();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			return value;
		}
	}
}
