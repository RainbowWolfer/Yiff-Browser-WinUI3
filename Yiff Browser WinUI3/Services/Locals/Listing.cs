using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		}

		public Listing() {
			Follows = new List<ListingItem>();
			Blocks = new List<ListingItem>();
			FollowPools = new List<int>();
		}
	}

	public class ListingItem {
		public string Name { get; set; }
		public List<string> Tags { get; set; }

		public bool IsCloud { get; set; }

		public ListingItem(string name) {
			Name = name;
			Tags = new List<string>();
		}
	}
}
