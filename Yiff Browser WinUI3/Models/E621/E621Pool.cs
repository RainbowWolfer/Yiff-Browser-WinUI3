﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yiff_Browser_WinUI3.Models.E621 {
	public class E621Pool {

		[JsonProperty("id")]
		public int ID { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty("creator_id")]
		public int CreatorID { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("is_active")]
		public bool IsActive { get; set; }

		[JsonProperty("category")]
		public string Category { get; set; }

		[JsonProperty("post_ids")]
		public List<int> PostIDs { get; set; }

		[JsonProperty("creator_name")]
		public string CreatorName { get; set; }

		[JsonProperty("post_count")]
		public int PostCount { get; set; }

	}
}
