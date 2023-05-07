using CommunityToolkit.WinUI.UI.Controls.TextToolbarSymbols;
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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class PostsInfoListView : UserControl {

		public ObservableCollection<E621Post> Posts {
			get => (ObservableCollection<E621Post>)GetValue(MyPropertyProperty);
			set => SetValue(MyPropertyProperty, value);
		}

		public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
			nameof(Posts),
			typeof(ObservableCollection<E621Post>),
			typeof(PostsInfoListView),
			new PropertyMetadata(new ObservableCollection<E621Post>(), OnPostsChanged)
		);

		private static void OnPostsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not PostsInfoListView view) {
				return;
			}

			ObservableCollection<E621Post> oldCollection = (ObservableCollection<E621Post>)e.OldValue;
			ObservableCollection<E621Post> newCollection = (ObservableCollection<E621Post>)e.NewValue;

			if (oldCollection != null) {
				oldCollection.CollectionChanged -= view.Posts_CollectionChanged;
			}

			newCollection.CollectionChanged += view.Posts_CollectionChanged;
		}



		public ObservableCollection<E621Post> Blocks {
			get => (ObservableCollection<E621Post>)GetValue(BlocksProperty);
			set => SetValue(BlocksProperty, value);
		}

		public static readonly DependencyProperty BlocksProperty = DependencyProperty.Register(
			nameof(Blocks),
			typeof(ObservableCollection<E621Post>),
			typeof(PostsInfoListView),
			new PropertyMetadata(new ObservableCollection<E621Post>(), OnBlocksChanged)
		);

		private static void OnBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not PostsInfoListView view) {
				return;
			}

			ObservableCollection<E621Post> oldCollection = (ObservableCollection<E621Post>)e.OldValue;
			ObservableCollection<E621Post> newCollection = (ObservableCollection<E621Post>)e.NewValue;

			if (oldCollection != null) {
				oldCollection.CollectionChanged -= view.Posts_CollectionChanged;
			}

			newCollection.CollectionChanged += view.Posts_CollectionChanged;
		}

		public int TargetPostsCount {
			get => (int)GetValue(TargetPostsCountProperty);
			set => SetValue(TargetPostsCountProperty, value);
		}

		public static readonly DependencyProperty TargetPostsCountProperty = DependencyProperty.Register(
			nameof(TargetPostsCount),
			typeof(int),
			typeof(PostsInfoListView),
			new PropertyMetadata(0)
		);




		private void Posts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Reset) {
				Clear();
			} else {
				UpdatePostsInfo();
			}
		}

		private void Clear() {
			List.Clear();
			List.Add(new PostsInfoList("Hot Tags (Top 20)", new List<PostInfoLine>()));
		}

		private void UpdatePostsInfo() {
			if (Posts.Count + Blocks.Count != TargetPostsCount) {
				return;
			}

			List.Clear();

			AddToList("Blacklist", Blocks);
			AddToList("Hot Tags (Top 20)", Posts, 20);
		}

		private void AddToList(string name, IEnumerable<E621Post> posts, int limit = int.MaxValue) {
			Dictionary<string, int> tags = CountTags(posts);
			List<PostInfoLine> list = new();

			int count = 0;
			foreach (KeyValuePair<string, int> item in tags) {
				list.Add(new PostInfoLine(item.Key, $"{item.Value}"));
				if (count++ > limit) {
					break;
				}
			}
			List.Add(new PostsInfoList(name, list));
		}

		private static Dictionary<string, int> CountTags(IEnumerable<E621Post> posts) {
			Dictionary<string, int> allTags = new();

			foreach (E621Post item in posts) {
				foreach (string tag in item.Tags.GetAllTags()) {
					if (allTags.ContainsKey(tag)) {
						allTags[tag]++;
					} else {
						allTags.Add(tag, 1);
					}
				}
			}

			Dictionary<string, int> tags = allTags.OrderByDescending(o => o.Value).ToDictionary(x => x.Key, x => x.Value);

			return tags;
		}


		public ObservableCollection<PostsInfoList> List { get; } = new();



		public PostsInfoListView() {
			this.InitializeComponent();
		}
	}

	public class PostInfoLine : BindableBase {
		private string name;
		private string detail;

		public string Name {
			get => name;
			set => SetProperty(ref name, value);
		}

		public string Detail {
			get => detail;
			set => SetProperty(ref detail, value);
		}

		public ICommand CopyCommand => new DelegateCommand(Copy);

		private void Copy() {
			Name.CopyToClipboard();
		}

		public PostInfoLine(string name, string detail) {
			Name = name;
			Detail = detail;
		}

		public override string ToString() {
			return $"Name ({Name}) - Detail ({Detail})";
		}
	}

	public class PostsInfoList : ObservableCollection<PostInfoLine> {
		public string Key { get; set; }
		public PostsInfoList(string key) : base() {
			this.Key = key;
		}
		public PostsInfoList(string key, List<PostInfoLine> content) : base() {
			this.Key = key;
			content.ForEach(s => this.Add(s));
		}
	}

}
