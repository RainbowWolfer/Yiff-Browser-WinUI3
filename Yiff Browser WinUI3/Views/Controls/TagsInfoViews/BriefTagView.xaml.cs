using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Views.Controls.TagsInfoViews {
	public sealed partial class BriefTagView : UserControl {
		public E621Tag E621Tag {
			get => (E621Tag)GetValue(MyPropertyProperty);
			set => SetValue(MyPropertyProperty, value);
		}

		public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
			nameof(E621Tag),
			typeof(E621Tag),
			typeof(BriefTagView),
			new PropertyMetadata(null, OnE621TagChanged)
		);

		private static void OnE621TagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is BriefTagView view && e.NewValue is E621Tag value) {
				view.TitleText.Text = value.name;
				view.PostCountText.Text = value.post_count.NumberToK();
				view.CategoryText.Text = E621Tag.GetCategory(value.category);
				view.CategoryRectangle.Fill = new SolidColorBrush(E621Tag.GetCatrgoryColor(value.category, App.GetApplicationTheme() == ApplicationTheme.Dark));
			}
		}

		public BriefTagView() {
			this.InitializeComponent();
		}
	}
}
