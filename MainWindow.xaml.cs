// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WorkHoursRecorder.Helpers;
using WorkHoursRecorder.Models;
using WorkHoursRecorder.Views.Controls;
using WorkHoursRecorder.Views.Pages;

namespace WorkHoursRecorder;
public sealed partial class MainWindow : Window {
	public static MainWindow? Instance { get; private set; }
	public MainWindow() {
		Instance = this;
		this.InitializeComponent();

		ExtendsContentIntoTitleBar = true;
		SetTitleBar(AppTitleBar);

		MainFrame.Navigate(typeof(InitialPage), null, new EntranceNavigationTransitionInfo());
	}

	public void SetIcon(ApplicationTheme theme) {
		AppTitleIcon.Source = theme switch {
			ApplicationTheme.Light => new BitmapImage(new Uri("ms-appx:///Resources/Icons/Clock-Black.png")),
			ApplicationTheme.Dark => new BitmapImage(new Uri("ms-appx:///Resources/Icons/Clock-White.png")),
			_ => throw new Exception(),
		};
	}

	public void Intialize() {
		RootNavigation.SelectedItem = RootNavigation.MenuItems.First();
		MainDatePicker.SelectedDate = new DateTimeOffset(DateTime.Today);
		RootNavigation.IsEnabled = true;
	}

	private void RootNavigation_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
		Type targetType;
		if (args.IsSettingsSelected) {
			targetType = typeof(SettingsPage);
		} else if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag) {
			targetType = tag switch {
				"0" => typeof(CalendarPage),
				_ => throw new NotImplementedException(),
			};
		} else {
			return;
		}

		MainFrame.Navigate(targetType, null, args.RecommendedNavigationTransitionInfo);
	}

	private void PreviousMonthButton_Click(object sender, RoutedEventArgs e) {
		DateTimeOffset date = GetDatePickerValue() ?? new DateTimeOffset(DateTime.Today);
		MainDatePicker.SelectedDate = date.AddMonths(-1);
	}

	private void NextMonthButton_Click(object sender, RoutedEventArgs e) {
		DateTimeOffset date = GetDatePickerValue() ?? new DateTimeOffset(DateTime.Today);
		MainDatePicker.SelectedDate = date.AddMonths(1);
	}
	private void ReturnMonthButton_Click(object sender, RoutedEventArgs e) {
		MainDatePicker.SelectedDate = new DateTimeOffset(DateTime.Today);
	}

	private void MainDatePicker_SelectedDateChanged(DatePicker sender, DatePickerSelectedValueChangedEventArgs args) {
		if (MainFrame.Content is not CalendarPage page) {
			return;
		}

		DateTimeOffset? date = args.NewDate;
		if (date == null) {
			return;
		}
		page.LoadByDate(date.Value.Year, date.Value.Month);
	}

	public static DateTimeOffset? GetDatePickerValue() {
		if (Instance == null) {
			return null;
		}
		return Instance.MainDatePicker.SelectedDate;
	}

	private async void CalculateButton_Click(object sender, RoutedEventArgs e) {
		DateTimeOffset? date = GetDatePickerValue();
		if (date == null) {
			return;
		}

		ContentDialog dialog = new() {
			XamlRoot = RootGrid.XamlRoot,
			Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
			Title = "Month Summary",
			CloseButtonText = "Back",
			Content = new MonthSummaryView(Local.GetWorkDaysInMonth(date.Value.Year, date.Value.Month)),
		};
		await dialog.ShowAsync();

	}
}
