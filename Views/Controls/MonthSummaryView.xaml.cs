using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkHoursRecorder.Helpers;
using WorkHoursRecorder.Models;

namespace WorkHoursRecorder.Views.Controls {
	public sealed partial class MonthSummaryView : UserControl {
		public MonthSummaryView(DateTimeOffset date, List<Pair<Date, List<WorkTimeSpan>>> days) {
			this.InitializeComponent();

			DateText.Text = $"{date.Year}-{date.Month.ToDuo()}";

			int daysCount = days.Count;
			double hoursCount = 0;
			foreach (Pair<Date, List<WorkTimeSpan>> day in days) {
				double hours = day.Value.Sum(x => x.GetSpan());
				hoursCount += hours;
			}

			if (daysCount != 0) {
				hoursCount /= daysCount;
			}

			DaysText.Text = $"{daysCount} Days";
			HoursText.Text = $"{Math.Round(hoursCount, 1)} Hours";
		}

	}
}
