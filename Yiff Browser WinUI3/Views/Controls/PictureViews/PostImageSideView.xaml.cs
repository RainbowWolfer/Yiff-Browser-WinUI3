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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;
using Yiff_Browser_WinUI3.Services.Networks;

namespace Yiff_Browser_WinUI3.Views.Controls.PictureViews {
	public sealed partial class PostImageSideView : UserControl {

		public E621Post E621Post {
			get => (E621Post)GetValue(E621PostProperty);
			set => SetValue(E621PostProperty, value);
		}

		public static readonly DependencyProperty E621PostProperty = DependencyProperty.Register(
			nameof(E621Post),
			typeof(E621Post),
			typeof(PostImageSideView),
			new PropertyMetadata(null)
		);

		public PostImageSideView() {
			this.InitializeComponent();
		}
	}


	public class PostImageSideViewModel : BindableBase {
		private E621Post e621Post;

		private string[] sourceURLs;
		private string description = "No Description";
		private string sourceTitle;
		private bool isLoadingComments;

		public E621Post E621Post {
			get => e621Post;
			set => SetProperty(ref e621Post, value, OnPostChanged);
		}

		public ObservableCollection<CommentItem> CommentItems { get; } = new();

		public string Description {
			get => description;
			set => SetProperty(ref description, value);
		}

		public string[] SourceURLs {
			get => sourceURLs;
			set => SetProperty(ref sourceURLs, value);
		}

		public string SourceTitle {
			get => sourceTitle;
			set => SetProperty(ref sourceTitle, value);
		}

		public bool IsLoadingComments {
			get => isLoadingComments;
			set => SetProperty(ref isLoadingComments, value);
		}

		private void OnPostChanged() {
			Description = E621Post.Description.NotBlankCheck() ?? "No Description";
			SourceURLs = E621Post.Sources.ToArray();
			if (SourceURLs.IsEmpty()) {
				SourceTitle = "No Source";
			} else if (SourceURLs.Length == 1) {
				SourceTitle = "Source";
			} else {
				SourceTitle = "Sources";
			}

			LoadComments();

		}


		private async void LoadComments() {
			IsLoadingComments = true;

			CommentItems.Clear();

			E621Comment[] comments = await E621API.GetCommentsAsync(E621Post.ID);
			foreach (E621Comment item in comments) {
				CommentItems.Add(new CommentItem() {
					E621Comment = item,
				});
			}

			IsLoadingComments = false;
		}
	}


	public class CommentItem : BindableBase {
		private E621Comment e621Comment = null;

		private string userAvatarURL;
		private string username;
		private int score;

		private string textContent;
		private E621User e621User;
		private E621Post avatarPost;
		private bool isLoadingAvatar;

		public E621Comment E621Comment {
			get => e621Comment;
			set => SetProperty(ref e621Comment, value, OnCommentChanged);
		}

		public E621User E621User {
			get => e621User;
			set => SetProperty(ref e621User, value, OnUserChanged);
		}

		public E621Post AvatarPost {
			get => avatarPost;
			set => SetProperty(ref avatarPost, value);
		}

		public string UserAvatarURL {
			get => userAvatarURL;
			set => SetProperty(ref userAvatarURL, value);
		}

		public string Username {
			get => username;
			set => SetProperty(ref username, value);
		}

		public int Score {
			get => score;
			set => SetProperty(ref score, value);
		}

		public string TextContent {
			get => textContent;
			set => SetProperty(ref textContent, value);
		}

		public bool IsLoadingAvatar {
			get => isLoadingAvatar;
			set => SetProperty(ref isLoadingAvatar, value);
		}

		private async void OnCommentChanged() {
			if (E621Comment == null) {
				return;
			}

			Username = E621Comment.creator_name;
			Score = E621Comment.score;
			TextContent = E621Comment.body;

			E621User = await E621API.GetUserAsync(E621Comment.creator_id);
		}

		private void OnUserChanged() {
			if (E621User == null) {
				return;
			}

			//UserAvatarURL= E621Comment.

		}

	}
}
