using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;
using Yiff_Browser_WinUI3.Services.Locals;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class PostTagsListView : UserControl {
		public readonly ObservableCollection<GroupTagListWithColor> tags = new();

		public Tags Tags {
			get => (Tags)GetValue(TagsProperty);
			set => SetValue(TagsProperty, value);
		}

		public static readonly DependencyProperty TagsProperty = DependencyProperty.Register(
			nameof(Tags),
			typeof(Tags),
			typeof(PostTagsListView),
			new PropertyMetadata(Array.Empty<string>(), OnTagsChanged)
		);

		private static void OnTagsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is PostTagsListView view) {
				view.UpdateTagsGroup((Tags)e.NewValue);
			}
		}

		private void UpdateTagsGroup(Tags tags) {
			if (tags == null) {
				return;
			}
			RemoveGroup();
			bool isDark = App.GetApplicationTheme() == ApplicationTheme.Dark;

			AddNewGroup("Artist", ToGroupTag(tags.Artist, E621Tag.GetCatrgoryColor(E621TagCategory.Artists, isDark)));
			AddNewGroup("Copyright", ToGroupTag(tags.Copyright, E621Tag.GetCatrgoryColor(E621TagCategory.Copyrights, isDark)));
			AddNewGroup("Species", ToGroupTag(tags.Species, E621Tag.GetCatrgoryColor(E621TagCategory.Species, isDark)));
			AddNewGroup("Character", ToGroupTag(tags.Character, E621Tag.GetCatrgoryColor(E621TagCategory.Characters, isDark)));
			AddNewGroup("General", ToGroupTag(tags.General, E621Tag.GetCatrgoryColor(E621TagCategory.General, isDark)));
			AddNewGroup("Meta", ToGroupTag(tags.Meta, E621Tag.GetCatrgoryColor(E621TagCategory.Meta, isDark)));
			AddNewGroup("Invalid", ToGroupTag(tags.Invalid, E621Tag.GetCatrgoryColor(E621TagCategory.Invalid, isDark)));
			AddNewGroup("Lore", ToGroupTag(tags.Lore, E621Tag.GetCatrgoryColor(E621TagCategory.Lore, isDark)));

		}

		public List<GroupTag> ToGroupTag(List<string> tags, Color color) {
			List<GroupTag> result = new();
			foreach (string tag in tags) {
				GroupTag item = new(tag, color);
				item.InfoAction += Item_InfoAction;
				item.AddAction += Item_AddAction;
				item.MinusAction += Item_MinusAction;
				result.Add(item);
			}
			return result;
		}

		private void Item_MinusAction(string tag) {
		}

		private void Item_AddAction(string tag) {
		}

		private void Item_InfoAction(string tag) {
		}

		private void RemoveGroup() {
			tags.Clear();
		}

		private void AddNewGroup(string title, List<GroupTag> content) {
			if (content == null) {
				return;
			}
			if (content.Count == 0) {
				return;
			}
			tags.Add(new GroupTagListWithColor(title, content));
		}



		public ICommand ItemClickCommand {
			get => (ICommand)GetValue(ItemClickCommandProperty);
			set => SetValue(ItemClickCommandProperty, value);
		}

		public static readonly DependencyProperty ItemClickCommandProperty = DependencyProperty.Register(
			nameof(ItemClickCommand),
			typeof(ICommand),
			typeof(PostTagsListView),
			new PropertyMetadata(null)
		);




		public PostTagsListView() {
			this.InitializeComponent();
		}

		private void TagsListView_ItemClick(object sender, ItemClickEventArgs e) {
			ItemClickCommand?.Execute(e.ClickedItem);
		}
	}

	public class GroupTag : BindableBase {
		public event Action<string> InfoAction;
		public event Action<string> MinusAction;
		public event Action<string> AddAction;

		private string content;
		private Color color;
		private bool isInFollows;
		private bool isInBlocks;

		public string Content {
			get => content;
			set => SetProperty(ref content, value);
		}
		public Color Color {
			get => color;
			set => SetProperty(ref color, value);
		}

		public bool IsInFollows {
			get => isInFollows;
			set => SetProperty(ref isInFollows, value);
		}

		public bool IsInBlocks {
			get => isInBlocks;
			set => SetProperty(ref isInBlocks, value);
		}

		public ICommand InfoCommand => new DelegateCommand(Info);
		public ICommand MinusCommand => new DelegateCommand(Minus);
		public ICommand AddCommand => new DelegateCommand(Add);

		public ICommand FollowCommand => new DelegateCommand<string>(Follow);
		public ICommand BlockCommand => new DelegateCommand<string>(Block);

		private void Info() => InfoAction?.Invoke(Content);
		private void Minus() => MinusAction?.Invoke(Content);
		private void Add() => AddAction?.Invoke(Content);

		private void Follow(string addOrRemove) {
			if (addOrRemove == "Add") {
				Local.Listing.AddFollow(Content);
			} else {
				Local.Listing.RemoveFollow(Content);
			}
			Update();
			Listing.Write();
		}

		private void Block(string addOrRemove) {
			if (addOrRemove == "Add") {
				Local.Listing.AddBlock(Content);
			} else {
				Local.Listing.RemoveBlock(Content);
			}
			Update();
			Listing.Write();
		}

		public ICommand CopyCommand => new DelegateCommand(Copy);

		private void Copy() {
			Content.CopyToClipboard();
		}

		public GroupTag(string content, Color color) {
			Content = content;
			Color = color;
			Update();
		}

		public void Update() {
			IsInFollows = Local.Listing.ContainFollows(content);
			IsInBlocks = Local.Listing.ContainBlocks(content);
		}
	}

	public class GroupTagListWithColor : ObservableCollection<GroupTag> {
		public string Key { get; set; }
		public GroupTagListWithColor(string key) : base() {
			this.Key = key;
		}
		public GroupTagListWithColor(string key, List<GroupTag> content) : base() {
			this.Key = key;
			content.ForEach(s => this.Add(s));
		}
	}
}
