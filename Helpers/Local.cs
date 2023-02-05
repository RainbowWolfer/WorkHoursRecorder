using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using WorkHoursRecorder.Models;

namespace WorkHoursRecorder.Helpers;
public static class Local {
	public static WorkDayInfo? Info { get; private set; }
	public static StorageFolder LocalFolder => ApplicationData.Current.LocalFolder;

	public static StorageFile? File { get; private set; }

	static Local() {
		Debug.WriteLine(LocalFolder.Path);
	}

	public static async Task Initialize() {
		Info = await Load();
	}

	public static async Task AddWorkDayInfo(Date day, Time start, Time end) {
		Info?.Add(day, start, end);
		await Save();
	}

	public static async Task RemoveWorkDayInfo(Date day, WorkTimeSpan span) {
		Info?.Remove(day, span);
		await Save();
	}

	public static async Task EditWorkDayInfo(Date day, WorkTimeSpan old, WorkTimeSpan @new) {
		Info?.Edit(day, old, @new);
		await Save();
	}

	public static List<WorkTimeSpan> GetTimesByDay(Date day) {
		return Info?.GetDateByDate(day.Year, day.Month, day.Day) ?? new List<WorkTimeSpan>();
	}

	public static List<Pair<Date, List<WorkTimeSpan>>> GetWorkDaysInMonth(int year, int month) {
		return Info?.GetWorkDaysInMonth(year, month) ?? new List<Pair<Date, List<WorkTimeSpan>>>();
	}

	public static async Task Update(WorkDayInfo? info) {
		Info = info;
		await Save();
	}

	public static async Task Save() {
		string json = JsonConvert.SerializeObject(Info, Formatting.Indented);
		await FileIO.WriteTextAsync(await GetFile(), json);
	}

	public static async Task<WorkDayInfo> Load() {
		string json = await FileIO.ReadTextAsync(await GetFile());
		return JsonConvert.DeserializeObject<WorkDayInfo?>(json) ?? new WorkDayInfo();
	}

	public static async Task<StorageFile> GetFile() {
		const string FILE_NAME = "info.json";
		if (File == null) {
			File = await LocalFolder.CreateFileAsync(FILE_NAME, CreationCollisionOption.OpenIfExists);
		}
		return File;
	}

	public static string InfoToJson() {
		return JsonConvert.SerializeObject(Info, Formatting.Indented);
	}

	public static WorkDayInfo? JsonToInfo(string json, out Exception? exception) {
		try {
			exception = null;
			return JsonConvert.DeserializeObject<WorkDayInfo?>(json);
		} catch (Exception ex) {
			exception = ex;
			return null;
		}
	}
}
