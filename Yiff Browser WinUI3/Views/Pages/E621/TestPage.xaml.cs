using Prism.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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
using Yiff_Browser_WinUI3.Services.Networks;
using System.Diagnostics;
using Microsoft.UI.Input;
using Windows.UI.Core;
using Yiff_Browser_WinUI3.Services.Locals;

namespace Yiff_Browser_WinUI3.Views.Pages.E621 {
	public sealed partial class TestPage : Page {
		public TestPage() {
			this.InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e) {
			bool followsOrBlocks = true;
			ListingsManager view = new(followsOrBlocks);
			await view.CreateContentDialog(XamlRoot, new ContentDialogParameters() {
				Title = followsOrBlocks ? "Follows" : "Blocks",
				CloseText = "Back",
			}).ShowDialogAsync();

			Local.Listing.Follows = view.GetResult();

			//save to local
			Listing.Write();
		}


	}

	public class TestPageViewModel : BindableBase {
		private string url;

		public string URL {
			get => url;
			set => SetProperty(ref url, value);
		}

		private DelegateCommand testURLCommand;
		public ICommand TestURLCommand => testURLCommand ??= new DelegateCommand(TestURL);

		private async void TestURL() {
			HttpResult<string> content = await NetCode.ReadURLAsync(url);
			Debug.WriteLine(content.Content);
		}

	}
}
