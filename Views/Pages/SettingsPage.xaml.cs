using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using WorkHoursRecorder.Helpers;

namespace WorkHoursRecorder.Views.Pages;
public sealed partial class SettingsPage : Page {
	public SettingsPage() {
		this.InitializeComponent();
		//12hour or 24hour
		//9.11H or 9.1H or 9H

	}

	private async void ExportButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) {
		string json = Local.InfoToJson();
		FileSavePicker savePicker = new() {
			SuggestedFileName = $"WorkDaysInfo",
		};
		savePicker.FileTypeChoices.Add("Json File", new List<string>() { ".json" }); 

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
	}

	private void ImportButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) {

	}
}
