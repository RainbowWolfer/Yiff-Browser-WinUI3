using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yiff_Browser_WinUI3.Helpers {
	public static class ContentDialogHelper {
		public static ContentDialog CreateContentDialog(this object target, XamlRoot xamlRoot, ContentDialogParameters parameters) {
			if (xamlRoot is null) {
				throw new ArgumentNullException(nameof(xamlRoot));
			}

			if (parameters is null) {
				throw new ArgumentNullException(nameof(parameters));
			}

			ContentDialog dialog = new() {
				XamlRoot = xamlRoot,
				Style = App.DialogStyle,
				Content = target,
				Title = parameters.Title,
				PrimaryButtonText = parameters.PrimaryText,
				SecondaryButtonText = parameters.SecondaryText,
				CloseButtonText = parameters.CloseText,
			};

			if (!parameters.SkipWidthSet) {
				dialog.Resources["ContentDialogMaxWidth"] = parameters.MaxWidth;
			}

			return dialog;
		}

		public static async Task<ContentDialogResult> ShowDialogAsync(this ContentDialog dialog) {
			return await dialog.ShowDialogAsync(ContentDialogResult.None);
		}

		public static async Task<ContentDialogResult> ShowDialogAsync(this ContentDialog dialog, ContentDialogResult fallback) {
			try {
				return await dialog.ShowAsync();
			} catch {
				return fallback;
			}
		}

	}


	public class ContentDialogParameters {
		public bool SkipWidthSet { get; set; } = false;
		public double MaxWidth { get; set; } = 1050;

		public string Title { get; set; } = null;
		public string PrimaryText { get; set; } = null;
		public string SecondaryText { get; set; } = null;
		public string CloseText { get; set; } = null;

		public ContentDialogButton DefaultButton { get; set; } = ContentDialogButton.None;

	}


	public class LoadingDialogControl {
		public ContentDialog Dialog { get; private set; }

		public async Task Start(Func<Task> task) {
			if (task is null) {
				throw new ArgumentNullException(nameof(task));
			}

			ShowDialog();
			await task();
			HideDialog();
		}

		private async void ShowDialog() {
			await Dialog.ShowDialogAsync();
		}

		private void HideDialog() {
			Dialog.Hide();
		}

		public LoadingDialogControl(XamlRoot xamlRoot, object content, ContentDialogParameters parameters = null) {
			Dialog = content.CreateContentDialog(xamlRoot, parameters ?? new ContentDialogParameters() {
				SkipWidthSet = true,
			});
		}
	}
}
