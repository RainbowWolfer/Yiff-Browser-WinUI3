using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Services.Locals {
	public class Listing {
		public List<ListingItem> Follows { get; set; }
		public List<ListingItem> Blocks { get; set; }

		public List<int> FollowPools { get; set; }

		[JsonConstructor]
		public Listing(List<ListingItem> follows, List<ListingItem> blocks, List<int> followPools) {
			Follows = follows ?? new List<ListingItem>();
			Blocks = blocks ?? new List<ListingItem>();
			FollowPools = followPools ?? new List<int>();

			Preinflate();
		}

		public Listing() {
			Follows = new List<ListingItem>();
			Blocks = new List<ListingItem>();
			FollowPools = new List<int>();

			Preinflate();
		}

		public void Preinflate() {
			if (Follows.IsEmpty()) {
				Follows.Add(new ListingItem("Default") {
					IsActive = true,
				});
			}
			if (Blocks.IsEmpty()) {
				Blocks.Add(new ListingItem("Default") {
					IsActive = true,
				});
			}
		}

		public ListingItem GetActiveFollows() => Follows.Find(x => x.IsActive);
		public ListingItem GetActiveBlocks() => Blocks.Find(x => x.IsActive);

		public bool ContainFollows(string tag) {
			ListingItem list = GetActiveFollows();
			return list.Tags.Contains(tag.Trim().ToLower());
		}

		public bool ContainBlocks(string tag) {
			ListingItem list = GetActiveBlocks();
			return list.Tags.Contains(tag.Trim().ToLower());
		}

		public void AddFollow(string tag) {
			ListingItem list = GetActiveFollows();
			string _tag = tag.Trim().ToLower();
			if (!list.Tags.Contains(_tag)) {
				list.Tags.Add(_tag);
			}
		}

		public void RemoveFollow(string tag) {
			ListingItem list = GetActiveFollows();
			string _tag = tag.Trim().ToLower();
			if (list.Tags.Contains(_tag)) {
				list.Tags.Remove(_tag);
			}
		}

		public void AddBlock(string tag) {
			ListingItem list = GetActiveBlocks();
			string _tag = tag.Trim().ToLower();
			if (!list.Tags.Contains(_tag)) {
				list.Tags.Add(_tag);
			}
		}

		public void RemoveBlock(string tag) {
			ListingItem list = GetActiveBlocks();
			string _tag = tag.Trim().ToLower();
			if (list.Tags.Contains(_tag)) {
				list.Tags.Remove(_tag);
			}
		}



		#region Local

		public static async Task Read() {
			string json = await Local.ReadFile(Local.ListingFile);
			Local.Listing = JsonConvert.DeserializeObject<Listing>(json) ?? new Listing();
		}

		public static async void Write() {
			string json = JsonConvert.SerializeObject(Local.Listing, Formatting.Indented);
			await Local.WriteFile(Local.ListingFile, json);
		}

		#endregion

	}

	public class ListingItem {
		public string Name { get; set; }
		public List<string> Tags { get; set; }

		public bool IsCloud { get; set; }

		public bool IsActive { get; set; }

		public ListingItem(string name) {
			Name = name;
			Tags = new List<string>();
		}
	}
}
