using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;
using Yiff_Browser_WinUI3.Services.Locals;
using Yiff_Browser_WinUI3.Services.Networks;
using Yiff_Browser_WinUI3.Views.Controls.PictureViews;
using Yiff_Browser_WinUI3.Views.Controls.TagsInfoViews;

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

		public PostsViewerViewModel ViewModel { get; private set; }


		public ICommand OnPreviewsUpdatedCommand {
			get => (ICommand)GetValue(OnPreviewsUpdatedCommandProperty);
			set => SetValue(OnPreviewsUpdatedCommandProperty, value);
		}

		public static readonly DependencyProperty OnPreviewsUpdatedCommandProperty = DependencyProperty.Register(
			nameof(OnPreviewsUpdatedCommand),
			typeof(ICommand),
			typeof(PostsViewer),
			new PropertyMetadata(null)
		);

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

			if (view.ViewModel != null) {
				view.ViewModel.PostsCollectionChanged -= view.Posts_CollectionChanged;
				view.ViewModel.OnPreviewsUpdated -= view.ViewModel_OnPreviewsUpdated;
				view.ViewModel.OnScrollReset -= view.ViewModel_OnScrollReset;
				view.ViewModel.OnImagesListManagerItemClick -= view.ViewModel_OnImagesListManagerItemClick;
				view.ViewModel.IsInSelectionModeChanged -= view.ViewModel_IsInSelectionModeChanged;
			}

			view.ViewModel = new PostsViewerViewModel();
			view.ViewModel.PostsCollectionChanged += view.Posts_CollectionChanged;
			view.ViewModel.OnPreviewsUpdated += view.ViewModel_OnPreviewsUpdated;
			view.ViewModel.OnScrollReset += view.ViewModel_OnScrollReset;
			view.ViewModel.OnImagesListManagerItemClick += view.ViewModel_OnImagesListManagerItemClick;
			view.ViewModel.IsInSelectionModeChanged += view.ViewModel_IsInSelectionModeChanged;

			view.Root.DataContext = view.ViewModel;

			view.ViewModel.Initialize(value);

			view.PostDetailView.Visibility = Visibility.Collapsed;
			view.openedImageItem = null;
		}

		private void ViewModel_IsInSelectionModeChanged(bool isInSelectionMode) {
			foreach (ImageViewItem item in MainGrid.Children.Cast<ImageViewItem>()) {
				item.IsSelected = false;
			}
		}

		private void ViewModel_OnScrollReset() {
			MainScrollViewer.ChangeView(0, 0, 1);
		}

		private void ViewModel_OnPreviewsUpdated(object sender, OnPreviewsUpdateEventArgs e) {
			OnPreviewsUpdated?.Invoke(sender, e);
			OnPreviewsUpdatedCommand?.Execute(e);
		}

		public PostsViewer() {
			this.InitializeComponent();
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
			if (ViewModel.IsInSelectionMode) {

				view.IsSelected = !view.IsSelected;

			} else {

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

				if (Local.Settings.EnableTransitionAnimation) {
					ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("image_in", view.GetCurrentImage());
					ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("image_in");
					imageAnimation.Configuration = new DirectConnectedAnimationConfiguration();
					imageAnimation?.TryStart(PostDetailView.GetCurrentImage());
				}

			}

		}

		private void PostDetailView_RequestBack() {
			if (openedImageItem == null) {
				return;
			}

			PostDetailView.Visibility = Visibility.Collapsed;

			if (Local.Settings.EnableTransitionAnimation) {
				ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("image_out", PostDetailView.GetCurrentImage());
				ConnectedAnimation imageAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("image_out");
				imageAnimation.Configuration = new DirectConnectedAnimationConfiguration();
				imageAnimation?.TryStart(openedImageItem.GetCurrentImage());
			}

			openedImageItem = null;
		}

		private void ViewModel_OnImagesListManagerItemClick(E621Post post) {
			ImageViewItem found = null;
			foreach (ImageViewItem item in MainGrid.Children.Cast<ImageViewItem>()) {
				if (item.Post == post) {
					found = item;
					break;
				}
			}
			if (found == null) {
				return;
			}

			openedImageItem = found;
			PostDetailView.E621Post = post;
			PostDetailView.InitialImageURL = post.Sample.URL;
		}

		private void PageForwardButton_Click(object sender, RoutedEventArgs e) {
			PageFlyout.Hide();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e) {
			ViewModel.XamlRoot = XamlRoot;
		}

		public void PauseVideo() {
			PostDetailView.PauseVideo();
		}

		public void PlayVideo() {
			if (PostDetailView.Visibility == Visibility.Visible) {
				PostDetailView.PlayVideo();
			}
		}

	}

	public class PostsViewerViewModel : BindableBase {
		public XamlRoot XamlRoot { get; set; }

		public event NotifyCollectionChangedEventHandler PostsCollectionChanged;
		public event OnPreviewsUpdateEventHandler OnPreviewsUpdated;
		public event Action OnScrollReset;
		public event Action<E621Post> OnImagesListManagerItemClick;
		public event Action<bool> IsInSelectionModeChanged;

		private int pageValue;
		private bool isLoading;
		private bool enablePreviousPageButton;
		private int page = -1;
		private string[] tags = { "" };

		private bool isPostsInfoPaneOpen = false;
		private bool isInSelectionMode;
		private bool inputByPosts;
		private PostsInfoViewParameters postsInfoViewParameters;
		private string errorHint;
		private bool isPool;

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

		public string ErrorHint {
			get => errorHint;
			set => SetProperty(ref errorHint, value);
		}

		public bool IsInSelectionMode {
			get => isInSelectionMode;
			set => SetProperty(ref isInSelectionMode, value, OnIsInSelectionModeChanged);
		}

		private void OnIsInSelectionModeChanged() {
			IsInSelectionModeChanged?.Invoke(IsInSelectionMode);
		}

		public ObservableCollection<E621Post> Posts { get; } = new ObservableCollection<E621Post>();
		public ObservableCollection<E621Post> Blocks { get; } = new ObservableCollection<E621Post>();

		public PostsInfoViewParameters PostsInfoViewParameters {
			get => postsInfoViewParameters;
			set => SetProperty(ref postsInfoViewParameters, value);
		}

		public bool InputByPosts {
			get => inputByPosts;
			set => SetProperty(ref inputByPosts, value);
		}

		public bool IsPool {
			get => isPool;
			set => SetProperty(ref isPool, value);
		}

		public E621Pool Pool { get; set; }

		#region Posts Info

		public bool IsPostsInfoPaneOpen {
			get => isPostsInfoPaneOpen;
			set => SetProperty(ref isPostsInfoPaneOpen, value);
		}

		public ICommand PostsInfoButtonCommand => new DelegateCommand(PostsInfoButton);
		public ICommand TagsInfoButtonCommand => new DelegateCommand(TagsInfoButton);

		private void PostsInfoButton() {
			IsPostsInfoPaneOpen = !IsPostsInfoPaneOpen;
		}

		private async void TagsInfoButton() {
			if (IsPool) {
				await PoolInfoView.ShowAsDialog(Pool, XamlRoot);
			} else {
				await new TagsInfoView() {
					Tags = Tags,
				}.CreateContentDialog(XamlRoot, new ContentDialogParameters() {
					CloseText = "Back",
					SkipWidthSet = true,
				}).ShowDialogAsync();
			}
		}

		#endregion


		public PostsViewerViewModel() {
			Posts.CollectionChanged += (s, e) => PostsCollectionChanged?.Invoke(s, e);
		}

		public void Initialize(E621PostParameters value) {
			page = -1;
			if (value.InputPosts) {
				SetByPosts(value.Posts);
				InputByPosts = true;

				IsPool = false;
				Pool = null;

			} else {
				Tags = value.Tags;
				Page = value.Page;
				InputByPosts = false;

				IsPool = value.Pool != null;
				Pool = value.Pool;

			}
		}

		public ICommand RefreshCommand => new DelegateCommand(Refresh);

		public ICommand PreviousPageCommand => new DelegateCommand(PreviousPage);
		public ICommand NextPageCommand => new DelegateCommand(NextPage);

		public ICommand ForwardPageCommand => new DelegateCommand(ForwardPage);

		public ICommand DownloadCommand => new DelegateCommand(Download);

		private void Download() {

		}

		private void OnPageChanged() {
			Refresh();
			EnablePreviousPageButton = Page > 1;
		}

		private void SetByPosts(IEnumerable<E621Post> posts) {
			Posts.Clear();
			Blocks.Clear();

			if (posts.IsEmpty()) {
				return;
			}

			LoadPosts(posts);
		}

		private async void Refresh() {
			OnScrollReset?.Invoke();
			if (InputByPosts) {
				E621Post[] posts = Posts.ToArray().Concat(Blocks).ToArray();

				Posts.Clear();
				Blocks.Clear();

				LoadPosts(posts);
			} else {
				if (IsLoading) {
					return;
				}

				IsLoading = true;

				Posts.Clear();
				Blocks.Clear();

				E621Post[] posts;
				try {
					posts = await E621API.GetPostsByTagsAsync(new E621PostParameters() {
						Page = Page,
						Tags = Tags,
					});
				} catch {
					posts = null;
				}

				LoadPosts(posts);

				IsLoading = false;
			}
		}

		private void LoadPosts(IEnumerable<E621Post> posts) {
			if (posts == null) {
				return;
			}

			foreach (E621Post post in posts) {
				if (Local.Listing.ContainBlocks(post)) {
					Blocks.Add(post);
				} else {
					Posts.Add(post);
				}
			}
			string[] previews = Posts.Select(x => x.Sample.URL).Where(x => x.IsNotBlank()).Take(5).ToArray();
			OnPreviewsUpdated?.Invoke(this, new OnPreviewsUpdateEventArgs(previews));

			RaisePropertyChanged(nameof(Blocks));
			RaisePropertyChanged(nameof(Posts));

			if (Blocks.IsNotEmpty() && Blocks.Count == posts.Count()) {
				ErrorHint = "All posts are blocked";
			} else if (Posts.IsEmpty()) {
				ErrorHint = "No posts found";
			} else {
				ErrorHint = string.Empty;
			}

			PostsInfoViewParameters = new PostsInfoViewParameters(Posts.ToArray(), Blocks.ToArray());
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

		public ICommand ImagesListManagerItemClickCommand => new DelegateCommand<E621Post>(ImagesListManagerItemClick);

		private void ImagesListManagerItemClick(E621Post post) {
			OnImagesListManagerItemClick?.Invoke(post);
		}
	}
}
