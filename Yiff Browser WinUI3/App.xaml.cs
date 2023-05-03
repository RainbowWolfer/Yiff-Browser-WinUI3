using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Yiff_Browser_WinUI3.Services.Locals;

namespace Yiff_Browser_WinUI3 {
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public partial class App : Application {
		#region Style

		public static Style DialogStyle => Current.Resources["DefaultContentDialogStyle"] as Style;
		public static Brush TextBoxDefaultBorderBrush => (Brush)Current.Resources["TextControlBorderBrush"];

		#endregion

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App() {
			this.InitializeComponent();
			Local.Settings = new LocalSettings();
			Local.Listing = new Listing();
		}

		/// <summary>
		/// Invoked when the application is launched.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override void OnLaunched(LaunchActivatedEventArgs args) {
			MainWindow window = new();
			window.Activate();
		}

		[DllImport("UXTheme.dll", SetLastError = true, EntryPoint = "#138")]
		private static extern bool ShouldSystemUseDarkMode();

		public static ApplicationTheme GetApplicationTheme() {
			return ShouldSystemUseDarkMode() ? ApplicationTheme.Dark : ApplicationTheme.Dark;
		}
	}
}
