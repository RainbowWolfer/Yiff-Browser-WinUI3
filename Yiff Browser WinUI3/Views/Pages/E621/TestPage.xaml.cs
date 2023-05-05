using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Prism.Mvvm;
using Yiff_Browser_WinUI3.Views.Controls;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Views.Pages.E621 {
	public sealed partial class TestPage : Page {
		public TestPage() {
			this.InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e) {
			ListingsManager view = new() {
				FollowsOrBlocks = false,
			};
			await view.CreateContentDialog(XamlRoot, new ContentDialogParameters() {
				Title = "Follows",
				CloseText = "Back",
			}).ShowDialogAsync();

			//save to local

		}
	}

	public class TestPageViewModel : BindableBase {

	}
}
