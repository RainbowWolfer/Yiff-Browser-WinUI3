using Microsoft.UI.Xaml.Data;
using System;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Converters {
	public class E621RatingToIconConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value is E621Rating rating) {
				return E621Helpers.GetRatingIcon(rating);
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
