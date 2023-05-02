using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Prism.Mvvm;
using Yiff_Browser_WinUI3.Models.E621;
using Windows.System;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Yiff_Browser_WinUI3.Helpers;
using System.Windows.Input;
using Prism.Commands;
using System.Numerics;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class ImageViewItem : UserControl {

		public event Action OnPostDeleted;

		public E621Post Post {
			get => (E621Post)GetValue(MyPropertyProperty);
			set => SetValue(MyPropertyProperty, value);
		}

		public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
			nameof(Post),
			typeof(E621Post),
			typeof(ImageViewItem),
			new PropertyMetadata(null, OnPostChanged)
		);

		private static void OnPostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not ImageViewItem view) {
				return;
			}
			if (e.NewValue is E621Post post) {
				view.ViewModel.Post = post;
			}
		}

		public ImageViewItem() {
			this.InitializeComponent();
			TypeHintBorder.Translation += new Vector3(0, 0, 32);
		}

	}

	public class ImageViewItemViewModel : BindableBase {
		private E621Post post;
		private string typeHint;
		private bool showBetterImage;

		private string previewImageURL;
		private string sampleImageURL;

		public string PreviewImageURL {
			get => previewImageURL;
			set => SetProperty(ref previewImageURL, value.NotBlankCheck() ?? "");
		}

		public string SampleImageURL {
			get => sampleImageURL;
			set => SetProperty(ref sampleImageURL, value);
		}

		public E621Post Post {
			get => post;
			set => SetProperty(ref post, value, OnPostChanged);
		}

		public string TypeHint {
			get => typeHint;
			set => SetProperty(ref typeHint, value);
		}

		public bool ShowBetterImage {
			get => showBetterImage;
			set => SetProperty(ref showBetterImage, value);
		}

		private void OnPostChanged() {
			if (new string[] { "gif", "webm", "swf" }.Contains(Post.file.ext.ToLower())) {
				TypeHint = Post.file.ext.ToUpper();
			}
			if (post.HasNoValidURLs()) {

			} else {
				PreviewImageURL = post.preview.url;
			}
		}

		public ICommand OnPreviewLoadedCommand => new DelegateCommand(OnPreviewLoaded);
		public ICommand OpenInBrowserCommand => new DelegateCommand(OpenInBrowser);
		public ICommand OpenCommand => new DelegateCommand(Open);

		private void OnPreviewLoaded() {
			ShowBetterImage = true;
			SampleImageURL = post.sample.url;
		}

		private async void OpenInBrowser() {
			bool result = await Launcher.LaunchUriAsync(new Uri(@$"https://e621.net/posts/{Post.id}"));
			if (!result) {
				//show error
			}
		}

		private void Open() {

		}

	}
}