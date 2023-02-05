using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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
