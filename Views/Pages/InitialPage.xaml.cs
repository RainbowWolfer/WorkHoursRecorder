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
using Windows.Foundation;
using Windows.Foundation.Collections;
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
