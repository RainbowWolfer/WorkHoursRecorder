using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using WorkHoursRecorder.Helpers;
using WorkHoursRecorder.Models;

namespace WorkHoursRecorder.Views.Controls;
public sealed partial class DayButton : UserControl {
	private DateTime current;
	private List<WorkTimeSpan>? workTimeSpans;
	private bool isSelected;

	public DateTime Current {
		get => current;
		private set {
			current = value;
			DayText.Text = $"{value.Day}";
			UpdateInfo();
		}
	}

	public List<WorkTimeSpan>? WorkTimeSpans {
		get => workTimeSpans;
		private set {
			workTimeSpans = value;
			if (WorkTimeSpans == null || WorkTimeSpans.Count == 0) {
				HourSpanText.Visibility = Visibility.Collapsed;
			} else {
				HourSpanText.Visibility = Visibility.Visible;
				HourSpanText.Text = $"{value.GetHourSpan()}H";
			}
		}
	}

	public bool IsSelected {
		get => isSelected;
		set {
			isSelected = value;
			ButtonBorderAnimation.To = value ? 1 : 0;
			ButtonBorderStoryboard.Begin();
		}
	}

	public event Action<DayButton>? OnClick;

	public DayButton(DateTime current) {
		this.InitializeComponent();
		Current = current;

		if (current == DateTime.Today) {
			DayText.FontWeight = FontWeights.Bold;
			DayText.FontSize = 32;
		}
	}

	private void MainButton_Click(object sender, RoutedEventArgs e) {
		OnClick?.Invoke(this);
	}

	public void UpdateInfo() {
		WorkTimeSpans = Local.Info?.GetDateByDate(Current);
	}
}
