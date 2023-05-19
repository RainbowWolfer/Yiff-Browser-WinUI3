using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;

namespace Yiff_Browser_WinUI3.Services.Downloads {
	public class DownloadManager {
		public static DownloadManager Instance { get; private set; }


		private BackgroundTransferGroup TransferGroup { get; init; }
		private BackgroundDownloader Downloader { get; init; }

		public DownloadManager() {
			Instance = this;
			TransferGroup = BackgroundTransferGroup.CreateGroup("Default");
			TransferGroup.TransferBehavior = BackgroundTransferBehavior.Serialized;
			Downloader = new BackgroundDownloader() {
				TransferGroup = TransferGroup,
			};
		}

		public void Download(string url) {
			//Downloader.CreateDownload(new Uri(url),);
		}


	}
}
