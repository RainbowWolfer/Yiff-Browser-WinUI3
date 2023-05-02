using Microsoft.UI;
using System;
using System.Globalization;
using System.Reflection;
using Windows.UI;

namespace Yiff_Browser_WinUI3.Helpers {
	public static class ColorHelper {
		//
		// Summary:
		//     Creates a Windows.UI.Color from a XAML color string. Any format used in XAML
		//     should work.
		//
		// Parameters:
		//   colorString:
		//     The XAML color string.
		//
		// Returns:
		//     The created Windows.UI.Color.
		public static Color ToColor(this string colorString) {
			if (string.IsNullOrEmpty(colorString)) {
				ThrowArgumentException();
			}

			if (colorString[0] == '#') {
				switch (colorString.Length) {
					case 9: {
						uint num4 = Convert.ToUInt32(colorString[1..], 16);
						byte a = (byte)(num4 >> 24);
						byte r2 = (byte)((num4 >> 16) & 0xFFu);
						byte g2 = (byte)((num4 >> 8) & 0xFFu);
						byte b9 = (byte)(num4 & 0xFFu);
						return Color.FromArgb(a, r2, g2, b9);
					}
					case 7: {
						uint num3 = Convert.ToUInt32(colorString[1..], 16);
						byte r = (byte)((num3 >> 16) & 0xFFu);
						byte g = (byte)((num3 >> 8) & 0xFFu);
						byte b8 = (byte)(num3 & 0xFFu);
						return Color.FromArgb(byte.MaxValue, r, g, b8);
					}
					case 5: {
						ushort num2 = Convert.ToUInt16(colorString[1..], 16);
						byte b4 = (byte)(num2 >> 12);
						byte b5 = (byte)((uint)(num2 >> 8) & 0xFu);
						byte b6 = (byte)((uint)(num2 >> 4) & 0xFu);
						byte b7 = (byte)(num2 & 0xFu);
						b4 = (byte)((b4 << 4) | b4);
						b5 = (byte)((b5 << 4) | b5);
						b6 = (byte)((b6 << 4) | b6);
						b7 = (byte)((b7 << 4) | b7);
						return Color.FromArgb(b4, b5, b6, b7);
					}
					case 4: {
						ushort num = Convert.ToUInt16(colorString[1..], 16);
						byte b = (byte)((uint)(num >> 8) & 0xFu);
						byte b2 = (byte)((uint)(num >> 4) & 0xFu);
						byte b3 = (byte)(num & 0xFu);
						b = (byte)((b << 4) | b);
						b2 = (byte)((b2 << 4) | b2);
						b3 = (byte)((b3 << 4) | b3);
						return Color.FromArgb(byte.MaxValue, b, b2, b3);
					}
					default:
						return ThrowFormatException();
				}
			}

			if (colorString.Length > 3 && colorString[0] == 's' && colorString[1] == 'c' && colorString[2] == '#') {
				string[] array = colorString.Split(',');
				if (array.Length == 4) {
					double num5 = double.Parse(array[0][3..], CultureInfo.InvariantCulture);
					double num6 = double.Parse(array[1], CultureInfo.InvariantCulture);
					double num7 = double.Parse(array[2], CultureInfo.InvariantCulture);
					double num8 = double.Parse(array[3], CultureInfo.InvariantCulture);
					return Color.FromArgb((byte)(num5 * 255.0), (byte)(num6 * 255.0), (byte)(num7 * 255.0), (byte)(num8 * 255.0));
				}

				if (array.Length == 3) {
					double num9 = double.Parse(array[0][3..], CultureInfo.InvariantCulture);
					double num10 = double.Parse(array[1], CultureInfo.InvariantCulture);
					double num11 = double.Parse(array[2], CultureInfo.InvariantCulture);
					return Color.FromArgb(byte.MaxValue, (byte)(num9 * 255.0), (byte)(num10 * 255.0), (byte)(num11 * 255.0));
				}

				return ThrowFormatException();
			}

			PropertyInfo declaredProperty = typeof(Colors).GetTypeInfo().GetDeclaredProperty(colorString);
			if (declaredProperty != null) {
				return (Color)declaredProperty.GetValue(null);
			}

			return ThrowFormatException();
			static void ThrowArgumentException() {
				throw new ArgumentException("The parameter \"colorString\" must not be null or empty.");
			}

			static Color ThrowFormatException() {
				throw new FormatException("The parameter \"colorString\" is not a recognized Color format.");
			}
		}

