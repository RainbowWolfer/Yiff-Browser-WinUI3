using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Yiff_Browser_WinUI3.Services.Locals {
	public static class Local {
		public static LocalSettings Settings { get; set; }
		public static Listing Listing { get; set; }


		public static StorageFolder LocalFolder => ApplicationData.Current.LocalFolder;


		public static StorageFile ListingFile { get; private set; }

		public static async Task Initialize() {
			Debug.WriteLine(LocalFolder.Path);

			ListingFile = await LocalFolder.CreateFileAsync("Listings.json", CreationCollisionOption.OpenIfExists);

			await Listing.Read();

		}



		public static async Task<string> ReadFile(IStorageFile file) {
			return await FileIO.ReadTextAsync(file);
		}

		public static async Task WriteFile(IStorageFile file, string content) {
			await FileIO.WriteTextAsync(file, content);
		}

	}
}
