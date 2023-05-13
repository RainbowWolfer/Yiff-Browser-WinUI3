using Microsoft.UI.Xaml.Data;
using System;
using System.Collections;
using System.Linq;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Converters {
	public abstract class UnifyArrayConverter<T> : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value is IList list) {
				return Enumerable.Range(0, list.Count).Select(i => list[i]).Cast<T>().ToArray();
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}

	public class UnifyE621PostsConverter : UnifyArrayConverter<E621Post> { }
}