		//
		// Summary:
		//     Converts a Windows.UI.Color to a hexadecimal string representation.
		//
		// Parameters:
		//   color:
		//     The color to convert.
		//
		// Returns:
		//     The hexadecimal string representation of the color.
		public static string ToHex(this Color color) {
			return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
		}

		//
		// Summary:
		//     Converts a Windows.UI.Color to a premultiplied Int32 - 4 byte ARGB structure.
		//
		// Parameters:
		//   color:
		//     The color to convert.
		//
		// Returns:
		//     The int representation of the color.
		public static int ToInt(this Color color) {
			int num = color.A + 1;
			return (color.A << 24) | ((byte)(color.R * num >> 8) << 16) | ((byte)(color.G * num >> 8) << 8) | (byte)(color.B * num >> 8);
		}

		//
		// Summary:
		//     Converts a Windows.UI.Color to an Microsoft.Toolkit.Uwp.HslColor.
		//
		// Parameters:
		//   color:
		//     The Windows.UI.Color to convert.
		//
		// Returns:
		//     The converted Microsoft.Toolkit.Uwp.HslColor.
		public static HslColor ToHsl(this Color color) {
			double num = 0.00392156862745098 * (double)(int)color.R;
			double num2 = 0.00392156862745098 * (double)(int)color.G;
			double num3 = 0.00392156862745098 * (double)(int)color.B;
			double num4 = Math.Max(Math.Max(num, num2), num3);
			double num5 = Math.Min(Math.Min(num, num2), num3);
			double num6 = num4 - num5;
			double num7 = ((num6 == 0.0) ? 0.0 : ((num4 == num) ? (((num2 - num3) / num6 + 6.0) % 6.0) : ((num4 != num2) ? (4.0 + (num - num2) / num6) : (2.0 + (num3 - num) / num6))));
			double num8 = 0.5 * (num4 + num5);
			double s = ((num6 == 0.0) ? 0.0 : (num6 / (1.0 - Math.Abs(2.0 * num8 - 1.0))));
			HslColor result = default;
			result.H = 60.0 * num7;
			result.S = s;
			result.L = num8;
			result.A = 0.00392156862745098 * (double)(int)color.A;
			return result;
		}

		//
		// Summary:
		//     Converts a Windows.UI.Color to an Microsoft.Toolkit.Uwp.HsvColor.
		//
		// Parameters:
		//   color:
		//     The Windows.UI.Color to convert.
		//
		// Returns:
		//     The converted Microsoft.Toolkit.Uwp.HsvColor.
		public static HsvColor ToHsv(this Color color) {
			double num = 0.00392156862745098 * (double)(int)color.R;
			double num2 = 0.00392156862745098 * (double)(int)color.G;
			double num3 = 0.00392156862745098 * (double)(int)color.B;
			double num4 = Math.Max(Math.Max(num, num2), num3);
			double num5 = Math.Min(Math.Min(num, num2), num3);
			double num6 = num4 - num5;
			double num7 = ((num6 == 0.0) ? 0.0 : ((num4 == num) ? (((num2 - num3) / num6 + 6.0) % 6.0) : ((num4 != num2) ? (4.0 + (num - num2) / num6) : (2.0 + (num3 - num) / num6))));
			double s = ((num6 == 0.0) ? 0.0 : (num6 / num4));
			HsvColor result = default;
			result.H = 60.0 * num7;
			result.S = s;
			result.V = num4;
			result.A = 0.00392156862745098 * (double)(int)color.A;
			return result;
		}

