﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yiff_Browser_WinUI3.Helpers;
using Yiff_Browser_WinUI3.Models.E621;
using Yiff_Browser_WinUI3.Services.Locals;

namespace Yiff_Browser_WinUI3.Services.Networks {
	public static class E621API {
		public static string GetHost() => Local.Settings?.E621Yiff ?? false ? "e621.net" : "e926.net";

		public static int GetPostsPerPageCount() => Local.Settings?.E621PageLimitCount ?? 75;

		#region Posts
		public static async Task<E621Post[]> GetPostsByTagsAsync(E621PostParameters parameters) {
			if (parameters.Page <= 0) {
				parameters.Page = 1;
			}
			string url = $"https://{GetHost()}/posts.json?page={parameters.Page}{(parameters.UsePageLimit ? $"&limit={GetPostsPerPageCount()}" : "")}";

			IEnumerable<string> tags = parameters.Tags.Where(x => x.IsNotBlank());
			if (tags.IsNotEmpty()) {
				url += "&tags=";
				url += string.Join("+", tags);
			}

			HttpResult<string> result = await NetCode.ReadURLAsync(url);

			if (result.Result == HttpResultType.Success) {
				return JsonConvert.DeserializeObject<E621PostsRoot>(result.Content)?.Posts.ToArray() ?? Array.Empty<E621Post>();
			} else {
				return null;
			}
		}

		public static async Task<E621Post> GetPostAsync(string postID, CancellationToken? token = null) {
			if (postID.IsBlank()) {
				return null;
			}
			string url = $"https://{GetHost()}/posts/{postID}.json";
			HttpResult<string> result = await NetCode.ReadURLAsync(url, token);
			if (result.Result == HttpResultType.Success) {
				E621Post post = JsonConvert.DeserializeObject<E621PostsRoot>(result.Content).Post;
				return post;
			} else {
				return null;
			}
		}

		#endregion

		#region Tags

		public static async Task<E621AutoComplete[]> GetE621AutoCompleteAsync(string tag, CancellationToken? token = null) {
			HttpResult<string> result = await NetCode.ReadURLAsync($"https://{GetHost()}/tags/autocomplete.json?search[name_matches]={tag}", token);
			if (result.Result == HttpResultType.Success) {
				return JsonConvert.DeserializeObject<E621AutoComplete[]>(result.Content);
			} else {
				return null;
			}
		}

		public static async Task<E621Tag> GetE621TagAsync(string tag, CancellationToken? token = null) {
			tag = tag.ToLower().Trim();

			if (E621Tag.Pool.TryGetValue(tag, out E621Tag e621Tag)) {
				return e621Tag;
			}

			string url = $"https://{GetHost()}/tags.json?search[name_matches]={tag}";
			HttpResult<string> result = await NetCode.ReadURLAsync(url, token);
			if (result.Result == HttpResultType.Success && result.Content != "{\"tags\":[]}") {
				E621Tag t = JsonConvert.DeserializeObject<E621Tag[]>(result.Content)?.FirstOrDefault();
				if (t != null) {
					E621Tag.Pool.TryAdd(tag, t);
				}
				return t;
			} else {
				return null;
			}
		}

		public static async Task<E621Wiki> GetE621WikiAsync(string tag, CancellationToken? token = null) {
			tag = tag.ToLower().Trim();

			if (E621Wiki.wikiDictionary.ContainsKey(tag)) {
				return new E621Wiki() {
					body = E621Wiki.wikiDictionary[tag]
				};
			} else if (tag.StartsWith("fav:")) {
				return new E621Wiki() {
					body = $"Favorites of \"{tag[4..]}\"",
				};
			}

			if (E621Wiki.Pool.TryGetValue(tag, out E621Wiki e621Wiki)) {
				return e621Wiki;
			}

			string url = $"https://{GetHost()}/wiki_pages.json?search[title]={tag}";
			HttpResult<string> result = await NetCode.ReadURLAsync(url, token);
			if (result.Result == HttpResultType.Success) {
				if (result.Content == "[]") {
					return new E621Wiki();
				}
				return JsonConvert.DeserializeObject<E621Wiki[]>(result.Content).FirstOrDefault();
			} else {
				return null;
			}
		}

