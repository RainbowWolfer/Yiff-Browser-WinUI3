using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Yiff_Browser_WinUI3.Models.E621;
using System.Threading.Tasks;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Views.Controls.PictureViews {
	public sealed partial class PoolInfoView : UserControl {



		public E621Pool E621Pool {
			get => (E621Pool)GetValue(E621PoolProperty);
			set => SetValue(E621PoolProperty, value);
		}

		public static readonly DependencyProperty E621PoolProperty = DependencyProperty.Register(
			nameof(E621Pool),
			typeof(E621Pool),
			typeof(PoolInfoView),
			new PropertyMetadata(null, OnE621PoolChanged)
		);

		private static void OnE621PoolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not PoolInfoView view) {
				return;
			}


		}

		public PoolInfoView() {
			this.InitializeComponent();
		}

		public static async Task ShowAsDialog(E621Pool pool, XamlRoot xamlRoot) {
			await new PoolInfoView() {
				E621Pool = pool,
			}.CreateContentDialog(xamlRoot, new ContentDialogParameters() {
				CloseText = "Back",
				Title = $"Pool Info",
			}).ShowDialogAsync();
		}
	}
}
