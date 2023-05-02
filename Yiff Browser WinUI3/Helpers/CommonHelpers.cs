using Microsoft.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Yiff_Browser_WinUI3.Helpers {
	public static class CommonHelpers {
		public static int Count(this IEnumerable ie) {
			if (ie == null) {
				return 0;
			}
			int count = 0;
			IEnumerator enumerator = ie.GetEnumerator();
			enumerator.Reset();
			while (enumerator.MoveNext()) {
				count = checked(count + 1);
			}

			return count;
		}

		public static bool IsEmpty(this IEnumerable ie) {
			if (ie == null) {
				return true;
			}
			IEnumerator enumerator = ie.GetEnumerator();
			enumerator.Reset();
			return !enumerator.MoveNext();
		}

		public static bool IsNotEmpty(this IEnumerable ie) => !ie.IsEmpty();


		public static bool IsEmpty<T>(this IEnumerable<T> ie) => ie == null || !ie.Any();
		public static bool IsNotEmpty<T>(this IEnumerable<T> ie) => !ie.IsEmpty();

		public static bool IsBlank(this string str) => string.IsNullOrWhiteSpace(str);
		public static bool IsNotBlank(this string str) => !str.IsBlank();

		public static string NotBlankCheck(this string text) => text.IsBlank() ? null : text;

		public static Visibility ToVisibility(this bool b, bool reverse = false) {
			if (reverse) {
				return b ? Visibility.Collapsed : Visibility.Visible;
			} else {
				return b ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		public static string NumberToK(this int number) {
			if (number > 1000) {
				int a = number / 1000;
				int length = $"{number}".Length;
				int pow = (int)Math.Pow(10, length - 1);
				int head = int.Parse($"{number}".First().ToString());
				int b = (number - pow * head) / (pow / 10);
				if (b == 0) {
					return $"{a}K";
				} else {
					return $"{a}.{b}K";
				}
			} else {
				return $"{number}";
			}
		}
	}
}
