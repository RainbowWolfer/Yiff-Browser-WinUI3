using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;

namespace Yiff_Browser_WinUI3.Views.Pages.E621 {
	public sealed partial class DownloadPage : Page {

		public DownloadPage() {
			this.InitializeComponent();
		}

	}

	public class DownloadPageViewModel : BindableBase {

		public DownloadPageViewModel() {
			Load();
		}

		private async void Load() {
			//Uri source = new Uri("https://example.com/image.jpg");
			//{
			//	// Create a StorageFile for the destination file
			//	StorageFolder downloadsFolder = await KnownFolders.GetFolderForUserAsync(null /* current user */, KnownFolderId.PicturesLibrary);
			//	StorageFile destinationFile = await downloadsFolder.CreateFileAsync("image.jpg", CreationCollisionOption.GenerateUniqueName);
			//	BackgroundTransferCompletionGroup completionGroup = new();
			//	DownloadOperation download = new DownloadOperation(source, destinationFile, completionGroup);
			//	await download.StartAsync();
			//}
			//{
			//	BackgroundDownloader downloadDownloader = new(completionGroup);
			//	await downloadDownloader.CreateDownloadAsync();
			//	//downloadDownloader.
			//}
			//{
			//	BackgroundTransferGroup group = BackgroundTransferGroup.CreateGroup("Default");
			//	group.TransferBehavior = BackgroundTransferBehavior.Serialized;
			//	BackgroundDownloader downloader = new() {
			//		TransferGroup = group,
			//	};
			//	DownloadOperation download = downloader.CreateDownload();
			//	await download.StartAsync();

			//	IReadOnlyList<DownloadOperation> v = await BackgroundDownloader.GetCurrentDownloadsForTransferGroupAsync(group);
			//	foreach (var item in v) {
			//		item.Pause();
			//	}
			//}

			//{


			//	BackgroundDownloader downloader = new BackgroundDownloader();
			//	DownloadOperation download = downloader.CreateDownload(source, destinationFile);
			//	//download.Pause
			//}
		}
	}
}
