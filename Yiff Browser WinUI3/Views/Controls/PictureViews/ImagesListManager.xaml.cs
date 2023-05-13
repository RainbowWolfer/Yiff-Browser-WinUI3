using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Views.Controls.PictureViews {
	public sealed partial class ImagesListManager : UserControl {

		public ICommand ItemClickCommand {
			get => (ICommand)GetValue(ItemClickCommandProperty);
			set => SetValue(ItemClickCommandProperty, value);
		}

		public static readonly DependencyProperty ItemClickCommandProperty = DependencyProperty.Register(
			nameof(ItemClickCommand),
			typeof(ICommand),
			typeof(ImagesListManager),
			new PropertyMetadata(null)
		);

		public E621Post[] PostsList {
			get => (E621Post[])GetValue(PostsListProperty);
			set => SetValue(PostsListProperty, value);
		}

		public static readonly DependencyProperty PostsListProperty = DependencyProperty.Register(
			nameof(PostsList),
			typeof(E621Post[]),
			typeof(ImagesListManager),
			new PropertyMetadata(Array.Empty<E621Post>(), OnPostsListChanged)
		);

		private static void OnPostsListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not ImagesListManager view) {
				return;
			}

			view.Items.Clear();
			foreach (E621Post item in (E621Post[])e.NewValue) {
				view.Items.Add(new ImagesListManagerItem(item, view));
			}
		}



		public E621Post CurrentPost {
			get => (E621Post)GetValue(CurrentPostProperty);
			set => SetValue(CurrentPostProperty, value);
		}

		public static readonly DependencyProperty CurrentPostProperty = DependencyProperty.Register(
			nameof(CurrentPost),
			typeof(E621Post),
			typeof(ImagesListManager),
			new PropertyMetadata(null, OnCurrentPostChanged)
		);

		private static void OnCurrentPostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not ImagesListManager view) {
				return;
			}

			for (int i = 0; i < view.Items.Count; i++) {
				ImagesListManagerItem item = view.Items[i];
				item.IsSelected = item.Post == (E621Post)e.NewValue;
				if (item.IsSelected) {
					view.PutToCenter(item);
				}
			}

		}

		public ObservableCollection<ImagesListManagerItem> Items { get; } = new ObservableCollection<ImagesListManagerItem>();

		public ImagesListManager() {
			this.InitializeComponent();

		}

		private void PhotosListGrid_PointerEntered(object sender, PointerRoutedEventArgs e) {

		}

		private void PhotosListGrid_PointerExited(object sender, PointerRoutedEventArgs e) {

		}

		private async void PutToCenter(ImagesListManagerItem item) {
			int index = Items.IndexOf(item);
			//var c = ListControl.ContainerFromIndex(index);
			//double width = c.ActualWidth;
			int dount = ListControl.Items.Count;
			int margin = 3;


			//var found = PhotosListView.Items.Where(i => i is ListViewItem lvi && lvi.Content is PhotosListItem pli && pli == item).FirstOrDefault();
			////var found = PhotosListView.Items.Where(i => i is ListViewItem lvi && lvi == item);
			////await PhotosListView.SmoothScrollIntoViewWithItemAsync(found);

			//await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
			//	PhotosListView.ScrollToCenterOfView(found);
			//});

			ListScroll.ChangeView(0, 0, 1);
		}

	}

	public class ImagesListManagerItem : BindableBase {
		private readonly ImagesListManager manager;

		private E621Post post;

		private string imageURL = null;
		private string typeIcon = null;
		private bool showLoading;

		private bool isSelected = false;

		private Brush borderBrush = new SolidColorBrush(Colors.Beige);
		private bool isMouseOn;

		public ImagesListManagerItem(E621Post post, ImagesListManager manager) {
			Post = post;
			this.manager = manager;
		}

		public E621Post Post {
			get => post;
			set => SetProperty(ref post, value, OnPostChanged);
		}

		public string ImageURL {
			get => imageURL;
			set => SetProperty(ref imageURL, value);
		}

		public string TypeIcon {
			get => typeIcon;
			set => SetProperty(ref typeIcon, value);
		}

		public bool ShowLoading {
			get => showLoading;
			set => SetProperty(ref showLoading, value);
		}

		public Brush BorderBrush {
			get => borderBrush;
			set => SetProperty(ref borderBrush, value);
		}

		public bool IsMouseOn {
			get => isMouseOn;
			set => SetProperty(ref isMouseOn, value);
		}

		public bool IsSelected {
			get => isSelected;
			set => SetProperty(ref isSelected, value, OnIsSelectedChanged);
		}

		private void OnIsSelectedChanged() {
			BorderBrush = new SolidColorBrush(IsSelected ? Colors.Red : Colors.Beige);
		}

		private void OnPostChanged() {
			if (Post == null || Post.HasNoValidURLs()) {
				return;
			}

			ShowLoading = true;

			ImageURL = Post.Sample.URL;
			FileType fileType = Post.GetFileType();
			TypeIcon = fileType switch {
				FileType.Gif => "\uF4A9",
				FileType.Webm => "\uE102",
				_ => null,
			};
		}

		public ICommand ImageOpenedCommand => new DelegateCommand<RoutedEventArgs>(ImageOpened);
		public ICommand ImageFailedCommand => new DelegateCommand(ImageFailed);

		private void ImageOpened(RoutedEventArgs args) {
			ShowLoading = false;
		}

		private void ImageFailed() {
			ShowLoading = false;
		}

		public ICommand PointerEnteredCommand => new DelegateCommand(PointerEntered);
		public ICommand PointerExitedCommand => new DelegateCommand(PointerExited);

		private void PointerEntered() {
			IsMouseOn = true;
		}

		private void PointerExited() {
			IsMouseOn = false;
		}

		public ICommand ClickCommand => new DelegateCommand(Click);

		private void Click() {
			manager.ItemClickCommand?.Execute(Post);
		}
	}
}
