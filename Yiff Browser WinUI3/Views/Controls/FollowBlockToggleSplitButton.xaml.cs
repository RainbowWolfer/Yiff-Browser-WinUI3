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

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class FollowBlockToggleSplitButton : UserControl {


		//true => follow, false => block
		public bool FollowOrBlock {
			get => (bool)GetValue(FollowOrBlockProperty);
			set => SetValue(FollowOrBlockProperty, value);
		}

		public static readonly DependencyProperty FollowOrBlockProperty = DependencyProperty.Register(
			nameof(FollowOrBlock),
			typeof(bool),
			typeof(FollowBlockToggleSplitButton),
			new PropertyMetadata(true)
		);



		public FollowBlockToggleSplitButton() {
			this.InitializeComponent();
		}
	}
}
