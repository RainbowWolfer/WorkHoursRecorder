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
using WorkHoursRecorder.Models;

namespace WorkHoursRecorder.Views.Controls;
public sealed partial class TimeSpanItemView : UserControl {
	private WorkTimeSpan span;

	public WorkTimeSpan Span {
		get => span;
		set {
			span = value;
			TimeSpanText.Text = $"{span.Start} - {span.End}";
			WholeTimeText.Text = $"{span.GetSpan()}H";
		}
	}

	public event Action<WorkTimeSpan>? OnEdit;
	public event Action<WorkTimeSpan>? OnDelete;
	public TimeSpanItemView() {
		this.InitializeComponent();
	}

	private void Item_Delete(object sender, RoutedEventArgs e) {
		OnDelete?.Invoke(Span);
	}

	private void Item_Edit(object sender, RoutedEventArgs e) {
		OnEdit?.Invoke(Span);
	}
}
