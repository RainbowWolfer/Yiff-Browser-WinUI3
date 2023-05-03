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

namespace Yiff_Browser_WinUI3.Views.Controls {
	public sealed partial class ListingsManager : UserControl {
		public ListingsManager() {
			this.InitializeComponent();
		}

		private void DeleteConfirmTeachingTip_ActionButtonClick(TeachingTip sender, object args) {

		}
	}

	public class ListingsManagerViewModel : BindableBase {
		private bool isDelelteListTipOpen;

		public List<string> Strings { get; }

		public ListingsManagerViewModel() {
			Strings = new List<string>() { "1", "2", "3", "4", "5", "5", "5", "5", "5" };
		}

		public bool IsDelelteListTipOpen {
			get => isDelelteListTipOpen;
			set => SetProperty(ref isDelelteListTipOpen, value);
		}

		public ICommand DeleteListCommand => new DelegateCommand(DeleteList);

		public ICommand TestCommand => new DelegateCommand(Test);

		private void DeleteList() {
			IsDelelteListTipOpen = true;
		}

		private void Test() {

		}
	}
}
