using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;
using Yiff_Browser_WinUI3.Services.Locals;
using Yiff_Browser_WinUI3.Services.Networks;
using Yiff_Browser_WinUI3.Views.Controls.LoadingViews;
using Yiff_Browser_WinUI3.Views.Controls.Users;
using Yiff_Browser_WinUI3.Views.Pages;
using Yiff_Browser_WinUI3.Views.Pages.E621;

namespace Yiff_Browser_WinUI3 {
	public sealed partial class MainWindow : Window {
		public static MainWindow Instance { get; private set; }
		public static LoadingDialogControl LoaderControl { get; private set; }

		public string TAG_HOME { get; } = "TAG_HOME";
		public string TAG_FAVORITES { get; } = "TAG_FAVORITES";
		public string TAG_FOLLOWS { get; } = "TAG_FOLLOWS";
		public string TAG_DOWNLOADS { get; } = "TAG_DOWNLOADS";
		public string TAG_USER { get; } = "TAG_USER";

		private string userAvatarURL;
		private string usernameText;

		public string UserAvatarURL {
			get => userAvatarURL;
			set {
				userAvatarURL = value;
				if (value.IsBlank()) {
					UserAvatarPicture.ProfilePicture = new BitmapImage(new Uri("ms-appx:///Resources/E621/e612-Bigger.png"));
				} else {
					UserAvatarPicture.ProfilePicture = new BitmapImage(new Uri(value));
				}
			}
		}

		public string UsernameText {
			get => usernameText;
			set {
				usernameText = value;
				UserUsernameTextBlock.Text = value.NotBlankCheck() ?? "Account";
				ToolTipService.SetToolTip(UserButton, UserUsernameTextBlock.Text);
			}
		}

		public MainWindow() {
			Instance = this;
			this.InitializeComponent();
		}

		private void Root_Loaded(object sender, RoutedEventArgs e) {
			Initialize();
		}

		private async void Initialize() {
			LoadingRingWithTextBelow loader = new("Loading local stuff");

			LoaderControl = new(Root.XamlRoot, loader);

			static async Task Init() => await Local.Initialize();

			await LoaderControl.Start(Init);

			NavigateHome();

			LoadLocalUser();
		}

		private async void LoadLocalUser() {
			if (Local.Settings.CheckLocalUser()) {

				E621User user = await E621API.GetUserAsync(Local.Settings.Username);
				UsernameText = user.name;
				E621Post avatarPost = await E621API.GetPostAsync(user.avatar_id);

				App.User = user;
				App.AvatarPost = avatarPost;
				if (avatarPost != null && !avatarPost.HasNoValidURLs()) {
					UserAvatarURL = avatarPost.Sample.URL;
				} else {
					UserAvatarURL = null;
				}

			} else {
				UserAvatarURL = null;
				UsernameText = null;
			}
		}


		private void MainNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args) {
			NavigationViewItem item = args.InvokedItemContainer as NavigationViewItem;
			string tag = item.Tag as string;

			Type targetType;
			if (args.IsSettingsInvoked) {
				targetType = typeof(SettingsPage);
			} else {
				if (tag == TAG_HOME) {
					targetType = typeof(E621HomePage);
				} else if (tag == TAG_USER) {
					targetType = typeof(E621LoginPage);
				} else {
					targetType = typeof(TestPage);
					//throw new NotSupportedException($"{tag}");
				}
			}
			MainFrame.Navigate(targetType, null, args.RecommendedNavigationTransitionInfo);
		}


		public void NavigateHome() {
			MainNavigationView.SelectedItem = ItemHome;
			MainFrame.Navigate(typeof(E621HomePage), null, new EntranceNavigationTransitionInfo());
		}

		private async void UserButton_Click(object sender, RoutedEventArgs e) {
			if (Local.Settings.CheckLocalUser()) {
				if (App.User == null) {
					return;
				}

				UserInfoView view = new(App.User, App.AvatarPost);
				view.OnAvatarRefreshed += (s, e) => {
					UserAvatarURL = e;
				};

				ContentDialog dialog = view.CreateContentDialog(Root.XamlRoot, new ContentDialogParameters() {
					CloseText = "Back",
				});
				view.Dialog = dialog;
				dialog.Closing += (s, e) => {
					if (view.IsRefreshing) {
						e.Cancel = true;
					}
				};

				await dialog.ShowDialogAsync();

			} else {
				LoginView view = new();

				ContentDialog dialog = view.CreateContentDialog(Root.XamlRoot, new ContentDialogParameters() {
					SkipWidthSet = true,
					Title = "Sign in",
					PrimaryText = "Submit",
					CloseText = "Back",
					DefaultButton = ContentDialogButton.Primary,
				});

				view.Dialog = dialog;

				dialog.Closing += (s, e) => {
					if (e.Result == ContentDialogResult.Primary) {
						view.Submit();
						e.Cancel = true;
					} else if (e.Result == ContentDialogResult.None) {
						if (view.IsLoading) {
							e.Cancel = true;
						}
					}
				};

				await dialog.ShowDialogAsync();

				if (Local.Settings.CheckLocalUser()) {
					(E621User user, E621Post avatarPost) = view.GetUserResult();
					App.User = user;
					UsernameText = user.name;
					App.AvatarPost = avatarPost;
					if (avatarPost != null && !avatarPost.HasNoValidURLs()) {
						UserAvatarURL = avatarPost.Sample.URL;
					} else {
						UserAvatarURL = null;
					}
				}
			}
		}

		private void MainNavigationView_PaneOpening(NavigationView sender, object args) {
			UserButton.Width = 312;
		}

		private void MainNavigationView_PaneClosing(NavigationView sender, NavigationViewPaneClosingEventArgs args) {
			UserButton.Width = 40;
		}

	}

	public class MainWindowViewModel : BindableBase {

	}
}
