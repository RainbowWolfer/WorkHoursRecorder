using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using WinRT.Interop;
using WorkHoursRecorder.Helpers;
using WorkHoursRecorder.Models;
using WorkHoursRecorder.Views.Controls;

namespace WorkHoursRecorder.Views.Pages;
public sealed partial class SettingsPage : Page {
	public SettingsPage() {
		this.InitializeComponent();
		//12hour or 24hour
		//9.11H or 9.1H or 9H

	}

	private async void ExportButton_Click(object sender, RoutedEventArgs e) {
		ExportButton.IsEnabled = false;

		string json = Local.InfoToJson();
		FileSavePicker savePicker = new() {
			SuggestedFileName = $"WorkDaysInfo",
		};
		savePicker.FileTypeChoices.Add("Json File", new List<string>() { ".json" });

		// Retrieve the window handle (HWND) of the current WinUI 3 window.
		MainWindow? window = (Application.Current as App)?.MainWindow;
		IntPtr hWnd = WindowNative.GetWindowHandle(window);

		// Initialize the folder picker with the window handle (HWND).
		InitializeWithWindow.Initialize(savePicker, hWnd);

		// Display the file picker dialog by calling PickSaveFileAsync
		StorageFile file = await savePicker.PickSaveFileAsync();
		if (file != null) {
			// Prevent updates to the remote version of the file until
			// we finish making changes and call CompleteUpdatesAsync.
			CachedFileManager.DeferUpdates(file);
			// write to file
			await FileIO.WriteTextAsync(file, json);
			// Let Windows know that we're finished changing the file so
			// the other app can update the remote version of the file.
			// Completing updates may require Windows to ask for user input.
			FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
			if (status == FileUpdateStatus.Complete) {
				Debug.WriteLine($"File {file.Name} was saved.");
			} else {
				Debug.WriteLine($"File {file.Name} couldn't be saved.");
			}
		} else {
			Debug.WriteLine($"User cancelled the save dialog.");
		}

		ExportButton.IsEnabled = true;
	}

	private async void ImportButton_Click(object sender, RoutedEventArgs e) {
		ImportButton.IsEnabled = false;
		FileOpenPicker openPicker = new() {

		};
		openPicker.FileTypeFilter.Clear();
		openPicker.FileTypeFilter.Add(".json");

		// Retrieve the window handle (HWND) of the current WinUI 3 window.
		MainWindow? window = (Application.Current as App)?.MainWindow;
		IntPtr hWnd = WindowNative.GetWindowHandle(window);

		// Initialize the folder picker with the window handle (HWND).
		InitializeWithWindow.Initialize(openPicker, hWnd);

		StorageFile file = await openPicker.PickSingleFileAsync();
		if (file != null) {
			string json = await FileIO.ReadTextAsync(file);
			WorkDayInfo? obj = Local.JsonToInfo(json, out Exception? exception);
			if (obj != null && exception == null) {
				ContentDialog dialog = new() {
					XamlRoot = this.XamlRoot,
					Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
					Title = "Import file",
					PrimaryButtonText = "Yes",
					CloseButtonText = "No",
					Content = "Are you sure to override your existing info by imported file?",
				};
				if (await dialog.ShowAsync() == ContentDialogResult.Primary) {
					await Local.Update(obj);
				}
			} else {
				ContentDialog dialog = new() {
					XamlRoot = this.XamlRoot,
					Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
					Title = "Error occured",
					CloseButtonText = "Back",
					Content = new ImportErrorView(exception),
				};
				await dialog.ShowAsync();
			}
		}
		ImportButton.IsEnabled = true;
	}
}
