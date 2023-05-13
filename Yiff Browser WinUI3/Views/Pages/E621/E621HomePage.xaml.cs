using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;
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
			//sender.TabItems.Remove(args.Tab);
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
			Instance = this;

			string[] startupTags = Local.Settings.StartupTags;
			//string[] startupTags = { "inpool:true" };
			Items.Add(new HomeTabViewItem(startupTags));
			SelectedIndex = 0;
		}


		#region Static
		public static E621HomePageViewModel Instance { get; private set; }

		public static void CreateNewTag(string tag) {
			if (Instance == null) {
				return;
			}
			Instance.Items.Add(new HomeTabViewItem(tag));
			Instance.SelectedIndex = Instance.Items.Count - 1;
		}

		public static void CreatePool(E621Pool pool) {
			if (Instance == null) {
				return;
			}

			Instance.Items.Add(new HomeTabViewItem(pool));
			Instance.SelectedIndex = Instance.Items.Count - 1;
		}

		public static void CreateConcatTag(string tag, bool addOrRemove) {
			if (Instance == null) {
				return;
			}

			HomeTabViewItem item = Instance.Items[Instance.SelectedIndex];
			if (item.Parameters.Tags == null) {
				return;
			}

			List<string> tags = item.Parameters.Tags.ToList();
			if (addOrRemove) {
				tags.Add($"{tag}");
			} else {
				tags.Add($"-{tag}");
			}

			Instance.Items.Add(new HomeTabViewItem(tags.ToArray()));
			Instance.SelectedIndex = Instance.Items.Count - 1;
		}

		public static void CreatePosts(string title, IEnumerable<E621Post> posts) {
			if (Instance == null || posts.IsEmpty()) {
				return;
			}
			Instance.Items.Add(new HomeTabViewItem(title, posts));
			Instance.SelectedIndex = Instance.Items.Count - 1;
		}


		#endregion

	}

	public class HomeTabViewItem : BindableBase {
		private string title;
		private string[] previewURLs;
		private bool isSelected;
		private string[] tags;
		private bool isLoading;
		private E621PostParameters parameters;
		private string titleIcon;

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

		public string TitleIcon {
			get => titleIcon;
			set => SetProperty(ref titleIcon, value);
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

		public HomeTabViewItem(string title, IEnumerable<E621Post> posts) {
			Title = title.NotBlankCheck() ?? "Empty title";
			Parameters = new E621PostParameters() {
				Posts = posts.ToArray(),
				InputPosts = true,
			};
		}

		public HomeTabViewItem(E621Pool pool) {
			Title = pool.Name.NotBlankCheck() ?? "Empty Pool Name";
			Parameters = new E621PostParameters() {
				Page = 1,
				Tags = new string[] { $"pool:{pool.ID}" },
				Pool = pool,
			};
		}

		public ICommand CopyCommand => new DelegateCommand(Copy);

		private void Copy() {
			Title.CopyToClipboard();
		}
	}
}
