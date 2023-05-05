using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Yiff_Browser_WinUI3.Services.Locals {
	public static class Local {
		public static LocalSettings Settings { get; set; }
		public static Listing Listing { get; set; }


		public static StorageFolder LocalFolder => ApplicationData.Current.LocalFolder;

		public static async Task Initialize(){

		}
	}
}
