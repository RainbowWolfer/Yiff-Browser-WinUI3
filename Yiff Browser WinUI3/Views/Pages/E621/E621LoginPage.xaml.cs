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
using System.Windows.Input;
using Prism.Commands;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Yiff_Browser_WinUI3.Views.Controls.UserHelperViews;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Views.Pages.E621 {
	public sealed partial class E621LoginPage : Page {
		public E621LoginPage() {
			this.InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {
			UsernameText.Focus(FocusState.Programmatic);
		}

		private void UsernameTextEnter_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
			ApiText.Focus(FocusState.Programmatic);
		}

		private void ApiTextEnter_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
			ViewModel.Submit();
		}

		private async void Help_Click(object sender, RoutedEventArgs e) {
			await new LoginHelpSection().CreateContentDialog(XamlRoot, new ContentDialogParameters() {
				Title = "Help",
				CloseText = "Back",
			}).ShowDialogAsync();
		}
	}

	public class E621LoginPageViewModel : BindableBase {
		private string userName;
		private string api;

		public string UserName {
			get => userName;
			set => SetProperty(ref userName, value);
		}

		public string API {
			get => api;
			set => SetProperty(ref api, value);
		}

		public ICommand PasteCommand => new DelegateCommand(Paste);

		public ICommand SubmitCommand => new DelegateCommand(Submit);

		public ICommand HelpInBrowserCommand => new DelegateCommand<string>(HelpInBrowser);

		private async void HelpInBrowser(string address) {
			_ = await Launcher.LaunchUriAsync(new Uri(address));
		}

		public void Submit() {
			
		}

		private async void Paste() {
			DataPackageView dataPackageView = Clipboard.GetContent();
			if (dataPackageView.Contains(StandardDataFormats.Text)) {
				string text = await dataPackageView.GetTextAsync();
				API = text;
			}

		}

	}
}
