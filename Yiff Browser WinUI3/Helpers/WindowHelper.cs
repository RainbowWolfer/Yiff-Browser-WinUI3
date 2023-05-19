using Microsoft.UI.Xaml;
using System;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Yiff_Browser_WinUI3.Helpers {
	public static class WindowHelper {
		public static FolderPicker CreateFolderPicker(params string[] filters) {
			Window window = new();
			IntPtr hwnd = WindowNative.GetWindowHandle(window);
			FolderPicker pick = new();
			foreach (string f in filters) {
				pick.FileTypeFilter.Add(f);
			}
			InitializeWithWindow.Initialize(pick, hwnd);
			return pick;
		}

	}
}
