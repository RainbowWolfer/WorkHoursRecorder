using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkHoursRecorder.Models;

namespace WorkHoursRecorder.Views.Controls {
	public sealed partial class MonthSummaryView : UserControl {
		public List<Pair<Date, List<WorkTimeSpan>>> Days { get; }
		public MonthSummaryView(List<Pair<Date, List<WorkTimeSpan>>> days) {
			this.InitializeComponent();
			Days = days;

			int daysCount = Days.Count;
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
