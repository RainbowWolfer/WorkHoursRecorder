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

namespace WorkHoursRecorder.Views.Controls;
public sealed partial class ImportErrorView : UserControl {
	public ImportErrorView(Exception? exception) {
		this.InitializeComponent();
		DetailButton.Visibility = exception == null ? Visibility.Collapsed : Visibility.Visible;
		DetailText.Text = $"{exception}";
	}
}
