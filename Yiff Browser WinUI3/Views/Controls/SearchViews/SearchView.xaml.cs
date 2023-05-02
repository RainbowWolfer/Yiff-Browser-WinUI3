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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Yiff_Browser_WinUI3.Models.E621;

namespace Yiff_Browser_WinUI3.Views.Controls.SearchViews {
	public sealed partial class SearchView : UserControl {
		public SearchView(ContentDialog dialog) {
			this.InitializeComponent();
			ViewModel.Dialog = dialog;
		}

		private void ListView_ItemClick(object sender, ItemClickEventArgs e) {
			//e.ClickedItem
		}

		public bool IsConfirmDialog() => ViewModel.ConfirmDialog;
	}

	public class SearchViewModel : BindableBase {
		public ContentDialog Dialog { get; set; }

		private string searchText;

		public ObservableCollection<SearchTagItem> AutoCompletes { get; } = new();

		public bool ConfirmDialog { get; private set; } = false;

		public string SearchText {
			get => searchText;
			set => SetProperty(ref searchText, value, OnSearchTextChanged);
		}

		private void OnSearchTextChanged() {
			Debug.WriteLine(SearchText);


		}

		public ICommand ItemClickCommand => new DelegateCommand<ItemClickEventArgs>(ItemClick);

		public ICommand BackCommand => new DelegateCommand(Back);

		private void ItemClick(ItemClickEventArgs args) {
			SearchTagItem clickedItem = (SearchTagItem)args.ClickedItem;
			foreach (SearchTagItem item in AutoCompletes) {
				item.IsSelected = item == clickedItem;
			}
		}

		private void Back() {
			Dialog.Hide();
		}

		public SearchViewModel() {
			AutoCompletes.Add(new SearchTagItem(new E621AutoComplete() { name = "!!" }));
			AutoCompletes.Add(new SearchTagItem(new E621AutoComplete() { name = "feet", antecedent_name = "sw" }));
			AutoCompletes.Add(new SearchTagItem(new E621AutoComplete()));
			AutoCompletes.Add(new SearchTagItem(new E621AutoComplete()));
			AutoCompletes.Add(new SearchTagItem(new E621AutoComplete()));
			AutoCompletes.Add(new SearchTagItem(new E621AutoComplete()));
			AutoCompletes.Add(new SearchTagItem(new E621AutoComplete()));
		}

	}

	public class SearchTagItem : BindableBase {
		private E621AutoComplete autoComplete;
		private bool isSelected;

		public E621AutoComplete AutoComplete {
			get => autoComplete;
			set => SetProperty(ref autoComplete, value);
		}

		public bool IsSelected {
			get => isSelected;
			set => SetProperty(ref isSelected, value);
		}

		public SearchTagItem(E621AutoComplete autoComplete) {
			AutoComplete = autoComplete;
			IsSelected = false;
		}
	}
}
