using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkHoursRecorder.Helpers;
using WorkHoursRecorder.Models;
using WorkHoursRecorder.Views.Controls;

namespace WorkHoursRecorder.Views.Pages;
public sealed partial class CalendarPage : Page {
	public Date? SelectedDate { get; private set; } = null;

	public CalendarPage() {
		this.InitializeComponent();
		LoadByToday();
	}

	protected override void OnNavigatedTo(NavigationEventArgs e) {
		base.OnNavigatedTo(e);

		DateTimeOffset? date = MainWindow.GetDatePickerValue();
		if (date != null) {
			LoadByDate(date.Value.Year, date.Value.Month);
		}
	}

	public void LoadByToday() {
		DateTime today = DateTime.Today;
		LoadByDate(today.Year, today.Month);
	}

	public void LoadByDate(int year, int month) {
		if (year < 1 || year > 9999 || month < 1 || month > 12) {
			return;
		}
		List<DateTime> dates = new();
		for (DateTime date = new(year, month, 1);
			date.Month == month;
			date = date.AddDays(1)
		) {
			dates.Add(date);
		}

		if (dates.Count == 0) {
			return;
		}

		int startColumn = dates.First().DayOfWeek switch {
			DayOfWeek.Sunday => 0,
			DayOfWeek.Monday => 1,
			DayOfWeek.Tuesday => 2,
			DayOfWeek.Wednesday => 3,
			DayOfWeek.Thursday => 4,
			DayOfWeek.Friday => 5,
			DayOfWeek.Saturday => 6,
			_ => throw new Exception(),
		};
		int row = 0;
		DatesGrid.Children.Clear();
		foreach (DateTime date in dates) {
			DayButton button = new(date);
			button.OnClick += DayButton_OnClick;
			DatesGrid.Children.Add(button);
			Grid.SetColumn(button, startColumn++);
			Grid.SetRow(button, row);
			if (startColumn > 6) {
				startColumn = 0;
				row++;
			}
			AdditionalRow.Height = new GridLength(row >= 5 ? 1 : 0, GridUnitType.Star);
		}
	}

	private void DayButton_OnClick(DayButton button) {
		SelectedDate = new Date(button.Current.Year, button.Current.Month, button.Current.Day);
		SideSplitView.IsPaneOpen = true;
		button.IsSelected = true;
		LoadTimeSpansListView(button.WorkTimeSpans);
	}

	private void SideSplitView_PaneOpening(SplitView sender, object args) {
		const double SCALE = 0.9;
		ContentTransformScaleXAnimation.To = SCALE;
		ContentTransformScaleYAnimation.To = SCALE;
		ContentTransformStoryboard.Begin();
	}

	private void SideSplitView_PaneClosing(SplitView sender, SplitViewPaneClosingEventArgs args) {
		ContentTransformScaleXAnimation.To = 1;
		ContentTransformScaleYAnimation.To = 1;
		ContentTransformStoryboard.Begin();

		SelectedDate = null;
		DatesGrid.Children.Select(x => (DayButton)x).ToList().ForEach(x => x.IsSelected = false);
	}

	private void SideSplitView_PaneClosed(SplitView sender, object args) {
		WorkSpansListView.Items.Clear();
	}

	private void SideBackButton_Click(object sender, RoutedEventArgs e) {
		SideSplitView.IsPaneOpen = false;
	}

	private async void SideAddButton_Click(object sender, RoutedEventArgs e) {
		if (SelectedDate == null) {
			return;
		}
		SideAddButton.IsEnabled = false;
		AddTimeSpanView view = new(Local.GetTimesByDay(SelectedDate.Value));
		view.SetTimes();
		ContentDialog dialog = new() {
			XamlRoot = this.XamlRoot,
			Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
			Title = "Add your time period",
			PrimaryButtonText = "Confirm",
			CloseButtonText = "Back",
			DefaultButton = ContentDialogButton.Primary,
			Content = view,
		};
		view.OnError += error => dialog.IsPrimaryButtonEnabled = !error;
		if (await dialog.ShowAsync() == ContentDialogResult.Primary) {
			(Time start, Time end) = view.GetResult();
			await Local.AddWorkDayInfo(SelectedDate.Value, start, end);
			ListViewItem newItem = AddToCurrentTimeSpan(new WorkTimeSpan(start, end));
			WorkSpansListView.SelectedItem = newItem;

			UpdateDayButton();
		}
		SideAddButton.IsEnabled = true;
	}