		public static async Task<bool> UploadBlacklistTags(string username, string[] tags) {
			HttpResult<string> result = await NetCode.PutRequestAsync(
				$"https://{GetHost()}/users/{username}.json",
				new KeyValuePair<string, string>("user[blacklisted_tags]", string.Join("\n", tags))
			);
			return result.Result == HttpResultType.Success;
		}

		#endregion

		#region Comments
		public static async Task<E621Comment[]> GetCommentsAsync(int postID, CancellationToken? token = null) {
			string url = $"https://{GetHost()}/comments.json?group_by=comment&search[post_id]={postID}";
			HttpResult<string> result = await NetCode.ReadURLAsync(url, token);
			if (result?.Content == "{\"comments\":[]}") {
				return Array.Empty<E621Comment>();
			}
			if (result.Result == HttpResultType.Success) {
				return JsonConvert.DeserializeObject<E621Comment[]>(result.Content);
			} else {
				return null;
			}
		}

		#endregion


		#region Pool

		public static async Task<E621Pool> GetPoolAsync(string id, CancellationToken? token = null) {
			HttpResult<string> result = await NetCode.ReadURLAsync($"https://{GetHost()}/pools/{id}.json", token);
			if (result.Result == HttpResultType.Success) {
				return JsonConvert.DeserializeObject<E621Pool>(result.Content);
			} else {
				return null;
			}
		}

		#endregion

		#region Users

		public static async Task<E621User> GetUserAsync(string username, CancellationToken? token = null) {
			string url = $"https://{GetHost()}/users.json?search[name_matches]={username}";
			HttpResult<string> result = await NetCode.ReadURLAsync(url, token);
			if (result.Result == HttpResultType.Success) {
				return JsonConvert.DeserializeObject<E621User[]>(result.Content).FirstOrDefault();
			} else {
				return null;
			}
		}

		public static async Task<E621User> GetUserAsync(int id, CancellationToken? token = null) {
			string url = $"https://{GetHost()}/users.json?search[id]={id}";
			HttpResult<string> result = await NetCode.ReadURLAsync(url, token);
			if (result.Result == HttpResultType.Success) {
				return JsonConvert.DeserializeObject<E621User[]>(result.Content).FirstOrDefault();
			} else {
				return null;
			}
		}

		public static async Task<HttpResult<string>> PostAddFavoriteAsync(int postID, CancellationToken? token = null) {
			string url = $"https://{GetHost()}/favorites.json";
			return await NetCode.PostRequestAsync(url, new List<KeyValuePair<string, string>>() {
				new KeyValuePair<string, string>("post_id", postID.ToString())
			}, token);
		}

		public static async Task<HttpResult<string>> PostDeleteFavoriteAsync(int postID ,CancellationToken? token = null) {
			string url = $"https://{GetHost()}/favorites/{postID}.json";
			return await NetCode.DeleteRequestAsync(url, token);
		}

		public static async Task<DataResult<E621Vote>> VotePost(int postID, int score, bool no_unvote, CancellationToken? token = null) {
			HttpResult<string> result = await NetCode.PostRequestAsync($"https://{GetHost()}/posts/{postID}/votes.json", new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("score", $"{score}"),
				new KeyValuePair<string, string>("no_unvote", $"{no_unvote}"),
			}, token);
			return new DataResult<E621Vote>(result.Result, JsonConvert.DeserializeObject<E621Vote>(result.Content));
		}

		// no up and down
		public static async Task<DataResult<E621Vote>> VoteComment(int commentID, int score, bool no_unvote, CancellationToken? token = null) {
			HttpResult<string> result = await NetCode.PostRequestAsync($"https://{GetHost()}/comments/{commentID}/votes.json", new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("score", $"{score}"),
				new KeyValuePair<string, string>("no_unvote", $"{no_unvote}"),
			}, token);
			return new DataResult<E621Vote>(result.Result, JsonConvert.DeserializeObject<E621Vote>(result.Content));
		}

		#endregion
	}

	public class E621PostParameters {
		public event Action<string[]> OnPreviewsUpdated;

		public int Page { get; set; } = 1;
		public string[] Tags { get; set; } = { "" };
		public bool UsePageLimit { get; set; } = true;


		public bool InputPosts { get; set; }
		public E621Post[] Posts { get; set; }

		public E621Pool Pool { get; set; }

	}

}