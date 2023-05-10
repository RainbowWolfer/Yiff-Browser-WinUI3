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
		public event Action<ImageViewItem, ImageViewItemViewModel> ImageClick;

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

		public bool ShowPostInfo {
			get => (bool)GetValue(ShowPostInfoProperty);
			set => SetValue(ShowPostInfoProperty, value);
		}

		public static readonly DependencyProperty ShowPostInfoProperty = DependencyProperty.Register(
			nameof(ShowPostInfo),
			typeof(bool),
			typeof(ImageViewItem),
			new PropertyMetadata(true)
		);

		public ICommand Command {
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public static readonly DependencyProperty CommandProperty =DependencyProperty.Register(
			nameof(Command),
			typeof(ICommand),
			typeof(ImageViewItem),
			new PropertyMetadata(null)
		);

		public ImageViewItem() {
			this.InitializeComponent();
			TypeHintBorder.Translation += new Vector3(0, 0, 32);
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			ImageClick?.Invoke(this, ViewModel);
			Command?.Execute(e);
		}

		public Image GetSampleImage() => SampleImage;
		public Image GetPreviewImage() => PreviewImage;

		public Image GetCurrentImage() {
			if (!ViewModel.HidePreviewImage) {
				return GetPreviewImage();
			} else {
				return GetSampleImage();
			}
		}
	}

	public class ImageViewItemViewModel : BindableBase {
		private E621Post post;
		private string typeHint;

		private LoadStage imageLoadStage = LoadStage.None;

		private string previewImageURL;
		private string sampleImageURL;
		private string errorLoadingHint;
		private bool hidePreviewImage;

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

		public LoadStage ImageLoadStage {
			get => imageLoadStage;
			set => SetProperty(ref imageLoadStage, value);
		}

		public string ErrorLoadingHint {
			get => errorLoadingHint;
			set => SetProperty(ref errorLoadingHint, value);
		}

		public bool HidePreviewImage {
			get => hidePreviewImage;
			set => SetProperty(ref hidePreviewImage, value);
		}

		private void OnPostChanged() {
			if (new string[] { "gif", "webm", "swf" }.Contains(Post.File.Ext.ToLower())) {
				TypeHint = Post.File.Ext.ToUpper();
			}
			if (Post.HasNoValidURLs()) {
				ErrorLoadingHint = "Try login to show this post";
			} else {
				PreviewImageURL = Post.Preview.URL;
			}
		}

		public ICommand OnPreviewLoadedCommand => new DelegateCommand(OnPreviewLoaded);
		public ICommand OnSampleLoadedCommand => new DelegateCommand(OnSampleLoaded);
		public ICommand OpenInBrowserCommand => new DelegateCommand(OpenInBrowser);
		public ICommand OpenCommand => new DelegateCommand(Open);

		private void OnPreviewLoaded() {
			SampleImageURL = Post.Sample.URL;
			ImageLoadStage = LoadStage.Preview;
		}

		private void OnSampleLoaded() {
			ImageLoadStage = LoadStage.Sample;
			HidePreviewImage = true;
		}

		private void OpenInBrowser() {
			@$"https://e621.net/posts/{Post.ID}".OpenInBrowser();
		}

		private void Open() {

		}

	}

	public enum LoadStage {
		None, Preview, Sample
	}
}
