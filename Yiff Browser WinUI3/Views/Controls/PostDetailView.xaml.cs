using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class PostDetailView : UserControl {
		public event Action RequestBack;

		public E621Post E621Post {
			get => (E621Post)GetValue(E621PostProperty);
			set {
				if (E621Post == value) {
					MediaDisplayView.Play();
				}
				SetValue(E621PostProperty, value);
			}
		}

		public static readonly DependencyProperty E621PostProperty = DependencyProperty.Register(
			nameof(E621Post),
			typeof(E621Post),
			typeof(PostDetailView),
			new PropertyMetadata(null)
		);



		public ICommand ImagesListManagerItemClickCommand {
			get => (ICommand)GetValue(ImagesListManagerItemClickCommandProperty);
			set => SetValue(ImagesListManagerItemClickCommandProperty, value);
		}

		public static readonly DependencyProperty ImagesListManagerItemClickCommandProperty = DependencyProperty.Register(
			nameof(ImagesListManagerItemClickCommand),
			typeof(ICommand),
			typeof(PostDetailView),
			new PropertyMetadata(null)
		);



		public E621Post[] PostsList {
			get => (E621Post[])GetValue(PostsListProperty);
			set => SetValue(PostsListProperty, value);
		}

		public static readonly DependencyProperty PostsListProperty = DependencyProperty.Register(
			nameof(PostsList),
			typeof(E621Post[]),
			typeof(PostDetailView),
			new PropertyMetadata(Array.Empty<E621Post>())
		);

		public bool InputByPosts {
			get => (bool)GetValue(InputByPostsProperty);
			set => SetValue(InputByPostsProperty, value);
		}

		public static readonly DependencyProperty InputByPostsProperty = DependencyProperty.Register(
			nameof(InputByPosts),
			typeof(bool),
			typeof(PostDetailView),
			new PropertyMetadata(false, OnInputByPostsChanged)
		);

		private static void OnInputByPostsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is PostDetailView view) {
				view.ViewModel.InputByPosts = (bool)e.NewValue;
			}
		}

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
			ViewModel.OnImagesListManagerItemClick += ViewModel_OnImagesListManagerItemClick;
		}

		private void ViewModel_OnImagesListManagerItemClick(E621Post post) {
			ImagesListManagerItemClickCommand?.Execute(post);
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

		private void BackButton_Click(object sender, RoutedEventArgs e) {
			MediaDisplayView.Pause();
			RequestBack?.Invoke();
		}
	}

	public class PostDetailViewModel : BindableBase {
		public event Action<E621Post> OnImagesListManagerItemClick;

		private E621Post e621Post;
		private string imageURL;
		private bool isMedia;
		private int fileSize;
		private bool showBackgroundImage = true;
		private string mediaURL;
		private bool showMoreInfoSplitView;
		private string fileTypeIcon;
		private string fileTypeToolTip;
		private string ratingToolTip;
		private string idTitle;
		private Color ratingColor;
		private bool showSounWarning;
		private Color soundWarningColor;
		private string soundWarningToolTip;
		private bool isFavoriteLoading;
		private bool hasFavorited;
		private bool ableToCopyImage;
		private bool isSidePaneOverlay = true;
		private bool inputByPosts;

		public bool IsSidePaneOverlay {
			get => isSidePaneOverlay;
			set => SetProperty(ref isSidePaneOverlay, value);
		}

		public E621Post E621Post {
			get => e621Post;
			set => SetProperty(ref e621Post, value, OnPostChanged);
		}

		public string ImageURL {
			get => imageURL;
			set => SetProperty(ref imageURL, value);
		}

		public string MediaURL {
			get => mediaURL;
			set => SetProperty(ref mediaURL, value);
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

		public bool ShowMoreInfoSplitView {
			get => showMoreInfoSplitView;
			set => SetProperty(ref showMoreInfoSplitView, value);
		}

		public string FileTypeIcon {
			get => fileTypeIcon;
			set => SetProperty(ref fileTypeIcon, value);
		}

		public string FileTypeToolTip {
			get => fileTypeToolTip;
			set => SetProperty(ref fileTypeToolTip, value);
		}

		public string RatingToolTip {
			get => ratingToolTip;
			set => SetProperty(ref ratingToolTip, value);
		}

		public string IDTitle {
			get => idTitle;
			set => SetProperty(ref idTitle, value);
		}
		public Color RatingColor {
			get => ratingColor;
			set => SetProperty(ref ratingColor, value);
		}

		public bool ShowSounWarning {
			get => showSounWarning;
			set => SetProperty(ref showSounWarning, value);
		}
		public Color SoundWarningColor {
			get => soundWarningColor;
			set => SetProperty(ref soundWarningColor, value);
		}
		public string SoundWarningToolTip {
			get => soundWarningToolTip;
			set => SetProperty(ref soundWarningToolTip, value);
		}

		public bool InputByPosts {
			get => inputByPosts;
			set => SetProperty(ref inputByPosts, value);
		}

		private void OnPostChanged() {
			ShowBackgroundImage = true;

			IDTitle = $"{E621Post.ID} ({E621Post.Rating.ToString()[..1]})";
			RatingColor = E621Post.Rating.GetRatingColor();

			FileSize = E621Post.File.Size;

			FileType type = E621Post.GetFileType();
			FileTypeIcon = type switch {
				FileType.Png or FileType.Jpg => "\uEB9F",
				FileType.Gif => "\uF4A9",
				FileType.Webm => "\uE714",
				_ => "\uE9CE",
			};
			FileTypeToolTip = $"Type: {type}";

			RatingToolTip = $"Rating: {E621Post.Rating}";

			List<string> tags = E621Post.Tags.GetAllTags();
			if (tags.Contains("sound_warning")) {
				ShowSounWarning = true;
				SoundWarningColor = E621Rating.Explict.GetRatingColor();
				SoundWarningToolTip = "This Video Has 'sound_warning' Tag";
			} else if (tags.Contains("sound")) {
				ShowSounWarning = true;
				SoundWarningColor = E621Rating.Questionable.GetRatingColor();
				SoundWarningToolTip = "This Video Has 'sound' Tag";
			} else {
				ShowSounWarning = false;
			}

			AbleToCopyImage = false;

			switch (type) {
				case FileType.Png:
				case FileType.Jpg:
				case FileType.Gif:
					ImageURL = E621Post.File.URL;
					MediaURL = string.Empty;
					IsMedia = false;
					break;
				case FileType.Webm:
					ImageURL = string.Empty;
					MediaURL = E621Post.File.URL;
					IsMedia = true;
					break;
				case FileType.Anim:
					//display error
					break;
				default: throw new NotSupportedException();
			}
		}

		public bool AbleToCopyImage {
			get => ableToCopyImage;
			set => SetProperty(ref ableToCopyImage, value);
		}

		public ICommand OnImageLoadedCommand => new DelegateCommand<BitmapImage>(OnImageLoaded);

		private void OnImageLoaded(BitmapImage image) {
			if (image != null) {
				ShowBackgroundImage = false;
				AbleToCopyImage = true;
			} else {

			}
		}

		public bool IsFavoriteLoading {
			get => isFavoriteLoading;
			set => SetProperty(ref isFavoriteLoading, value);
		}

		public bool HasFavorited {
			get => hasFavorited;
			set => SetProperty(ref hasFavorited, value, OnHasFavoritedChanged);
		}

		private async void OnHasFavoritedChanged() {
			if (IsFavoriteLoading) {
				return;
			}
			IsFavoriteLoading = true;

			await Task.Delay(3000);

			IsFavoriteLoading = false;
		}

		public ICommand OpenMoreInfoCommand => new DelegateCommand(OpenMoreInfo);

		private void OpenMoreInfo() {
			ShowMoreInfoSplitView = !ShowMoreInfoSplitView;
		}

		public ICommand CopyIDCommand => new DelegateCommand(CopyID);

		private void CopyID() {
			E621Post.ID.ToString().CopyToClipboard();
		}

		public ICommand OpenInBrowserCommand => new DelegateCommand(OpenInBrowser);

		private void OpenInBrowser() {
			@$"https://e621.net/posts/{E621Post.ID}".OpenInBrowser();
		}

		public ICommand CopyImageCommand => new DelegateCommand(CopyImage);

		private void CopyImage() {
			if (!AbleToCopyImage) {
				return;
			}

			DataPackage imageDataPackage = new() {
				RequestedOperation = DataPackageOperation.Copy,
			};

			imageDataPackage.SetBitmap(RandomAccessStreamReference.CreateFromUri(new Uri(E621Post.File.URL)));
			Clipboard.SetContent(imageDataPackage);
		}

		public ICommand ImagesListManagerItemClickCommand => new DelegateCommand<E621Post>(ImagesListManagerItemClick);

		private void ImagesListManagerItemClick(E621Post post) {
			OnImagesListManagerItemClick?.Invoke(post);
		}
	}
}
