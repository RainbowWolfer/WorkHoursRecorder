using Microsoft.UI.Xaml;

namespace WorkHoursRecorder;
public partial class App : Application {
	public MainWindow? MainWindow { get; private set; }
	public App() {
		this.InitializeComponent();
	}

	protected override void OnLaunched(LaunchActivatedEventArgs args) {
		MainWindow = new MainWindow();
		MainWindow.Activate();
		MainWindow.SetIcon(RequestedTheme);
	}

}
