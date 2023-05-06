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
using Prism.Mvvm;
using Windows.Media.Core;
using System.Diagnostics;
using Yiff_Browser_WinUI3.Helpers;
using Windows.Media.Playback;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class MediaDisplayView : UserControl {

		public string URL {
			get => (string)GetValue(URLProperty);
			set => SetValue(URLProperty, value);
		}

		public static readonly DependencyProperty URLProperty = DependencyProperty.Register(
			nameof(URL),
			typeof(string),
			typeof(MediaDisplayView),
			new PropertyMetadata(string.Empty, OnURLChanged)
		);

		private static void OnURLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is MediaDisplayView view) {
				view.ViewModel.Initiaize((string)e.NewValue);
			}
		}

		public MediaDisplayView() {
			this.InitializeComponent();
			MediaPlayer.MediaPlayer.IsLoopingEnabled = true;
		}

		public void Play() {
			MediaPlayer.MediaPlayer.Play();
		}

		public void Pause() {
			MediaPlayer.MediaPlayer.Pause();
		}

		private void Space_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
			if (MediaPlayer.MediaPlayer.CurrentState == MediaPlayerState.Paused) {
				MediaPlayer.MediaPlayer.Play();
			} else if (MediaPlayer.MediaPlayer.CurrentState == MediaPlayerState.Playing) {
				MediaPlayer.MediaPlayer.Pause();
			} else {

			}
		}

		private void Left_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
			Step(-1);
		}

		private void Right_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
			Step(1);
		}

		private void Step(double seconds) {
			TimeSpan currentPosition = MediaPlayer.MediaPlayer.Position;
			TimeSpan newPosition = currentPosition.Add(TimeSpan.FromSeconds(seconds));
			MediaPlayer.MediaPlayer.Position = newPosition;
		}

		private void CtrlLeft_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
			MediaPlayer.MediaPlayer.StepBackwardOneFrame();
		}

		private void CtrlRight_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args) {
			MediaPlayer.MediaPlayer.StepForwardOneFrame();
			//MediaControl.Show();
		}
	}

	public class MediaDisplayViewModel : BindableBase {
		private MediaSource mediaSource;

		public MediaSource MediaSource {
			get => mediaSource;
			set => SetProperty(ref mediaSource, value);
		}

		public void Initiaize(string url) {
			if (url.IsBlank()) {
				MediaSource = null;
			} else {
				MediaSource = MediaSource.CreateFromUri(new Uri(url));

				MediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
				MediaSource.StateChanged += MediaSource_StateChanged;

			}
		}

		private void MediaSource_StateChanged(MediaSource sender, MediaSourceStateChangedEventArgs args) {
			Debug.WriteLine(args.NewState);
		}

		private void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args) {
			Debug.WriteLine(args.Error);
		}
	}
}
