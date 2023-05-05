using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class TextSubmitInput : UserControl {
		public string Text {
			get => (string)GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			nameof(Text),
			typeof(string),
			typeof(TextSubmitInput),
			new PropertyMetadata(string.Empty, OnTextChanged)
		);

		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is TextSubmitInput input) {
				input.Update();
			}
		}

		private string HintText {
			get => (string)GetValue(HintTextProperty);
			set => SetValue(HintTextProperty, value);
		}

		private static readonly DependencyProperty HintTextProperty = DependencyProperty.Register(
			nameof(HintText),
			typeof(string),
			typeof(TextSubmitInput),
			new PropertyMetadata(string.Empty)
		);



		public string[] Exists {
			get => (string[])GetValue(ExistsProperty);
			set => SetValue(ExistsProperty, value);
		}

		public static readonly DependencyProperty ExistsProperty = DependencyProperty.Register(
			nameof(Exists),
			typeof(string[]),
			typeof(TextSubmitInput),
			new PropertyMetadata(Array.Empty<string>())
		);

		private void Update() {
			if (Text.IsBlank()) {
				HintText = "Cannot be empty";
			} else if (Exists?.Contains(Text) ?? false) {
				HintText = "Already exists";
			} else {
				HintText = string.Empty;
			}

			TextBoxBorderBrush = HintText.IsBlank() ? App.TextBoxDefaultBorderBrush : new SolidColorBrush(Colors.Red);
		}


		public Brush TextBoxBorderBrush {
			get => (Brush)GetValue(TextBoxBorderBrushProperty);
			set => SetValue(TextBoxBorderBrushProperty, value);
		}

		public static readonly DependencyProperty TextBoxBorderBrushProperty = DependencyProperty.Register(
			nameof(TextBoxBorderBrush),
			typeof(Brush),
			typeof(TextSubmitInput),
			new PropertyMetadata(App.TextBoxDefaultBorderBrush)
		);



		public ICommand SubmitCommand {
			get => (ICommand)GetValue(SubmitCommandProperty);
			set => SetValue(SubmitCommandProperty, value);
		}

		public static readonly DependencyProperty SubmitCommandProperty = DependencyProperty.Register(
			nameof(SubmitCommand),
			typeof(ICommand),
			typeof(TextSubmitInput),
			new PropertyMetadata(null)
		);




		public TextSubmitInput() {
			this.InitializeComponent();
		}

		private void TextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e) {
			if (e.Key == VirtualKey.Enter && HintText.IsBlank()) {
				SubmitCommand.Execute(Text);
			}
		}
	}
}
