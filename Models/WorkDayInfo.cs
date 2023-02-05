using System;
using System.Collections.Generic;
using System.Linq;
using WorkHoursRecorder.Helpers;

namespace WorkHoursRecorder.Models;
public class WorkDayInfo {
	public List<Pair<Date, List<WorkTimeSpan>>> Times { get; set; } = new();

	public WorkDayInfo() {

	}

	public void Add(Date day, Time start, Time end) {
		WorkTimeSpan span = new(start, end);
		Pair<Date, List<WorkTimeSpan>>? pair = Times.Find(x => x.Key == day);
		if (pair != null) {//found
			pair.Value ??= new List<WorkTimeSpan>();
			int index = 0;
			for (int i = 0; i < pair.Value.Count; i++) {
				if (span.Start > pair.Value[i].Start) {
					index = i;
				}
			}
			pair.Value.Insert(index, span);
		} else {
			Times.Add(new Pair<Date, List<WorkTimeSpan>>(day, new List<WorkTimeSpan>() { span }));
		}
	}

	public void Remove(Date day, WorkTimeSpan span) {
		Pair<Date, List<WorkTimeSpan>>? pair = Times.Find(x => x.Key == day);
		if (pair != null && pair.Value != null) {//found
			pair.Value.Remove(span);
		}
	}

	public void Edit(Date day, WorkTimeSpan old, WorkTimeSpan @new) {
		Pair<Date, List<WorkTimeSpan>>? pair = Times.Find(x => x.Key == day);
		if (pair != null && pair.Value != null) {//found
			int foundIndex = pair.Value.FindIndex(x => x == old);
			if (foundIndex != -1) {
				pair.Value[foundIndex] = @new;
			}
		}
	}

	public List<WorkTimeSpan>? GetDateByDate(int year, int month, int day) {
		Date date = new(year, month, day);
		Pair<Date, List<WorkTimeSpan>>? pair = Times.Find(x => x.Key == date);
		if (pair != null) {//found
			return pair.Value?.ToList() ?? new List<WorkTimeSpan>();
		} else {
			return null;
		}
	}

	public List<WorkTimeSpan>? GetDateByDate(DateTime value) {
		return GetDateByDate(value.Year, value.Month, value.Day);
	}

	public List<Pair<Date, List<WorkTimeSpan>>> GetWorkDaysInMonth(int year, int month) {
		List<Pair<Date, List<WorkTimeSpan>>> filtered = Times.Where(x =>
			x.Key.Year == year
			&& x.Key.Month == month
			&& x.Value != null
			&& x.Value.Count > 0
			&& x.Value.Sum(c => c.GetSpan()) != 0
		).ToList();
		return filtered;
	}
}

public struct WorkTimeSpan : IEquatable<WorkTimeSpan> {
	public Time Start { get; set; }
	public Time End { get; set; }

	public WorkTimeSpan(Time start, Time end) {
		Start = start;
		End = end;
	}

	public double GetSpan(int round = 1) {
		TimeSpan from = new(Start.Hour, Start.Minute, 0);
		TimeSpan to = new(End.Hour, End.Minute, 0);
		TimeSpan span = to.Subtract(from);
		return Math.Round(span.TotalHours, round);
	}

	public bool CheckConflict(WorkTimeSpan span) {
		if (End > span.Start && Start < span.End) {
			return true;
		}
		return false;
	}

	public override bool Equals(object? obj) {
		return obj is WorkTimeSpan span && Equals(span);
	}

	public bool Equals(WorkTimeSpan other) {
		return Start.Equals(other.Start) &&
			   End.Equals(other.End);
	}

	public override int GetHashCode() {
		return HashCode.Combine(Start, End);
	}

	public override string? ToString() {
		return $"{Start} - {End}";
	}

	public static bool operator ==(WorkTimeSpan left, WorkTimeSpan right) {
		return left.Equals(right);
	}

	public static bool operator !=(WorkTimeSpan left, WorkTimeSpan right) {
		return !(left == right);
	}
}

public struct Time : IEquatable<Time> {
	public int Hour { get; set; }
	public int Minute { get; set; }
	public Time(int hour, int minute) {
		Hour = hour;
		Minute = minute;
	}

	public override string? ToString() {
		return $"{Hour.ToDuo()}:{Minute.ToDuo()}";
	}

	public override bool Equals(object? obj) {
		return obj is Time time && Equals(time);
	}

	public bool Equals(Time other) {
		return Hour == other.Hour &&
			   Minute == other.Minute;
	}

	public override int GetHashCode() {
		return HashCode.Combine(Hour, Minute);
	}

	public static bool operator ==(Time left, Time right) {
		return left.Equals(right);
	}

	public static bool operator !=(Time left, Time right) {
		return !(left == right);
	}

	public static bool operator >(Time left, Time right) {
		TimeSpan l = new(left.Hour, left.Minute, 0);
		TimeSpan r = new(right.Hour, right.Minute, 0);
		return l > r;
	}

	public static bool operator <(Time left, Time right) {
		TimeSpan l = new(left.Hour, left.Minute, 0);
		TimeSpan r = new(right.Hour, right.Minute, 0);
		return l < r;
	}
}

public struct Date : IEquatable<Date> {
	public int Year { get; set; }
	public int Month { get; set; }
	public int Day { get; set; }

	public Date(int year, int month, int day = 0) {
		Year = year;
		Month = month;
		Day = day;
	}

	public override bool Equals(object? obj) {
		return obj is Date date && Equals(date);
	}

	public bool Equals(Date other) {
		return Year == other.Year &&
			   Month == other.Month &&
			   Day == other.Day;
	}

	public override int GetHashCode() {
		return HashCode.Combine(Year, Month, Day);
	}

	public static bool operator ==(Date left, Date right) {
		return left.Equals(right);
	}

	public static bool operator !=(Date left, Date right) {
		return !(left == right);
	}
}
