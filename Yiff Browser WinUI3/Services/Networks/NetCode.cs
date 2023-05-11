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
using Yiff_Browser_WinUI3.Services.Locals;

namespace Yiff_Browser_WinUI3.Services.Networks {
	public static class NetCode {
		public const string USERAGENT = "YiffBrowserWinUI3_RainbowWolfer";

		public static async Task<HttpResult<string>> ReadURLAsync(string url, CancellationToken? token = null, string username = "", string api = "") {
			Debug.WriteLine("Reading: " + url);

			DateTime startDateTime = DateTime.Now;
			Stopwatch stopwatch = Stopwatch.StartNew();

			using HttpClient client = new();
			AddDefaultRequestHeaders(client, username, api);

			HttpResponseMessage message = null;
			HttpResultType result;
			HttpStatusCode code;

			string content = null;
			string helper = "";

			try {
				if (token != null) {
					message = await client.GetAsync(url, token.Value);
				} else {
					message = await client.GetAsync(url);
				}
				message.EnsureSuccessStatusCode();
				code = message.StatusCode;
				content = await message.Content.ReadAsStringAsync();

				result = HttpResultType.Success;
			} catch (OperationCanceledException) {
				code = message?.StatusCode ?? HttpStatusCode.NotFound;
				content = null;

				result = HttpResultType.Canceled;
			} catch (HttpRequestException e) {
				code = message?.StatusCode ?? HttpStatusCode.NotFound;
				content = e.Message;
				helper = e.Message;

				result = HttpResultType.Error;
			} finally {
				message?.Dispose();
			}

			stopwatch.Stop();

			HttpResult<string> hr = new(result, code, content, stopwatch.ElapsedMilliseconds, startDateTime, helper);
			//HttpRequestHistories.AddNewItem(startDateTime, url, hr, "Get");

			return hr;
		}

		private static void AddDefaultRequestHeaders(HttpClient client, string username, string api) {
			client.DefaultRequestHeaders.Add("User-Agent", USERAGENT);
			AddAuthorizationHeader(client, username, api);
			//AddAuthorizationHeader(client, "RainbowWolfer", "WUwPNbGDrfXnQoHfvU1nR3TD");
		}

		private static void AddAuthorizationHeader(HttpClient client, string username, string api) {
			if (username.IsBlank() && api.IsBlank()) {
				if (Local.Settings.CheckLocalUser()) {
					username = Local.Settings.Username;
					api = Local.Settings.UserAPI;
				} else {
					return;
				}
			}
			string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + api));
			client.DefaultRequestHeaders.Add("Authorization", "Basic " + encoded);
		}
	}
	public class HttpResult<T> {
		public HttpResultType Result { get; set; }
		public HttpStatusCode StatusCode { get; private set; }
		public T Content { get; private set; }
		public string Helper { get; private set; }
		public long Time { get; private set; }
		public DateTime StartTime { get; private set; }
		public HttpResult(HttpResultType result, HttpStatusCode statusCode, T content, long time, DateTime startTime, string helper = null) {
			Result = result;
			StatusCode = statusCode;
			Content = content;
			Time = time;
			Helper = helper;
			StartTime = startTime;
		}
	}

	public class HttpResultTypeNotFoundException : Exception {
		public HttpResultTypeNotFoundException() : base("") { }
	}

	public enum HttpResultType {
		Success, Error, Canceled
	}
}
