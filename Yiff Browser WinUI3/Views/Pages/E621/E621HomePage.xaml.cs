using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Services.Locals;
using Yiff_Browser_WinUI3.Services.Networks;
using Yiff_Browser_WinUI3.Views.Controls;
using Yiff_Browser_WinUI3.Views.Controls.SearchViews;

namespace Yiff_Browser_WinUI3.Views.Pages.E621 {
	public sealed partial class E621HomePage : Page {
		public E621HomePage() {
			this.InitializeComponent();
		}

		private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args) {
			ViewModel.Items.Remove((HomeTabViewItem)args.Item);
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {
			ViewModel.XamlRoot = XamlRoot;
		}

	}

	public class E621HomePageViewModel : BindableBase {
		public XamlRoot XamlRoot { get; set; }
		private int selectedIndex;

		public ObservableCollection<HomeTabViewItem> Items { get; } = new();

		public int SelectedIndex {
			get => selectedIndex;
			set => SetProperty(ref selectedIndex, value);
		}

		public ICommand SearchCommand => new DelegateCommand(Search);

		private async void Search() {
			ContentDialog dialog = new() {
				XamlRoot = XamlRoot,
				Style = App.DialogStyle,
				Title = "Search",
			};

			string pretext = "";
			if (SelectedIndex != -1) {
				pretext = Items[SelectedIndex].Title;
			}

			SearchView searchView = new(dialog, pretext);
			dialog.Content = searchView;

			await dialog.ShowAsync();

			if (!searchView.IsConfirmDialog()) {
				return;
			}

			string[] searchTags = searchView.GetSearchTags();

			HomeTabViewItem item = new(searchTags);

			Items.Add(item);

			SelectedIndex = Items.Count - 1;
		}

		public E621HomePageViewModel() {
			string[] startupTags = Local.Settings.StartupTags;
			Items.Add(new HomeTabViewItem(startupTags));
			SelectedIndex = 0;
		}

	}

	public class HomeTabViewItem : BindableBase {
		private string title;
		private string[] previewURLs;
		private bool isSelected;
		private string[] tags;
		private bool isLoading;
		private E621PostParameters parameters;

		public string Title {
			get => title;
			set => SetProperty(ref title, value);
		}

		public string[] PreviewURLs {
			get => previewURLs;
			set => SetProperty(ref previewURLs, value);
		}

		public bool IsSelected {
			get => isSelected;
			set => SetProperty(ref isSelected, value);
		}

		private string[] Tags {
			get => tags;
			set => SetProperty(ref tags, value);
		}

		public bool IsLoading {
			get => isLoading;
			set => SetProperty(ref isLoading, value);
		}

		public E621PostParameters Parameters {
			get => parameters;
			set => SetProperty(ref parameters, value);
		}

		public ICommand OnPreviewsUpdateCommand => new DelegateCommand<OnPreviewsUpdateEventArgs>(OnPreviewsUpdate);

		private void OnPreviewsUpdate(OnPreviewsUpdateEventArgs args) {
			PreviewURLs = args.PreviewURLs;
		}

		public HomeTabViewItem(params string[] tags) {
			Tags = tags ?? Array.Empty<string>();
			Title = Tags.ToFullString().NotBlankCheck() ?? "Default";
			Parameters = new E621PostParameters() {
				Page = 1,
				Tags = Tags,
			};

		}

		public ICommand CopyCommand => new DelegateCommand(Copy);

		private void Copy() {
			Title.CopyToClipboard();
		}
	}
}
