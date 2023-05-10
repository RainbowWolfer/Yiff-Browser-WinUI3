using ColorCode.Compilation.Languages;
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
using System.Threading;
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



		public bool IsOverlayCheck {
			get => (bool)GetValue(IsOverlayCheckProperty);
			set => SetValue(IsOverlayCheckProperty, value);
		}

		public static readonly DependencyProperty IsOverlayCheckProperty = DependencyProperty.Register(
			nameof(IsOverlayCheck),
			typeof(bool),
			typeof(PostImageSideView),
			new PropertyMetadata(true)
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

		private E621Post[] childrenPost;
		private E621Post parentPost;
		private PostPoolItem[] poolItems;

		public E621Post E621Post {
			get => e621Post;
			set => SetProperty(ref e621Post, value, OnPostChanged);
		}

		public ObservableCollection<CommentItem> CommentItems { get; } = new();

		public PostPoolItem[] PoolItems {
			get => poolItems;
			set => SetProperty(ref poolItems, value);
		}
		public E621Post ParentPost {
			get => parentPost;
			set => SetProperty(ref parentPost, value);
		}
		public E621Post[] ChildrenPost {
			get => childrenPost;
			set => SetProperty(ref childrenPost, value);
		}

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

			PoolItems = E621Post.Pools.Select(x => new PostPoolItem(x)).ToArray();

			LoadComments();
			LoadRelations();
		}

		private CancellationTokenSource cts1;
		private CancellationTokenSource cts2;

		private async void LoadRelations() {
			ParentPost = null;
			ChildrenPost = null;

			cts2?.Cancel();
			cts2 = new CancellationTokenSource();

			string parentID = E621Post.Relationships.ParentId;
			List<string> childrenIDs = E621Post.Relationships.Children;

			if (parentID.IsNotBlank()) {
				E621Post parent = await E621API.GetPostAsync(parentID, cts2.Token);
				ParentPost = parent;
			}

			if (childrenIDs.IsNotEmpty()) {
				List<E621Post> list = new();
				foreach (string id in childrenIDs) {
					if (cts2.IsCancellationRequested) {
						return;
					}
					E621Post post = await E621API.GetPostAsync(id, cts2.Token);
					if (post != null) {
						list.Add(post);
					}
				}
				ChildrenPost = list.ToArray();
			}

		}

		private async void LoadComments() {
			cts1?.Cancel();
			cts1 = new CancellationTokenSource();

			IsLoadingComments = true;

			CommentItems.Clear();

			List<Task> tasksPool = new();
			E621Comment[] comments = await E621API.GetCommentsAsync(E621Post.ID, cts1.Token);
			foreach (E621Comment comment in comments ?? Array.Empty<E621Comment>()) {
				CommentItem item = new() {
					E621Comment = comment,
					cts = cts1,
				};
				CommentItems.Add(item);
				Task task = item.LoadUserStuff();
				tasksPool.Insert(0, task);
			}

			LoadPool(tasksPool, cts1);

			RaisePropertyChanged(nameof(CommentItems));
			IsLoadingComments = false;
		}

		private static async void LoadPool(List<Task> tasksPool, CancellationTokenSource cts) {
			foreach (Task item in tasksPool) {
				if (cts.IsCancellationRequested) {
					return;
				}
				await item;
			}
		}
	}


	public class CommentItem : BindableBase {
		public CancellationTokenSource cts;

		private E621Comment e621Comment = null;

		private bool isLoadingAvatar;

		private string userAvatarURL;
		private string username;
		private string levelString;
		private DateTime createdDateTime;
		private int score;

		private string textContent;

		private E621User e621User;
		private E621Post avatarPost;

		public E621Comment E621Comment {
			get => e621Comment;
			set => SetProperty(ref e621Comment, value, OnCommentChanged);
		}

		public E621User E621User {
			get => e621User;
			set => SetProperty(ref e621User, value);
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

		public string LevelString {
			get => levelString;
			set => SetProperty(ref levelString, value);
		}

		public DateTime CreatedDateTime {
			get => createdDateTime;
			set => SetProperty(ref createdDateTime, value);
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

		private void OnCommentChanged() {
			if (E621Comment == null) {
				return;
			}

			Username = E621Comment.creator_name;
			Score = E621Comment.score;
			CreatedDateTime = E621Comment.created_at;
			TextContent = E621Comment.body;

		}

		public async Task LoadUserStuff() {
			IsLoadingAvatar = true;

			E621User = await E621API.GetUserAsync(E621Comment.creator_id, cts.Token);
			if (E621User == null) {
				IsLoadingAvatar = false;
				return;
			}

			LevelString = E621User.level_string;

			E621Post post = await E621API.GetPostAsync(E621User.avatar_id, cts.Token);
			if (post == null) {
				IsLoadingAvatar = false;
				return;
			}

			if (post.HasNoValidURLs()) {
				IsLoadingAvatar = false;
				return;
			}

			UserAvatarURL = post.Sample.URL;

			IsLoadingAvatar = false;
		}

	}

	public class PostPoolItem : BindableBase {
		private string poolID;
		public string PoolID {
			get => poolID;
			set => SetProperty(ref poolID, value);
		}

		public PostPoolItem(string item) {
			this.PoolID = item;
		}

	}
}
