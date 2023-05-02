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
using System.Collections.ObjectModel;
using Yiff_Browser_WinUI3.Models.E621;
using System.Collections.Specialized;
using System.Diagnostics;
using Prism.Mvvm;
using Yiff_Browser_WinUI3.Services.Networks;

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class PostsViewer : UserControl {
		public int ItemWidth { get; } = 380;
		public int ItemHeight { get; } = 50;


		public E621PostParameters Parameters {
			get => (E621PostParameters)GetValue(MyPropertyProperty);
			set => SetValue(MyPropertyProperty, value);
		}

		public static readonly DependencyProperty MyPropertyProperty = DependencyProperty.Register(
			nameof(Parameters),
			typeof(E621PostParameters),
			typeof(PostsViewer),
			new PropertyMetadata(null, OnParametersChanged)
		);

		private static void OnParametersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not PostsViewer view) {
				return;
			}

		}

		public ObservableCollection<E621Post> Posts {
			get => (ObservableCollection<E621Post>)GetValue(PostsProperty);
			set => SetValue(PostsProperty, value);
		}

		public static readonly DependencyProperty PostsProperty = DependencyProperty.Register(
			nameof(Posts),
			typeof(ObservableCollection<E621Post>),
			typeof(PostsViewer),
			new PropertyMetadata(new ObservableCollection<E621Post>(), OnChanged)
		);

		private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is not PostsViewer view) {
				return;
			}
			if (e.OldValue is ObservableCollection<E621Post> oldList) {
				oldList.CollectionChanged -= view.Posts_CollectionChanged;
			}
			if (e.NewValue is ObservableCollection<E621Post> newList) {
				newList.CollectionChanged += view.Posts_CollectionChanged;
				foreach (E621Post item in newList) {
					view.AddPost(item);
				}
			}
		}

		public PostsViewer() {
			this.InitializeComponent();
		}


		private void Posts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.Action == NotifyCollectionChangedAction.Reset) {
				MainGrid.Children.Clear();
			} else if (e.Action == NotifyCollectionChangedAction.Add) {
				foreach (E621Post post in e.NewItems.Cast<E621Post>()) {
					AddPost(post);
				}
			} else if (e.Action == NotifyCollectionChangedAction.Remove) {
				List<ImageViewItem> views = new();
				foreach (E621Post post in e.OldItems.Cast<E621Post>()) {
					foreach (ImageViewItem view in MainGrid.Children.Cast<ImageViewItem>()) {
						if (view.Post == post) {
							views.Add(view);
						}
					}
				}
				foreach (ImageViewItem view in views) {
					MainGrid.Children.Remove(view);
				}
			} else {
				throw new NotSupportedException();
			}
		}

		private void AddPost(E621Post post) {
			ImageViewItem view = new() {
				Margin = new Thickness(5),
				Post = post,
			};
			view.OnPostDeleted += () => {
				Posts.Remove(post);
			};

			double ratio = post.file.width / (double)post.file.height;
			double h = (ItemWidth / ratio) / ItemHeight;
			int h2 = (int)Math.Ceiling(h);

			VariableSizedWrapGrid.SetRowSpan(view, h2);
			VariableSizedWrapGrid.SetColumnSpan(view, 1);

			MainGrid.Children.Add(view);
		}

		private void PageForwardButton_Click(object sender, RoutedEventArgs e) {
			PageFlyout.Hide();
			ViewModel.Forward();
		}
	}

	public class PostsViewerViewModel : BindableBase {
		private int pageValue;
		private bool isLoading;

		public int PageValue {
			get => pageValue;
			set => SetProperty(ref pageValue, value);
		}

		public bool IsLoading {
			get => isLoading;
			set => SetProperty(ref isLoading, value);
		}

		public void Forward() {

		}
	}
}
