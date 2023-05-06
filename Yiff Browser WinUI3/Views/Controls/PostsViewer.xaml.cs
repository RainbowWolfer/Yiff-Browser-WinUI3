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
using System.Collections.ObjectModel;
using Yiff_Browser_WinUI3.Models.E621;
using System.Collections.Specialized;
using System.Diagnostics;
using Prism.Mvvm;
using Yiff_Browser_WinUI3.Services.Networks;
using System.Windows.Input;
using Prism.Commands;
using Yiff_Browser_WinUI3.Helpers;
using Microsoft.UI.Xaml.Media.Animation;
using Yiff_Browser_WinUI3.Services.Locals;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public delegate void OnPreviewsUpdateEventHandler(object sender, OnPreviewsUpdateEventArgs e);

	public class OnPreviewsUpdateEventArgs : RoutedEventArgs {
		public string[] PreviewURLs { get; set; }

		public OnPreviewsUpdateEventArgs(string[] previewURLs) : base() {
			PreviewURLs = previewURLs;
		}

	}

	public sealed partial class PostsViewer : UserControl {
		public event OnPreviewsUpdateEventHandler OnPreviewsUpdated;

		public int ItemWidth { get; } = 380;
		public int ItemHeight { get; } = 50;


		public E621PostParameters Parameters {
			get => (E621PostParameters)GetValue(MyPropertyProperty);
			set => SetValue(MyPropertyProperty, value);
		}

		public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
			nameof(Parameters),
			typeof(E621PostParameters),
			typeof(PostsViewer),
			new PropertyMetadata(null, OnParametersChanged)
		);

		private static void OnParametersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not PostsViewer view) {
				return;
			}
			E621PostParameters value = (E621PostParameters)e.NewValue;
			view.ViewModel.Tags = value.Tags;
			view.ViewModel.Page = value.Page;
		}

		public PostsViewer() {
			this.InitializeComponent();
			ViewModel.PostsCollectionChanged += Posts_CollectionChanged;
			ViewModel.OnPreviewsUpdated += (s, e) => OnPreviewsUpdated?.Invoke(s, e);
		}

		private void Posts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Reset) {
				MainGrid.Children.Clear();
			} else if (e.Action == NotifyCollectionChangedAction.Add) {
				foreach (E621Post post in e.NewItems.Cast<E621Post>()) {
					AddPost(post);
				}
			} else if (e.Action == NotifyCollectionChangedAction.Remove) {
				List<ImageViewItem> views = new();
				foreach (E621Post post in e.OldItems.Cast<E621Post>()) {
					foreach (ImageViewItem view in MainGrid.Children.Cast<ImageViewItem>()) {
						if (view.Post == post) {
							views.Add(view);
						}
					}
				}
				foreach (ImageViewItem view in views) {
					MainGrid.Children.Remove(view);
				}
			} else {
				throw new NotSupportedException();
			}
		}

		private void AddPost(E621Post post) {
			ImageViewItem view = new() {
				Margin = new Thickness(5),
				Post = post,
			};
			view.OnPostDeleted += () => {
				ViewModel.Posts.Remove(post);
			};
			view.ImageClick += View_ImageClick;

			double ratio = post.File.Width / (double)post.File.Height;
			double h = (ItemWidth / ratio) / ItemHeight;
			int h2 = (int)Math.Ceiling(h);

			VariableSizedWrapGrid.SetRowSpan(view, h2);
			VariableSizedWrapGrid.SetColumnSpan(view, 1);

			MainGrid.Children.Add(view);
		}

		private ImageViewItem openedImageItem;
		private void View_ImageClick(ImageViewItem view, ImageViewItemViewModel viewModel) {
			if (openedImageItem != null) {
				return;
			}

			openedImageItem = view;
			PostDetailView.Visibility = Visibility.Visible;

			PostDetailView.E621Post = view.Post;
			PostDetailView.InitialImageURL = viewModel.ImageLoadStage switch {
				LoadStage.None or LoadStage.Preview => view.Post.Preview.URL,
				LoadStage.Sample => view.Post.Sample.URL,
				_ => throw new NotSupportedException(),
			};

			if (Local.Settings.EnanbleTransitionAnimation) {
				ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("image", view.GetCurrentImage());
				ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("image");
				imageAnimation?.TryStart(PostDetailView.GetCurrentImage());
			}

		}

		private void PostDetailView_RequestBack() {
			if (openedImageItem == null) {
				return;
			}

			PostDetailView.Visibility = Visibility.Collapsed;

			if (Local.Settings.EnanbleTransitionAnimation) {
				ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("image", PostDetailView.GetCurrentImage());
				ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("image");
				imageAnimation?.TryStart(openedImageItem.GetCurrentImage());
			}

			openedImageItem = null;
		}

		private void PageForwardButton_Click(object sender, RoutedEventArgs e) {
			PageFlyout.Hide();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e) {
			ViewModel.XamlRoot = XamlRoot;
		}
	}

	public class PostsViewerViewModel : BindableBase {
		public XamlRoot XamlRoot { get; set; }

		public event NotifyCollectionChangedEventHandler PostsCollectionChanged;
		public event OnPreviewsUpdateEventHandler OnPreviewsUpdated;

		private int pageValue;
		private bool isLoading;
		private bool enablePreviousPageButton;
		private int page = -1;
		private string[] tags = { "" };

		private bool isPostsInfoPaneOpen;
		private bool isInSelectionMode;

		public int PageValue {
			get => pageValue;
			set => SetProperty(ref pageValue, value);
		}

		public bool IsLoading {
			get => isLoading;
			set => SetProperty(ref isLoading, value);
		}

		public bool EnablePreviousPageButton {
			get => enablePreviousPageButton;
			set => SetProperty(ref enablePreviousPageButton, value);
		}

		public string[] Tags {
			get => tags;
			set => SetProperty(ref tags, value);
		}

		public int Page {
			get => page;
			set => SetProperty(ref page, value, OnPageChanged);
		}

		public bool IsInSelectionMode {
			get => isInSelectionMode;
			set => SetProperty(ref isInSelectionMode, value);
		}

		public ObservableCollection<E621Post> Posts { get; } = new ObservableCollection<E621Post>();

		#region Posts Info

		public bool IsPostsInfoPaneOpen {
			get => isPostsInfoPaneOpen;
			set => SetProperty(ref isPostsInfoPaneOpen, value);
		}

		public ICommand PostsInfoButtonCommand => new DelegateCommand(PostsInfoButton);
		public ICommand TagsInfoButtonCommand => new DelegateCommand(TagsInfoButton);

		private void PostsInfoButton() {
			IsPostsInfoPaneOpen = true;
		}

		private async void TagsInfoButton() {
			await new ContentDialog() {
				XamlRoot = XamlRoot,
				Style = App.DialogStyle,
				Title = Tags.ToFullString(),
				CloseButtonText = "Back",

			}.ShowAsync();
		}

		#endregion


		public PostsViewerViewModel() {
			Posts.CollectionChanged += (s, e) => PostsCollectionChanged?.Invoke(s, e);
		}

		private void OnPageChanged() {
			Refresh();
			EnablePreviousPageButton = Page > 1;
		}

		public ICommand RefreshCommand => new DelegateCommand(Refresh);

		public ICommand PreviousPageCommand => new DelegateCommand(PreviousPage);
		public ICommand NextPageCommand => new DelegateCommand(NextPage);

		public ICommand ForwardPageCommand => new DelegateCommand(ForwardPage);

		public ICommand DownloadCommand => new DelegateCommand(Download);

		private void Download() {

		}


		private async void Refresh() {
			if (IsLoading) {
				return;
			}

			IsLoading = true;

			Posts.Clear();
			E621Post[] posts;
			try {
				posts = await E621API.GetE621PostsByTagsAsync(new E621PostParameters() {
					Page = Page,
					Tags = Tags,
				});
			} catch {
				posts = Array.Empty<E621Post>();
			}
			if (posts != null) {
				foreach (E621Post post in posts) {
					Posts.Add(post);
				}
				string[] previews = Posts.Select(x => x.Sample.URL).Where(x => x.IsNotBlank()).Take(5).ToArray();
				OnPreviewsUpdated?.Invoke(this, new OnPreviewsUpdateEventArgs(previews));
			}

			IsLoading = false;
		}

		private void PreviousPage() {
			if (IsLoading) {
				return;
			}
			Page = Math.Clamp(Page - 1, 1, 999);
		}

		private void NextPage() {
			if (IsLoading) {
				return;
			}
			Page = Math.Clamp(Page + 1, 1, 999);
		}

		public void ForwardPage() {
			if (IsLoading) {
				return;
			}
			Page = Math.Clamp(PageValue, 1, 999);
		}
	}
}
