using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class ListingsManager : UserControl {


		public bool FollowsOrBlocks {
			get => (bool)GetValue(FollowsOrBlocksProperty);
			set => SetValue(FollowsOrBlocksProperty, value);
		}

		public static readonly DependencyProperty FollowsOrBlocksProperty = DependencyProperty.Register(
			nameof(FollowsOrBlocks),
			typeof(bool),
			typeof(ListingsManager),
			new PropertyMetadata(false)
		);

		public ListingsManager() {
			this.InitializeComponent();
			ViewModel.RequestDeleteListing += ViewModel_RequestDeleteListing;
			ViewModel.RequestRenameListing += ViewModel_RequestRenameListing;

			ViewModel.RequestUpdateListingView += ViewModel_RequestUpdateListingView;
		}

		private void ViewModel_RequestUpdateListingView() {

		}

		private void ViewModel_RequestRenameListing(ListingViewItem item) {
			ListViewItem found = (ListViewItem)ListingsListView.ContainerFromItem(item);
			if (found != null) {
				RenameTeachingTip.Target = found;
				RenameTeachingTip.PreferredPlacement = TeachingTipPlacementMode.Bottom;
			} else {
				RenameTeachingTip.PreferredPlacement = TeachingTipPlacementMode.Center;
			}
		}

		private void ViewModel_RequestDeleteListing(ListingViewItem item) {
			ListViewItem found = (ListViewItem)ListingsListView.ContainerFromItem(item);
			if (found != null) {
				DeleteConfirmTeachingTip.Target = found;
				DeleteConfirmTeachingTip.PreferredPlacement = TeachingTipPlacementMode.Bottom;
			} else {
				DeleteConfirmTeachingTip.PreferredPlacement = TeachingTipPlacementMode.Center;
			}
			DeleteConfirmTeachingTip.Subtitle = $"Are you sure to delete ({item.Item.Name}) with {item.Item.Tags.Count} tags";
		}
	}

	public class ListingsManagerViewModel : BindableBase {
		public event Action<ListingViewItem> RequestDeleteListing;
		public event Action<ListingViewItem> RequestRenameListing;

		public event Action RequestUpdateListingView;

		private bool isDelelteListTipOpen;
		private bool isRenameListTipOpen;

		private bool followsOrBlocks;

		private ListingViewItem checkedItem = null;
		private int selectedIndex = -1;
		private int tagItemsSelectedIndex = 0;

		private ListingViewItem toBeManipulatedListItem = null;

		private string renameListingText = "";

		private string[] existListNames;
		private string[] existTagNames;

		public ObservableCollection<ListingViewItem> ListingItems { get; } = new();

		public ObservableCollection<TagViewItem> ItemTags { get; } = new();

		public ListingViewItem CheckedItem {
			get => checkedItem;
			set => SetProperty(ref checkedItem, value, OnCheckedItemChanged);
		}

		private void OnCheckedItemChanged() {
			foreach (ListingViewItem item in ListingItems) {
				item.IsSelected = item == CheckedItem;
			}
		}

		public int SelectedIndex {
			get => selectedIndex;
			set => SetProperty(ref selectedIndex, value, OnSelectedIndexChanged);
		}

		private void OnSelectedIndexChanged() {
			ItemTags.Clear();

			ListingViewItem item = ListingItems[SelectedIndex];

			foreach (string tag in item.Item.Tags) {
				ItemTags.Add(CreateTagViewItem(tag));
			}

		}

		public int TagItemsSelectedIndex {
			get => tagItemsSelectedIndex;
			set => SetProperty(ref tagItemsSelectedIndex, value);
		}

		private TagViewItem CreateTagViewItem(string tag) {
			TagViewItem item = new(tag);
			item.OnDelete += i => {
				ItemTags.Remove(i);
			};
			return item;
		}

		public bool FollowsOrBlocks {
			get => followsOrBlocks;
			set => SetProperty(ref followsOrBlocks, value, OnFollowsOrBlocksChanged);
		}

		private void OnFollowsOrBlocksChanged() {
			List<ListingItem> items;
			if (FollowsOrBlocks) {
				items = Local.Listing.Follows;
			} else {
				items = Local.Listing.Blocks;
			}
			foreach (ListingItem item in items) {
				ListingItems.Add(new ListingViewItem(item));
			}
		}

		public ListingsManagerViewModel() {
			ListingItems.CollectionChanged += ListingItems_CollectionChanged;
			ItemTags.CollectionChanged += ItemTags_CollectionChanged;

			ListingItems.Add(CreateNewListItem(new ListingItem("Default") {
				Tags = { "feet", "cum", "cum_on_soles", "toe", "feet", "cum", "cum_on_soles", "toe", "feet", "cum", "cum_on_soles", "toe", "feet", "cum", "cum_on_soles", "toe", },
			}));
		}

		private void ItemTags_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (ItemTags.IsEmpty()) {
				SelectedIndex = -1;
			}
			if (e.OldItems.IsEmpty() && e.NewItems.IsNotEmpty()) {
				SelectedIndex = 0;
			}
			ExistTagNames = ItemTags.Select(x => x.Tag).ToArray();
		}

		private void ListingItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (ListingItems.IsEmpty()) {
				SelectedIndex = -1;
			}
			if (e.OldItems.IsEmpty() && e.NewItems.IsNotEmpty()) {
				SelectedIndex = 0;
			}

			ExistListNames = ListingItems.Select(x => x.Item.Name).ToArray();
		}

		public bool IsDelelteListTipOpen {
			get => isDelelteListTipOpen;
			set => SetProperty(ref isDelelteListTipOpen, value);
		}

		public bool IsRenameListTipOpen {
			get => isRenameListTipOpen;
			set => SetProperty(ref isRenameListTipOpen, value);
		}

		public string RenameListingText {
			get => renameListingText;
			set => SetProperty(ref renameListingText, value);
		}

		public string[] ExistListNames {
			get => existListNames;
			set => SetProperty(ref existListNames, value);
		}

		public string[] ExistTagNames {
			get => existTagNames;
			set => SetProperty(ref existTagNames, value);
		}


		public ICommand PasteAsNewListCommand => new DelegateCommand(PasteAsNewList);

		public ICommand ConfirmDeleteListingCommand => new DelegateCommand(ConfirmDeleteListing);
		public ICommand CancelDeleteListingCommand => new DelegateCommand(CancelDeleteListing);
		public ICommand OnNewTagSumbitCommand => new DelegateCommand<string>(OnNewTagSumbit);
		public ICommand OnNewListSumbitCommand => new DelegateCommand<string>(OnNewListSumbit);

		private void OnNewListSumbit(string text) {
			ListingItems.Add(CreateNewListItem(new ListingItem(text)));
		}

		private void OnNewTagSumbit(string text) {
			ItemTags.Insert(0, CreateTagViewItem(text));
			TagItemsSelectedIndex = 0;
		}

		private ListingViewItem CreateNewListItem(ListingItem item) {
			ListingViewItem viewItem = new(item);
			viewItem.OnDelete += p => {
				RequestDeleteListing?.Invoke(p);
				IsDelelteListTipOpen = true;
				toBeManipulatedListItem = p;
			};
			viewItem.OnRename += p => {
				RequestRenameListing?.Invoke(p);
				RenameListingText = p.Item.Name;
				IsRenameListTipOpen = true;
				toBeManipulatedListItem = p;
			};
			viewItem.OnCheck += p => {
				CheckedItem = p;
			};
			return viewItem;
		}

		private void PasteAsNewList() {

		}


		private void ConfirmDeleteListing() {
			if (toBeManipulatedListItem == null) {
				return;
			}

			bool reindex = toBeManipulatedListItem == ListingItems[SelectedIndex];

			ListingItems.Remove(toBeManipulatedListItem);

			if (reindex) {
				SelectedIndex = Math.Clamp(SelectedIndex - 1, 0, ListingItems.Count);
			}

			toBeManipulatedListItem = null;

			IsDelelteListTipOpen = false;
		}

		private void CancelDeleteListing() {
			toBeManipulatedListItem = null;
		}



		public ICommand ConfirmRenameListingCommand => new DelegateCommand(ConfirmRenameListing);
		public ICommand CancelRenameListingCommand => new DelegateCommand(CancelRenameListing);

		private void ConfirmRenameListing() {
			if (toBeManipulatedListItem == null) {
				return;
			}
			string text = RenameListingText.Trim();
			if (text.IsBlank() || (ExistListNames?.Contains(text) ?? false)) {
				return;
			}

			toBeManipulatedListItem.Item.Name = text;
			toBeManipulatedListItem.UpdateItem();

			IsRenameListTipOpen = false;

			toBeManipulatedListItem = null;
		}


		private void CancelRenameListing() {
			toBeManipulatedListItem = null;
		}

	}

	public class TagViewItem : BindableBase {
		private bool isLoading;
		private E621Tag e621Tag;
		private string tag;

		public TagViewItem(string tag) {
			Tag = tag;
		}

		public event Action<TagViewItem> OnDelete;

		public string Tag {
			get => tag;
			set => SetProperty(ref tag, value);
		}

		public E621Tag E621Tag {
			get => e621Tag;
			set => SetProperty(ref e621Tag, value);
		}

		public ICommand DeleteCommand => new DelegateCommand(Delete);
		public ICommand CopyCommand => new DelegateCommand(Copy);
		public ICommand LoadedCommand => new DelegateCommand<string>(Loaded);

		public bool IsLoading {
			get => isLoading;
			set => SetProperty(ref isLoading, value);
		}

		private async void Loaded(string tag) {
			IsLoading = true;
			E621Tag = await E621API.GetE621TagAsync(tag);
			IsLoading = false;
		}

		private void Delete() {
			OnDelete?.Invoke(this);
		}

		private void Copy() {

		}

	}

	public class ListingViewItem : BindableBase {
		public event Action<ListingViewItem> OnCheck;
		public event Action<ListingViewItem> OnDelete;
		public event Action<ListingViewItem> OnRename;

		private bool isSelected;
		private bool isLoading;

		public ListingItem Item { get; set; }

		public void UpdateItem() {
			RaisePropertyChanged(nameof(Item));
		}

		public bool IsSelected {
			get => isSelected;
			set => SetProperty(ref isSelected, value);
		}


		public bool IsLoading {
			get => isLoading;
			set => SetProperty(ref isLoading, value);
		}

		public ICommand CheckedCommand => new DelegateCommand<RoutedEventArgs>(Checked);
		private void Checked(RoutedEventArgs args) {
			OnCheck?.Invoke(this);
		}

		public ICommand RenameCommand => new DelegateCommand(Rename);
		public ICommand DeleteCommand => new DelegateCommand(Delete);

		private void Rename() {
			OnRename?.Invoke(this);
		}

		private void Delete() {
			OnDelete?.Invoke(this);
		}

		public ListingViewItem(ListingItem item) {
			Item = item;
		}

	}
}
