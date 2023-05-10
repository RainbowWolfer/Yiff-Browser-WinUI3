using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Converters {
	public class E621RatingToBrushConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value is E621Rating rating) {
				return new SolidColorBrush(E621Helpers.GetRatingColor(rating));
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
