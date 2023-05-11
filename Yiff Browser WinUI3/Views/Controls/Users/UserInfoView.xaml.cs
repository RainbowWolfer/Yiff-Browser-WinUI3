using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Yiff_Browser_WinUI3.Models.E621;
using Prism.Mvvm;
using Yiff_Browser_WinUI3.Services;
using CommunityToolkit.WinUI.Helpers;

namespace Yiff_Browser_WinUI3.Views.Controls.Users {
	public sealed partial class UserInfoView : UserControl {

		public ContentDialog Dialog { get; set; }

		public UserInfoView(E621User user, E621Post avatar) {
			this.InitializeComponent();
			ViewModel.User = user;
			ViewModel.AvatarPost = avatar;

			string yellow = App.IsDarkTheme() ? "#FFD700" : "#F7B000";

			Hello1Text.Foreground = new SolidColorBrush(yellow.ToColor());
			Hello2Text.Foreground = new SolidColorBrush(yellow.ToColor());
		}

		private void HelloGrid_PointerEntered(object sender, PointerRoutedEventArgs e) {

			Hello1TextOpacityAnimation.To = 0;
			Hello2TextOpacityAnimation.To = 1;
			Hello1TransformAnimation.To = -20;
			Hello2TransformAnimation.To = 0;

			HelloTextStoryboard.Begin();
		}

		private void HelloGrid_PointerExited(object sender, PointerRoutedEventArgs e) {

			Hello1TextOpacityAnimation.To = 1;
			Hello2TextOpacityAnimation.To = 0;
			Hello1TransformAnimation.To = 0;
			Hello2TransformAnimation.To = 20;

			HelloTextStoryboard.Begin();
		}

	}

	public class UserInfoViewModel : BindableBase {
		private E621Post avatarPost;
		private E621User user;

		private string helloText1;
		private string helloText2;
		private string username;
		private string levelString;
		private DateTime createdAt;

		private string avatarURL = "/Resources/E621/e612-Bigger.png";
		private string email;
		private string userID;

		public E621User User {
			get => user;
			set => SetProperty(ref user, value, OnUserChanged);
		}

		public E621Post AvatarPost {
			get => avatarPost;
			set => SetProperty(ref avatarPost, value, OnAvatarPostChanged);
		}

		public string Username {
			get => username;
			set => SetProperty(ref username, value);
		}
		public string LevelString {
			get => levelString;
			set => SetProperty(ref levelString, value);
		}
		public string Email {
			get => email;
			set => SetProperty(ref email, value);
		}
		public string UserID {
			get => userID;
			set => SetProperty(ref userID, value);
		}
		public DateTime CreatedAt {
			get => createdAt;
			set => SetProperty(ref createdAt, value);
		}
		public string HelloText1 {
			get => helloText1;
			set => SetProperty(ref helloText1, value);
		}
		public string HelloText2 {
			get => helloText2;
			set => SetProperty(ref helloText2, value);
		}
		public string AvatarURL {
			get => avatarURL;
			set => SetProperty(ref avatarURL, value);
		}

		private void OnUserChanged() {
			KeyValuePair<string, string> pair = HelloInLanguages.GetRandomWelcomePair();
			HelloText1 = pair.Value;
			HelloText2 = $"This is 'Hello' in {pair.Key}";

			if (User == null) {
				return;
			}

			Username = User.name;
			LevelString = User.level_string;
			CreatedAt = User.created_at;
			Email = User.email;
			UserID = User.id.ToString();
		}

		private void OnAvatarPostChanged() {
			if (AvatarPost.HasNoValidURLs()) {
				AvatarURL = "/Resources/E621/e612-Bigger.png";
			} else {
				AvatarURL = AvatarPost.Sample.URL;
			}
		}

	}
}
