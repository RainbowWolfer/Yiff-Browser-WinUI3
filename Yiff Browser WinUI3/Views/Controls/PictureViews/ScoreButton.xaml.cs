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
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Yiff_Browser_WinUI3.Views.Controls.PictureViews {
	public sealed partial class ScoreButton : UserControl {


		public bool UpOrDown {
			get => (bool)GetValue(UpOrDownProperty);
			set => SetValue(UpOrDownProperty, value);
		}

		public static readonly DependencyProperty UpOrDownProperty = DependencyProperty.Register(
			nameof(UpOrDown),
			typeof(bool),
			typeof(ScoreButton),
			new PropertyMetadata(false)
		);

		public bool IsLoading {
			get => (bool)GetValue(IsLoadingProperty);
			set => SetValue(IsLoadingProperty, value);
		}

		public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
			nameof(IsLoading),
			typeof(bool),
			typeof(ScoreButton),
			new PropertyMetadata(false)
		);

		public bool IsChecked {
			get => (bool)GetValue(IsCheckedProperty);
			set => SetValue(IsCheckedProperty, value);
		}

		public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register(
			nameof(IsChecked),
			typeof(bool),
			typeof(ScoreButton),
			new PropertyMetadata(false)
		);





		public ICommand Command {
			get => (ICommand)GetValue(CommandProperty);
			set => SetValue(CommandProperty, value);
		}

		public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
			nameof(Command),
			typeof(ICommand),
			typeof(ScoreButton),
			new PropertyMetadata(null)
		);







		public ScoreButton() {
			this.InitializeComponent();
		}
	}
}
