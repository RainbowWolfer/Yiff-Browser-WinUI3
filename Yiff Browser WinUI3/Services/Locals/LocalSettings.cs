using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yiff_Browser_WinUI3.Services.Locals {
	public class LocalSettings {
		public bool E621Yiff { get; set; } = true;
		public int E621PageLimitCount { get; set; } = 75;


		public string Username { get; set; } = "";
		public string UserAPI { get; set; } = "";

	}
}
