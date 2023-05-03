using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Windows.System;
using Yiff_Browser_WinUI3.Helpers;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class AddButtonInput : UserControl {
		public string PlaceholderText {
			get => (string)GetValue(PlaceholderTextProperty);
			set => SetValue(PlaceholderTextProperty, value);
		}

		public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
			nameof(PlaceholderText),
			typeof(string),
			typeof(AddButtonInput),
			new PropertyMetadata(string.Empty, OnPlaceHolderTextChanged)
		);

		private static void OnPlaceHolderTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is AddButtonInput view) {
				view.ViewModel.PlaceholderText = (string)e.NewValue;
			}
		}

		public string[] Exists {
			get => (string[])GetValue(ExistsProperty);
			set => SetValue(ExistsProperty, value);
		}

		public static readonly DependencyProperty ExistsProperty = DependencyProperty.Register(
			nameof(Exists),
			typeof(string[]),
			typeof(AddButtonInput),
			new PropertyMetadata(Array.Empty<string>(), OnExistsChanged)
		);

		private static void OnExistsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is AddButtonInput view) {
				view.ViewModel.Exists = (string[])e.NewValue;
			}
		}

		public AddButtonInput() {
			this.InitializeComponent();
		}
	}


	public class AddButtonInputViewModel : BindableBase {
		private string placeholderText = string.Empty;
		private string text = string.Empty;
		private Brush textBoxBrush = App.TextBoxDefaultBorderBrush;
		private bool isErrorTipOpen;
		private string errorTip;

		public string PlaceholderText {
			get => placeholderText;
			set => SetProperty(ref placeholderText, value);
		}

		public string Text {
			get => text;
			set => SetProperty(ref text, value, OnTextChanged);
		}

		public Brush TextBoxBrush {
			get => textBoxBrush;
			set => SetProperty(ref textBoxBrush, value);
		}

		public bool IsErrorTipOpen {
			get => isErrorTipOpen;
			set => SetProperty(ref isErrorTipOpen, value, O);
		}

		private void O() {
			Debug.WriteLine(IsErrorTipOpen);
		}

		public string ErrorTip {
			get => errorTip;
			set => SetProperty(ref errorTip, value);
		}

		public string[] Exists { get; set; }

		private void OnTextChanged() {
			IsErrorTipOpen = false;
			if (Text.IsBlank() || (Exists?.Contains(Text) ?? false)) {
				TextBoxBrush = new SolidColorBrush(Colors.Red);
			} else {
				TextBoxBrush = App.TextBoxDefaultBorderBrush;
			}
		}

		public ICommand SubmitCommand => new DelegateCommand(Sumbit);
		public ICommand TextBoxKeyDownCommand => new DelegateCommand<KeyRoutedEventArgs>(TextBoxKeyDown);

		private void TextBoxKeyDown(KeyRoutedEventArgs args) {
			if (args.Key == VirtualKey.Enter) {
				Sumbit();
			}
		}

		private void Sumbit() {
			if (text.IsBlank()) {
				ErrorTip = "Cannot be empty";
				IsErrorTipOpen = true;
				return;
			} else if (Exists?.Contains(Text) ?? false) {
				ErrorTip = "Already existed";
				IsErrorTipOpen = true;
				return;
			} else {
				ErrorTip = "";
				IsErrorTipOpen = false;
			}

			Text = string.Empty;
			TextBoxBrush = App.TextBoxDefaultBorderBrush;
		}
	}
}
