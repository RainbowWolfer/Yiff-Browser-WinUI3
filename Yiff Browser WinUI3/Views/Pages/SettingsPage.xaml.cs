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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.ViewManagement;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Services.Locals;

namespace Yiff_Browser_WinUI3.Views.Pages {
	public sealed partial class SettingsPage : Page {
		public SettingsPage() {
			this.InitializeComponent();
		}
	}

	public class SettingsPageViewModel : BindableBase {

		private string downloadFolderPath;
		public string DownloadFolderPath {
			get => downloadFolderPath;
			set => SetProperty(ref downloadFolderPath, value);
		}

		public ICommand ClearDownloadFolderCommand => new DelegateCommand(ClearDownloadFolder);
		public ICommand SelectDownloadFolderCommand => new DelegateCommand(SelectDownloadFolder);

		private void ClearDownloadFolder() {
			Local.Settings.ClearDownloadFolder();
		}

		private async void SelectDownloadFolder() {
			FolderPicker pick = WindowHelper.CreateFolderPicker("*");
			StorageFolder folder = await pick.PickSingleFolderAsync();
			if (folder != null) {
				Local.Settings.SetDownloadFolder(folder);
			}
		}

		public ICommand OpenAppLocalFolderCommand => new DelegateCommand(OpenAppLocalFolder);

		private async void OpenAppLocalFolder() {
			await Launcher.LaunchFolderAsync(Local.LocalFolder, new FolderLauncherOptions() {
				DesiredRemainingView = ViewSizePreference.UseMore,
			});
		}
	}
}
