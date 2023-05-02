using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace Yiff_Browser_WinUI3.Converters {
	public abstract class BoolToValueConverter<T> : IValueConverter {
		public T TrueValue { get; set; }
		public T FalseValue { get; set; }

		public virtual object Convert(object value, Type targetType, object parameter, string language) {
			if (value is bool b) {
				return b ? TrueValue : FalseValue;
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}


	public class BoolToStringConverter : BoolToValueConverter<string> { }
	public class BoolToNumberConverter : BoolToValueConverter<double> { }
	public class BoolToBrushConverter : BoolToValueConverter<Brush> { }
	public class BoolToThicknessConverter : BoolToValueConverter<Thickness> { }
	public class BoolToCornerRadiusConverter : BoolToValueConverter<CornerRadius> { }


}
