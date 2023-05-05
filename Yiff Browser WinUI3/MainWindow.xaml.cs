using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Services.Locals;
using Yiff_Browser_WinUI3.Views.Controls.LoadingViews;
using Yiff_Browser_WinUI3.Views.Pages;
using Yiff_Browser_WinUI3.Views.Pages.E621;

namespace Yiff_Browser_WinUI3 {
	public sealed partial class MainWindow : Window {
		public string TAG_HOME { get; } = "TAG_HOME";
		public string TAG_FAVORITES { get; } = "TAG_FAVORITES";
		public string TAG_FOLLOWS { get; } = "TAG_FOLLOWS";
		public string TAG_DOWNLOADS { get; } = "TAG_DOWNLOADS";

		public MainWindow() {
			this.InitializeComponent();
		}

		private void Root_Loaded(object sender, RoutedEventArgs e) {
			Initialize();
		}

		private async void Initialize() {
			LoadingRingWithTextBelow loader = new() {
				Text = "Loading local stuff",
			};

			LoadingDialogControl control = new(Root.XamlRoot, loader);

			static async Task Init() => await Local.Initialize();

			await control.Start(Init);

			NavigateHome();
		}


		private void MainNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args) {
			NavigationViewItem item = args.InvokedItemContainer as NavigationViewItem;
			string tag = item.Tag as string;

			Type targetType;
			if (args.IsSettingsInvoked) {
				targetType = typeof(SettingsPage);
			} else {
				if (tag == TAG_HOME) {
					targetType = typeof(E621HomePage);
				} else {
					targetType = typeof(TestPage);
					//throw new NotSupportedException($"{tag}");
				}
			}
			MainFrame.Navigate(targetType, null, args.RecommendedNavigationTransitionInfo);
		}


		public void NavigateHome() {
			MainNavigationView.SelectedItem = ItemHome;
			MainFrame.Navigate(typeof(E621HomePage), null, new EntranceNavigationTransitionInfo());
		}

	}

	public class MainWindowViewModel : BindableBase {

	}
}
