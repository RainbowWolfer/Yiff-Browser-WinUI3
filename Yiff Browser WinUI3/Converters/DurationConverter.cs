﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;
using System.Globalization;

namespace Yiff_Browser_WinUI3.Converters {
	public class DurationConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, string language) {
			if (value == null) {
				return value;
			}

			string raw = value.ToString();
			if (double.TryParse(raw, out double seconds)) {
				TimeSpan result = TimeSpan.FromSeconds(seconds);
				if (result.Minutes < 1) {
					string minSec = $"{result.Seconds}s";
					return minSec;
				} else {
					string minSec = $"{result.Minutes}m:{result.Seconds}s";
					return minSec;
				}
			}

			return raw;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) {
			throw new NotSupportedException();
		}
	}
}
