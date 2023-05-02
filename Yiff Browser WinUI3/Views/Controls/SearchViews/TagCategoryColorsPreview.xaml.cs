using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Views.Controls.SearchViews {
	public sealed partial class TagCategoryColorsPreview : UserControl {
		public TagCategoryColorsPreview() {
			this.InitializeComponent();
			bool isDark = App.GetApplicationTheme() == ApplicationTheme.Dark;
			foreach (var item in new Dictionary<Rectangle, E621TagCategory>() {
				{ Artists, E621TagCategory.Artists },
				{ Copyrights, E621TagCategory.Copyrights },
				{ Species, E621TagCategory.Species },
				{ Characters, E621TagCategory.Characters },
				{ General, E621TagCategory.General },
				{ Meta, E621TagCategory.Meta },
				{ Invalid, E621TagCategory.Invalid },
				{ Lore, E621TagCategory.Lore },
			}) {
				item.Key.Fill = new SolidColorBrush(E621Tag.GetCatrgoryColor(item.Value, isDark));
			}
		}
	}
}
