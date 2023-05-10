using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Yiff_Browser_WinUI3.Services.Locals {
	public class LocalSettings {
		public bool E621Yiff { get; set; } = true;
		public int E621PageLimitCount { get; set; } = 75;


		//"brazhnik"
		public string[] StartupTags { get; set; } = { "order:rank" };

		public bool EnanbleTransitionAnimation { get; set; } = true;

		public int ImageAdaptWidth { get; set; } = 380;
		public int ImageAdaptHeight { get; set; } = 50;

		public string Username { get; set; } = "";
		public string UserAPI { get; set; } = "";

		public string LocalFolderToken { get; set; }


		#region Local

		public static async Task Read() {
			string json = await Local.ReadFile(Local.SettingsFile);
			Local.Settings = JsonConvert.DeserializeObject<LocalSettings>(json) ?? new LocalSettings();
		}

		public static async void Write() {
			string json = JsonConvert.SerializeObject(Local.Settings, Formatting.Indented);
			await Local.WriteFile(Local.SettingsFile, json);
		}

		#endregion

	}
}
