using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Models.E621 {
	public class E621Post {

		public string id;
		public DateTime? created_at;
		public DateTime? updated_at;
		public ArticleFile file;
		public Preview preview;
		public Sample sample;
		public Score score;
		public Tags tags;
		public List<string> locked_tags;
		public int change_seq;
		public Flags flags;
		public E621Rating rating;
		public int fav_count;
		public List<string> sources;
		public List<string> pools;
		public Relationships relationships;
		public int? approver_id;
		public int uploader_id;
		public string description;
		public int comment_count;
		public bool is_favorited;
		public bool has_notes;
		public string duration;


		public bool HasNoValidURLs() => preview.url.IsBlank() || sample.url.IsBlank() || file.url.IsBlank();


		public override string ToString() {
			return $"E621Post ({id}.{file.ext})";
		}

	}

	public class E621PostsRoot {
		public List<E621Post> posts;
	}

	public class ArticleFile {
		public int width;
		public int height;
		public string ext;
		public int size;
		public string md5;
		public string url;
	}

	public class Preview {
		public int width;
		public int height;
		public string url;
	}

	public class Alternates {
	}

	public class Sample {
		public bool has;
		public int height;
		public int width;
		public string url;
		public Alternates alternates;
	}

	public class Score {
		public int up;
		public int down;
		public int total;
	}

	public class Tags {
		public List<string> general;
		public List<string> species;
		public List<string> character;
		public List<string> copyright;
		public List<string> artist;
		public List<string> invalid;
		public List<string> lore;
		public List<string> meta;

		public List<string> GetAllTags() {
			List<string> result = new();
			general.ForEach(s => result.Add(s));
			species.ForEach(s => result.Add(s));
			character.ForEach(s => result.Add(s));
			copyright.ForEach(s => result.Add(s));
			artist.ForEach(s => result.Add(s));
			invalid.ForEach(s => result.Add(s));
			lore.ForEach(s => result.Add(s));
			meta.ForEach(s => result.Add(s));
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
		public string parent_id;
		public bool has_children;
		public bool has_active_children;
		public List<string> children;
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
