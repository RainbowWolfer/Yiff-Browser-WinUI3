using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows.Input;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Views.Controls.PictureViews {
	public sealed partial class SourceHyperLinkButton : UserControl {

		public string URL {
			get => (string)GetValue(URLProperty);
			set => SetValue(URLProperty, value);
		}

		public static readonly DependencyProperty URLProperty = DependencyProperty.Register(
			nameof(URL),
			typeof(string),
			typeof(SourceHyperLinkButton),
			new PropertyMetadata(string.Empty)
		);

		public SourceHyperLinkButton() {
			this.InitializeComponent();
		}
	}

	public class SourceHyperLinkButtonViewModel : BindableBase {
		private string url = string.Empty;
		private string iconPath = string.Empty;

		public string URL {
			get => url;
			set => SetProperty(ref url, value, OnURLChanged);
		}

		public string IconPath {
			get => iconPath;
			set => SetProperty(ref iconPath, value);
		}

		private void OnURLChanged() {
			if (URL.IsBlank()) {
				return;
			}
			string _url = URL;
			if (_url.StartsWith("https://")) {
				_url = _url[8..];
			} else if (_url.StartsWith("http://")) {
				_url = _url[7..];
			}
			string path = string.Empty;
			if (_url.Contains("tumblr")) {//something.tumblr.com
				path = "/Icons/tumblr-icon.png";
			}
			if (_url.StartsWith("twitter") || _url.StartsWith("www.twitter") || _url.StartsWith("pbs.twimg")) {
				path = "/Icons/Twitter-icon.png";
			} else if (_url.StartsWith("www.furaffinity") || _url.StartsWith("furaffinity") || _url.StartsWith("d.furaffinity")) {
				path = "/Icons/Furaffinity-icon.png";
			} else if (_url.StartsWith("www.deviantart") || _url.StartsWith("deviantart")) {
				path = "/Icons/DeviantArt-icon.png";
			} else if (_url.StartsWith("www.inkbunny") || _url.StartsWith("inkbunny")) {
				path = "/Icons/InkBunny-icon.png";
			} else if (_url.StartsWith("www.weasyl.com") || _url.StartsWith("weasyl.com")) {
				path = "/Icons/weasyl-icon.png";
			} else if (_url.StartsWith("www.pixiv") || _url.StartsWith("pixiv")) {
				path = "/Icons/Pixiv-icon.png";
			} else if (_url.StartsWith("www.instagram") || _url.StartsWith("instagram")) {
				path = "/Icons/Instagram-icon.png";
			} else if (_url.StartsWith("www.patreon") || _url.StartsWith("patreon")) {
				path = "/Icons/Patreon-icon.png";
			} else if (_url.StartsWith("www.subscribestar") || _url.StartsWith("subscribestar")) {
				path = "/Icons/SubscribeStar-icon.png";
			} else if (_url.StartsWith("mega")) {
				path = "/Icons/Mega-icon.png";
			} else if (_url.StartsWith("furrynetwork")) {
				path = "/Icons/FurryNetwork-icon.png";
			} else if (_url.StartsWith("t.me")) {
				path = "/Icons/Telegram-icon.png";
			} else if (_url.StartsWith("newgrounds") || _url.StartsWith("www.newgrounds")) {
				path = "/Icons/NewGrounds-icon.png";
			}

			IconPath = path;
		}

		public ICommand ClickCommand => new DelegateCommand(Click);
		public ICommand CopyCommand => new DelegateCommand(Copy);

		private void Click() {
			URL.OpenInBrowser();
		}

		private void Copy() {
			URL.CopyToClipboard();
		}
	}
}
