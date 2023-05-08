using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Yiff_Browser_WinUI3.Views.Controls.CustomControls {
	public class LockableToggleButton : ToggleButton {
		protected override void OnToggle() {
			if (!LockToggle) {
				base.OnToggle();
			}
		}

		public bool LockToggle {
			get => (bool)GetValue(LockToggleProperty);
			set => SetValue(LockToggleProperty, value);
		}

		public static readonly DependencyProperty LockToggleProperty = DependencyProperty.Register(
			nameof(LockToggle),
			typeof(bool),
			typeof(LockableToggleButton),
			new PropertyMetadata(false)
		);
	}
}
