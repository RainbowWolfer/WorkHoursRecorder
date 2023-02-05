using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkHoursRecorder.Helpers;
using WorkHoursRecorder.Models;

namespace WorkHoursRecorder.Views.Controls;
public sealed partial class AddTimeSpanView : UserControl {
	private readonly Brush startTextBrush;
	private readonly bool initialized = false;

	private readonly List<WorkTimeSpan> exist;
	private Time startTime;
	private Time endTime;

	public event Action<bool>? OnError;

	public Time StartTime {
		get => startTime;
		set {
			startTime = value;

			int hour = value.Hour;
			int minute = value.Minute;

			skipListening = true;

			StartTimeSlider.Value = TimeValueConverter.TimeToNumber(hour, minute);
			StartTimePicker.SelectedTime = new TimeSpan(hour, minute, 0);

			skipListening = false;

			UpdateText();
		}
	}
	public Time EndTime {
		get => endTime;
		set {
			endTime = value;

			int hour = value.Hour;
			int minute = value.Minute;

			skipListening = true;

			EndTimeSlider.Value = EndTimeSlider.Maximum - TimeValueConverter.TimeToNumber(hour, minute);
			EndTimePicker.SelectedTime = new TimeSpan(hour, minute, 0);

			skipListening = false;

			UpdateText();
		}
	}

	private bool skipListening { get; set; } = false;

	public AddTimeSpanView(List<WorkTimeSpan> exist) {
		this.InitializeComponent();
		initialized = true;

		this.exist = exist;

		startTextBrush = TimeSpanText.Foreground;
		UpdateText();
	}

	public void SetTimes() {
		StartTime = new Time(09, 00);
		EndTime = new Time(17, 00);
	}
	public void SetTimes(Time start, Time end) {
		StartTime = start;
		EndTime = end;
	}

	public (Time start, Time end) GetResult() {
		return (new Time(StartTime.Hour, StartTime.Minute), new Time(EndTime.Hour, EndTime.Minute));
	}

	private void StartTimeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
		if (!initialized || skipListening) {
			return;
		}

		(int hour, int minute) = TimeValueConverter.NumberToTime(StartTimeSlider.Value);
		StartTime = new Time(hour, minute);
	}

	private void EndTimeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e) {
		if (!initialized || skipListening) {
			return;
		}
		(int hour, int minute) = TimeValueConverter.NumberToTime(EndTimeSlider.Maximum - EndTimeSlider.Value);
		EndTime = new Time(hour, minute);
	}

	private void StartTimePicker_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args) {
		if (!initialized || skipListening) {
			return;
		}

		if (StartTimePicker.SelectedTime != null) {
			int hour = StartTimePicker.SelectedTime.Value.Hours;
			int minute = StartTimePicker.SelectedTime.Value.Minutes;
			StartTime = new Time(hour, minute);
		}
	}

	private void EndTimePicker_SelectedTimeChanged(TimePicker sender, TimePickerSelectedValueChangedEventArgs args) {
		if (!initialized || skipListening) {
			return;
		}

		if (EndTimePicker.SelectedTime != null) {
			int hour = EndTimePicker.SelectedTime.Value.Hours;
			int minute = EndTimePicker.SelectedTime.Value.Minutes;
			EndTime = new Time(hour, minute);
		}
	}

	private void UpdateText() {
		if (!initialized) {
			return;
		}

		TimeSpan? start = StartTimePicker.SelectedTime;
		TimeSpan? end = EndTimePicker.SelectedTime;
		if (start == null || end == null) {
			return;
		}
		TimeSpanText.Text = $"{start.Value.Hours.ToDuo()}:{start.Value.Minutes.ToDuo()} - {end.Value.Hours.ToDuo()}:{end.Value.Minutes.ToDuo()}";
		WholeTimeText.Text = $"{Math.Round(end.Value.Subtract(start.Value).TotalHours, 1)}H";

		string errorMessage;
		if (start > end) {
			errorMessage = "End time cannot be earlier than start time";
		} else if (exist.Any(x => new WorkTimeSpan(startTime, endTime).CheckConflict(x))) {
			errorMessage = "Time conflict exists";
		} else {
			errorMessage = string.Empty;
		}

		ErrorText.Text = errorMessage;
		if (string.IsNullOrWhiteSpace(errorMessage)) {//none error
			TimeSpanText.Foreground = startTextBrush;
			WholeTimeText.Foreground = startTextBrush;
			OnError?.Invoke(false);
			ErrorText.Visibility = Visibility.Collapsed;
		} else {
			TimeSpanText.Foreground = new SolidColorBrush(Colors.Red);
			WholeTimeText.Foreground = new SolidColorBrush(Colors.Red);
			OnError?.Invoke(true);
			ErrorText.Visibility = Visibility.Visible;
		}
	}

}

public class TimeValueConverter : IValueConverter {
	public static (int hour, int minute) NumberToTime(double minuteValue) {
		double h = minuteValue / 60;
		double t = Math.Truncate(h);
		double m = h - t;
		double minute = m * 60;
		double round = Math.Round(minute);
		return ((int)t, (int)round);
	}

	public static double TimeToNumber(int hour, int minute) {
		return hour * 60 + minute;
	}

	public bool Reversed { get; set; }

	public object Convert(object value, Type targetType, object parameter, string language) {
		if (value is double d) {
			if (Reversed) {
				d = 60 * 24 - d;
			}
			(int hour, int minute) = NumberToTime(d);
			return $"{hour.ToDuo()}:{minute.ToDuo()}";
		}
		return value;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language) {
		return value;
	}
}
