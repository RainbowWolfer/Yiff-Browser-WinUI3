using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;
using Yiff_Browser_WinUI3.Services.Networks;
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
			SearchView searchView = new(dialog);
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

	}

	public class HomeTabViewItem : BindableBase {
		private string title;
		private string[] previewURLs;
		private ObservableCollection<E621Post> posts;
		private bool isSelected;
		private string[] tags;
		private bool isLoading;

		public string Title {
			get => title;
			set => SetProperty(ref title, value);
		}

		public string[] PreviewURLs {
			get => previewURLs;
			set => SetProperty(ref previewURLs, value);
		}

		public ObservableCollection<E621Post> Posts {
			get => posts;
			set => SetProperty(ref posts, value);
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

		public HomeTabViewItem(params string[] tags) {
			Tags = tags ?? Array.Empty<string>();
			Title = Tags.ToFullString();
			Load();
		}

		private async void Load() {
			E621Post[] posts = await E621API.GetE621PostsByTagsAsync(new E621PostParameters() {
				Page = 1,
				Tags = Tags,
			});
			if (posts != null) {
				Posts = new ObservableCollection<E621Post>(posts);
				PreviewURLs = Posts.Select(x => x.sample.url).Where(x => x.IsNotBlank()).Take(5).ToArray();
			}
		}
	}
}
