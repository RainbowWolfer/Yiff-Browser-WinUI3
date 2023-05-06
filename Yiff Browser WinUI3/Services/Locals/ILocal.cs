using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yiff_Browser_WinUI3.Services.Locals {
	public interface ILocal {
		Task Read();
		void Write();
	}
}
