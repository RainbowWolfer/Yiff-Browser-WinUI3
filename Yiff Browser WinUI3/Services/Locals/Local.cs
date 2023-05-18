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
		public static StorageFile SettingsFile { get; private set; }

		public static async Task Initialize() {
			Debug.WriteLine(LocalFolder.Path);

			await Task.Run(async () => {
				ListingFile = await LocalFolder.CreateFileAsync("Listings.json", CreationCollisionOption.OpenIfExists);
				SettingsFile = await LocalFolder.CreateFileAsync("Settings.json", CreationCollisionOption.OpenIfExists);

				await Listing.Read();
				await LocalSettings.Read();
			});
		}



		public static async Task<string> ReadFile(IStorageFile file) {
			try {
				return await FileIO.ReadTextAsync(file);
			} catch (Exception ex) {
				Debug.WriteLine(ex);
				Debugger.Break();
				return null;
			}
		}

		public static async Task WriteFile(IStorageFile file, string content) {
			try {
				await FileIO.WriteTextAsync(file, content);
			} catch (Exception ex) {
				Debug.WriteLine(ex);
				Debugger.Break();
			}
		}

	}
}