	private void UpdateDayButton() {
		if (SelectedDate == null) {
			return;
		}
		DatesGrid.Children.Select(x => (DayButton)x).FirstOrDefault(x => {
			Date value = SelectedDate.Value;
			DateTime dayCurrent = x.Current;
			return value.Year == dayCurrent.Year && value.Month == dayCurrent.Month && value.Day == dayCurrent.Day;
		})?.UpdateInfo();
	}

	private ListViewItem AddToCurrentTimeSpan(WorkTimeSpan span) {
		ListViewItem item = GenerateTimeSpanListViewItem(span);
		int index = 0;
		List<WorkTimeSpan> items = WorkSpansListView.Items.Select(x => ((TimeSpanItemView)((ListViewItem)x).Content).Span).ToList();
		for (int i = 0; i < items.Count; i++) {
			if (span.Start > items[i].Start) {
				index = i;
			}
		}
		WorkSpansListView.Items.Insert(index, item);
		UpdateEmptyWorkSpanesListHintPanel();
		return item;
	}

	private void LoadTimeSpansListView(List<WorkTimeSpan>? workTimeSpans) {
		WorkSpansListView.Items.Clear();
		foreach (WorkTimeSpan span in workTimeSpans ?? new List<WorkTimeSpan>()) {
			WorkSpansListView.Items.Add(GenerateTimeSpanListViewItem(span));
		}
		WorkSpansListView.SelectedIndex = 0;
		UpdateEmptyWorkSpanesListHintPanel();
	}

	private void UpdateEmptyWorkSpanesListHintPanel() {
		if (WorkSpansListView.Items.Count == 0) {
			EmptyWorkSpanesListHintPanel.Visibility = Visibility.Visible;
		} else {
			EmptyWorkSpanesListHintPanel.Visibility = Visibility.Collapsed;
		}
	}

	private ListViewItem GenerateTimeSpanListViewItem(WorkTimeSpan span) {
		TimeSpanItemView view = new() { Span = span };
		ListViewItem item = new() {
			Content = view,
		};
		view.OnEdit += TimeSpanItemView_OnEdit;
		view.OnDelete += TimeSpanItemView_OnDelete;
		return item;
	}

	private async void TimeSpanItemView_OnDelete(WorkTimeSpan span) {
		if (SelectedDate == null) {
			return;
		}
		ListViewItem? found = WorkSpansListView.Items.Select(x => (ListViewItem)x)
			.FirstOrDefault(x => ((TimeSpanItemView)x.Content).Span == span);
		if (found == null) {
			return;
		}

		found.IsSelected = true;

		ContentDialog dialog = new() {
			XamlRoot = this.XamlRoot,
			Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
			Title = "Delete time period",
			PrimaryButtonText = "Confirm",
			CloseButtonText = "Back",
			DefaultButton = ContentDialogButton.Primary,
			Content = $"Are you sure to delete this time period ({span})",
		};

		if (await dialog.ShowAsync() != ContentDialogResult.Primary) {
			return;
		}

		WorkSpansListView.Items.Remove(found);
		WorkSpansListView.SelectedIndex = 0;

		UpdateEmptyWorkSpanesListHintPanel();
		await Local.RemoveWorkDayInfo(SelectedDate.Value, span);
		UpdateDayButton();
	}

	private async void TimeSpanItemView_OnEdit(WorkTimeSpan span) {
		if (SelectedDate == null) {
			return;
		}
		ListViewItem? found = WorkSpansListView.Items.Select(x => (ListViewItem)x)
			.FirstOrDefault(x => ((TimeSpanItemView)x.Content).Span == span);
		if (found != null) {
			found.IsSelected = true;
		}
		List<WorkTimeSpan> exists = Local.GetTimesByDay(SelectedDate.Value);
		exists.Remove(span);
		AddTimeSpanView view = new(exists);
		view.SetTimes(span.Start, span.End);
		ContentDialog dialog = new() {
			XamlRoot = this.XamlRoot,
			Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
			Title = "Edit time period",
			PrimaryButtonText = "Confirm",
			CloseButtonText = "Back",
			DefaultButton = ContentDialogButton.Primary,
			Content = view,
		};
		view.OnError += error => dialog.IsPrimaryButtonEnabled = !error;
		if (await dialog.ShowAsync() == ContentDialogResult.Primary) {
			(Time start, Time end) = view.GetResult();
			WorkTimeSpan @new = new(start, end);
			await Local.EditWorkDayInfo(SelectedDate.Value, span, @new);

			TimeSpanItemView? foundItem = WorkSpansListView.Items.Select(x => (TimeSpanItemView)((ListViewItem)x).Content).FirstOrDefault(x => x.Span == span);
			if (foundItem != null) {
				foundItem.Span = @new;
			}
		}
		UpdateDayButton();
	}

}
