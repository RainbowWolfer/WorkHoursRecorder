using Microsoft.UI.Xaml.Controls;
using WorkHoursRecorder.Helpers;

namespace WorkHoursRecorder.Views.Pages;
public sealed partial class InitialPage : Page {
	public InitialPage() {
		this.InitializeComponent();
		Start();
	}

	private async void Start() {
		await Local.Initialize();

		MainWindow.Instance?.Intialize();
	}
}
