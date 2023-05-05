using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Models.E621 {
	public class E621Post {

		[JsonProperty("id")]
		public int ID { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty("file")]
		public File File { get; set; }

		[JsonProperty("preview")]
		public Preview Preview { get; set; }

		[JsonProperty("sample")]
		public Sample Sample { get; set; }

		[JsonProperty("score")]
		public Score Score { get; set; }

		[JsonProperty("tags")]
		public Tags Tags { get; set; }

		[JsonProperty("locked_tags")]
		public List<object> LockedTags { get; set; }

		[JsonProperty("change_seq")]
		public int ChangeSeq { get; set; }

		[JsonProperty("flags")]
		public Flags Flags { get; set; }

		[JsonProperty("rating")]
		public string Rating { get; set; }

		[JsonProperty("fav_count")]
		public int FavCount { get; set; }

		[JsonProperty("sources")]
		public List<string> Sources { get; set; }

		[JsonProperty("pools")]
		public List<object> Pools { get; set; }

		[JsonProperty("relationships")]
		public Relationships Relationships { get; set; }

		[JsonProperty("approver_id")]
		public object ApproverId { get; set; }

		[JsonProperty("uploader_id")]
		public int UploaderId { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("comment_count")]
		public int CommentCount { get; set; }

		[JsonProperty("is_favorited")]
		public bool IsFavorited { get; set; }

		[JsonProperty("has_notes")]
		public bool HasNotes { get; set; }

		[JsonProperty("duration")]
		public object Duration { get; set; }


		public bool HasNoValidURLs() => Preview.URL.IsBlank() || Sample.URL.IsBlank() || File.URL.IsBlank();


		public override string ToString() {
			return $"E621Post ({ID}.{File.Ext})";
		}

	}

	public class E621PostsRoot {
		[JsonProperty("posts")]
		public List<E621Post> Posts { get; set; }
	}

	public class File {
		[JsonProperty("width")]
		public int Width { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("ext")]
		public string Ext { get; set; }

		[JsonProperty("size")]
		public int Size { get; set; }

		[JsonProperty("md5")]
		public string Md5 { get; set; }

		[JsonProperty("url")]
		public string URL { get; set; }
	}

	public class Preview {
		[JsonProperty("width")]
		public int Width { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("url")]
		public string URL { get; set; }
	}

	public class Alternates {
	}

	public class Sample {
		[JsonProperty("has")]
		public bool Has { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("width")]
		public int Width { get; set; }

		[JsonProperty("url")]
		public string URL { get; set; }

		[JsonProperty("alternates")]
		public Alternates Alternates { get; set; }
	}


	public class Score {
		[JsonProperty("up")]
		public int Up { get; set; }

		[JsonProperty("down")]
		public int Down { get; set; }

		[JsonProperty("total")]
		public int Total { get; set; }
	}

	public class Tags {
		[JsonProperty("general")]
		public List<string> General { get; set; }

		[JsonProperty("species")]
		public List<string> Species { get; set; }

		[JsonProperty("character")]
		public List<string> Character { get; set; }

		[JsonProperty("copyright")]
		public List<string> Copyright { get; set; }

		[JsonProperty("artist")]
		public List<string> Artist { get; set; }

		[JsonProperty("invalid")]
		public List<string> Invalid { get; set; }

		[JsonProperty("lore")]
		public List<string> Lore { get; set; }

		[JsonProperty("meta")]
		public List<string> Meta { get; set; }

		public List<string> GetAllTags() {
			List<string> result = new();
			General.ForEach(s => result.Add(s));
			Species.ForEach(s => result.Add(s));
			Character.ForEach(s => result.Add(s));
			Copyright.ForEach(s => result.Add(s));
			Artist.ForEach(s => result.Add(s));
			Invalid.ForEach(s => result.Add(s));
			Lore.ForEach(s => result.Add(s));
			Meta.ForEach(s => result.Add(s));
			return result;
		}

	}

	public class Flags {
		public bool pending;
		public bool flagged;
		public bool note_locked;
		public bool status_locked;
		public bool rating_locked;
		public bool deleted;
	}

	public class Relationships {
		[JsonProperty("parent_id")]
		public object ParentId { get; set; }

		[JsonProperty("has_children")]
		public bool HasChildren { get; set; }

		[JsonProperty("has_active_children")]
		public bool HasActiveChildren { get; set; }

		[JsonProperty("children")]
		public List<object> Children { get; set; }
	}

	public enum E621Rating {
		[EnumMember(Value = "s")]
		Safe,
		[EnumMember(Value = "q")]
		Questionable,
		[EnumMember(Value = "e")]
		Explict
	}
}
