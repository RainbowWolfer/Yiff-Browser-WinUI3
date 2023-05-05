using Newtonsoft.Json;
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
		public static async Task<E621Post[]> GetE621PostsByTagsAsync(E621PostParameters parameters) {
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
			if (E621Tag.Pool.TryGetValue(tag, out E621Tag e621Tag)) {
				return e621Tag;
			}

			tag = tag.ToLower().Trim();
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

		#endregion
	}

	public class E621PostParameters {
		public event Action<string[]> OnPreviewsUpdated;

		public int Page { get; set; } = 1;
		public string[] Tags { get; set; } = { "" };
		public bool UsePageLimit { get; set; } = true;
	}

}