		//
		// Summary:
		//     Creates a Windows.UI.Color from the specified hue, saturation, lightness, and
		//     alpha values.
		//
		// Parameters:
		//   hue:
		//     0..360 range hue
		//
		//   saturation:
		//     0..1 range saturation
		//
		//   lightness:
		//     0..1 range lightness
		//
		//   alpha:
		//     0..1 alpha
		//
		// Returns:
		//     The created Windows.UI.Color.
		public static Color FromHsl(double hue, double saturation, double lightness, double alpha = 1.0) {
			if (hue < 0.0 || hue > 360.0) {
				throw new ArgumentOutOfRangeException(nameof(hue));
			}

			double num = (1.0 - Math.Abs(2.0 * lightness - 1.0)) * saturation;
			double num2 = hue / 60.0;
			double num3 = num * (1.0 - Math.Abs(num2 % 2.0 - 1.0));
			double num4 = lightness - 0.5 * num;
			double num5;
			double num6;
			double num7;
			if (num2 < 1.0) {
				num5 = num;
				num6 = num3;
				num7 = 0.0;
			} else if (num2 < 2.0) {
				num5 = num3;
				num6 = num;
				num7 = 0.0;
			} else if (num2 < 3.0) {
				num5 = 0.0;
				num6 = num;
				num7 = num3;
			} else if (num2 < 4.0) {
				num5 = 0.0;
				num6 = num3;
				num7 = num;
			} else if (num2 < 5.0) {
				num5 = num3;
				num6 = 0.0;
				num7 = num;
			} else {
				num5 = num;
				num6 = 0.0;
				num7 = num3;
			}

			byte r = (byte)(255.0 * (num5 + num4));
			byte g = (byte)(255.0 * (num6 + num4));
			byte b = (byte)(255.0 * (num7 + num4));
			return Color.FromArgb((byte)(255.0 * alpha), r, g, b);
		}

		//
		// Summary:
		//     Creates a Windows.UI.Color from the specified hue, saturation, value, and alpha
		//     values.
		//
		// Parameters:
		//   hue:
		//     0..360 range hue
		//
		//   saturation:
		//     0..1 range saturation
		//
		//   value:
		//     0..1 range value
		//
		//   alpha:
		//     0..1 alpha
		//
		// Returns:
		//     The created Windows.UI.Color.
		public static Color FromHsv(double hue, double saturation, double value, double alpha = 1.0) {
			if (hue < 0.0 || hue > 360.0) {
				throw new ArgumentOutOfRangeException(nameof(hue));
			}

			double num = value * saturation;
			double num2 = hue / 60.0;
			double num3 = num * (1.0 - Math.Abs(num2 % 2.0 - 1.0));
			double num4 = value - num;
			double num5;
			double num6;
			double num7;
			if (num2 < 1.0) {
				num5 = num;
				num6 = num3;
				num7 = 0.0;
			} else if (num2 < 2.0) {
				num5 = num3;
				num6 = num;
				num7 = 0.0;
			} else if (num2 < 3.0) {
				num5 = 0.0;
				num6 = num;
				num7 = num3;
			} else if (num2 < 4.0) {
				num5 = 0.0;
				num6 = num3;
				num7 = num;
			} else if (num2 < 5.0) {
				num5 = num3;
				num6 = 0.0;
				num7 = num;
			} else {
				num5 = num;
				num6 = 0.0;
				num7 = num3;
			}

			byte r = (byte)(255.0 * (num5 + num4));
			byte g = (byte)(255.0 * (num6 + num4));
			byte b = (byte)(255.0 * (num7 + num4));
			return Color.FromArgb((byte)(255.0 * alpha), r, g, b);
		}
	}

	public struct HsvColor {
		//
		// Summary:
		//     The Hue in 0..360 range.
		public double H;

		//
		// Summary:
		//     The Saturation in 0..1 range.
		public double S;

		//
		// Summary:
		//     The Value in 0..1 range.
		public double V;

		//
		// Summary:
		//     The Alpha/opacity in 0..1 range.
		public double A;
	}

	public struct HslColor {
		//
		// Summary:
		//     The Hue in 0..360 range.
		public double H;

		//
		// Summary:
		//     The Saturation in 0..1 range.
		public double S;

		//
		// Summary:
		//     The Lightness in 0..1 range.
		public double L;

		//
		// Summary:
		//     The Alpha/opacity in 0..1 range.
		public double A;
	}
}
