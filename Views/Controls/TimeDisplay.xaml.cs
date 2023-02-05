using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace WorkHoursRecorder.Views.Controls;
public sealed partial class TimeDisplay : UserControl {
	public TimeDisplay() {
		this.InitializeComponent();
		Load();
	}

	private void Load() {
		RootGrid.ColumnDefinitions.Clear();
		RootGrid.Children.Clear();

		const int HOUR = 24;
		const int INTERVAL = 3;

		for (int h = 0; h < (HOUR + 1) + HOUR * INTERVAL; h++) {
			RootGrid.ColumnDefinitions.Add(new ColumnDefinition() {
				Width = new GridLength(1, GridUnitType.Star),
			});
		}
		for (int h = 0; h <= HOUR; h++) {
			Canvas canvas = new();
			canvas.Children.Add(new TextBlock() {
				Text = $"{h}",
				FontSize = 15,
			});
			RootGrid.Children.Add(canvas);
			Grid.SetColumn(canvas, h * (INTERVAL + 1));
		}
		for (int h = 0; h < (HOUR + 1) + HOUR * INTERVAL; h++) {
			if (h % (INTERVAL + 1) != 0) {
				Canvas canvas = new() {
					Background = new SolidColorBrush(Colors.Red),
					HorizontalAlignment = HorizontalAlignment.Stretch,
					VerticalAlignment = VerticalAlignment.Stretch,
				};
				canvas.Children.Add(new Border() {
					Background = new SolidColorBrush(Colors.Gray),
					Width = 2,
					Height = 5,
					HorizontalAlignment = HorizontalAlignment.Center,
				});
				RootGrid.Children.Add(canvas);
				Grid.SetColumn(canvas, h);
			}
		}

	}
}
