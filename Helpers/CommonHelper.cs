using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkHoursRecorder.Models;

namespace WorkHoursRecorder.Helpers;
public static class CommonHelper {
	public static double GetHourSpan(this List<WorkTimeSpan>? list) {
		if (list == null || list.Count == 0) {
			return 0;
		} else {
			double result = 0;
			foreach (WorkTimeSpan item in list) {
				result += item.GetSpan();
			}
			return result;
		}
	}

	public static string ToDuo(this int num) {
		if (0 <= num && num < 10) {
			return $"0{num}";
		} else {
			return $"{num}";
		}
	}
}
