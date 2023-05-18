using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Views.Controls.PostsView {
	public sealed partial class PostsBriefDisplayView : UserControl {


		public E621Post[] Posts {
			get => (E621Post[])GetValue(PostsProperty);
			set => SetValue(PostsProperty, value);
		}

		public static readonly DependencyProperty PostsProperty = DependencyProperty.Register(
			nameof(Posts),
			typeof(E621Post[]),
			typeof(PostsBriefDisplayView),
			new PropertyMetadata(Array.Empty<E621Post>(), OnPostsChanged)
		);

		private static void OnPostsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {

		}

		public PostsBriefDisplayView() {
			this.InitializeComponent();
		}


		public static async Task ShowAsDialog(XamlRoot xamlRoot, IEnumerable<E621Post> posts, string title = null) {
			await new PostsBriefDisplayView() {
				Posts = posts.ToArray(),
			}.CreateContentDialog(xamlRoot, new ContentDialogParameters() {
				Title = title,
				CloseText = "Back",
				SkipWidthSet = false,
			}).ShowDialogAsync();
		}
	}
}
