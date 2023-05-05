using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Yiff_Browser_WinUI3.Views.Controls.LoadingViews {
	public sealed partial class LoadingRingWithTextBelow : UserControl {

		public bool IsIndeterminate {
			get => (bool)GetValue(IsIndeterminateProperty);
			set => SetValue(IsIndeterminateProperty, value);
		}

		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.Register(
			nameof(IsIndeterminate),
			typeof(bool),
			typeof(LoadingRingWithTextBelow),
			new PropertyMetadata(true)
		);



		public int Progress {
			get => (int)GetValue(ProgressProperty);
			set => SetValue(ProgressProperty, value);
		}

		public static readonly DependencyProperty ProgressProperty = DependencyProperty.Register(
			nameof(Progress),
			typeof(int),
			typeof(LoadingRingWithTextBelow),
			new PropertyMetadata(0, OnProgressChanged)
		);

		private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is LoadingRingWithTextBelow view) {
				view.IsIndeterminate = false;
			}
		}



		public string Text {
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			nameof(Text),
			typeof(string),
			typeof(LoadingRingWithTextBelow),
			new PropertyMetadata(string.Empty)
		);



		public LoadingRingWithTextBelow() {
			this.InitializeComponent();
		}

		public LoadingRingWithTextBelow(string text) : this() {
			Text = text;
		}
	}
}
