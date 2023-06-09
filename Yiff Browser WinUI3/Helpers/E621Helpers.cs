﻿using CommunityToolkit.WinUI.Helpers;
using Windows.UI;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Helpers {
	public static class E621Helpers {

		public static Color GetRatingColor(this E621Rating rating) {
			bool isDark = App.IsDarkTheme();
			return rating switch {
				E621Rating.Safe => (isDark ? "#008000" : "#36973E").ToColor(),
				E621Rating.Questionable => (isDark ? "#FFFF00" : "#EFC50C").ToColor(),
				E621Rating.Explict => (isDark ? "#FF0000" : "#C92A2D").ToColor(),
				_ => (isDark ? "#FFF" : "#000").ToColor(),
			};
		}

		public static string GetRatingIcon(this E621Rating rating){
			return rating switch {
				E621Rating.Safe => "\uF78C",
				E621Rating.Questionable => "\uE897",
				E621Rating.Explict => "\uE814",
				_ => "\uE8BB",
			};
		}

	}
}
