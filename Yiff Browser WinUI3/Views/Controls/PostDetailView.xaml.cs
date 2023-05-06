using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class PostDetailView : UserControl {
		public event Action RequestBack;



		public E621Post E621Post {
			get => (E621Post)GetValue(E621PostProperty);
			set => SetValue(E621PostProperty, value);
		}

		public static readonly DependencyProperty E621PostProperty = DependencyProperty.Register(
			nameof(E621Post),
			typeof(E621Post),
			typeof(PostDetailView),
			new PropertyMetadata(null)
		);



		public string InitialImageURL {
			get => (string)GetValue(InitialImageURLProperty);
			set => SetValue(InitialImageURLProperty, value);
		}

		public static readonly DependencyProperty InitialImageURLProperty = DependencyProperty.Register(
			nameof(InitialImageURL),
			typeof(string),
			typeof(PostDetailView),
			new PropertyMetadata(string.Empty)
		);




		public PostDetailView() {
			this.InitializeComponent();
		}

		public Image GetBackgroundImage() => BackgroundImage;
		public FrameworkElement GetCurrentImage() {
			PictureDisplayView.ResetImage();
			if (ViewModel.ShowBackgroundImage) {
				return BackgroundImage;
			} else {
				return PictureDisplayView.GetTargetImage();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			RequestBack?.Invoke();
		}
	}

	public class PostDetailViewModel : BindableBase {
		private E621Post e621Post;
		private string targetURL;
		private bool isMedia;
		private int fileSize;
		private bool showBackgroundImage = true;

		public E621Post E621Post {
			get => e621Post;
			set => SetProperty(ref e621Post, value, OnPostChanged);
		}

		public string TargetURL {
			get => targetURL;
			set => SetProperty(ref targetURL, value);
		}

		public int FileSize {
			get => fileSize;
			set => SetProperty(ref fileSize, value);
		}

		public bool IsMedia {
			get => isMedia;
			set => SetProperty(ref isMedia, value);
		}

		public bool ShowBackgroundImage {
			get => showBackgroundImage;
			set => SetProperty(ref showBackgroundImage, value);
		}

		private void OnPostChanged() {
			ShowBackgroundImage = true;
			FileSize = E621Post.File.Size;
			TargetURL = E621Post.File.URL;
		}

		public ICommand OnImageLoadedCommand => new DelegateCommand(OnImageLoaded);

		private void OnImageLoaded() {
			ShowBackgroundImage = false;
		}
	}
}
