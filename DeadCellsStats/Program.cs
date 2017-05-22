
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DeadCellsStats {
	class Program {
		static Process gameProcess;
		static SheetsService service;
		static FileSystemWatcher fileWatcher;
		static Stats savedStats;

		static void Main(string[] args) {
			Process[] processes = Process.GetProcessesByName(Globals.ProcessName);

			if(processes.Length > 0) {
				gameProcess = processes.First();
			} else {
				Console.WriteLine("Please run the game before running this program!");
				Console.Read();
				return;
			}

			using(var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read)) {
				string credPath = System.Environment.GetFolderPath(
					System.Environment.SpecialFolder.Personal);
				credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

				UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Globals.Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;

				// Create Google Sheets API service
				service = new SheetsService(new BaseClientService.Initializer() {
					HttpClientInitializer = credential,
					ApplicationName = Globals.ApplicationName,
				});
			}

			// Create a new FileSystemWatcher and set its properties
			fileWatcher = new FileSystemWatcher();
			fileWatcher.Path = Path.GetDirectoryName(Globals.RunFilePath);
			fileWatcher.Filter = Path.GetFileName(Globals.RunFilePath);

			// Add event handlers
			fileWatcher.Changed += new FileSystemEventHandler(OnChanged);
			fileWatcher.Created += new FileSystemEventHandler(OnCreated);
			fileWatcher.Deleted += new FileSystemEventHandler(OnDeleted);
			fileWatcher.EnableRaisingEvents = true;

			Console.WriteLine("DEAD CELLS STATS");

			// Wait for the user to quit the program
			Console.WriteLine("Press \'q\' to quit the program.");
			while(true) {
				int input = Console.Read();

				if(input == 'g' && savedStats != null) {
					SaveStartingGold(true);
				} else if(input == 'q') {
					return;
				}
			}
		}

		// Called when the 'run.dat' file is deleted
		static void OnDeleted(object sender, FileSystemEventArgs e) {
			Console.WriteLine("Current run has ended.");
			Console.WriteLine("Waiting for a new run to start...");
		}

		// Called when the 'run.dat' file is created
		static void OnCreated(object sender, FileSystemEventArgs e) {
			Console.WriteLine("Run started!");
		}

		// Called when the 'run.dat' file is modified (new zone)
		static void OnChanged(object sender, FileSystemEventArgs e) {
			string tempFileName = Globals.RunFilePath + DateTime.Now.Ticks;
			File.Copy(Globals.RunFilePath, tempFileName);
			Run currentRun = JsonConvert.DeserializeObject<Run>(File.ReadAllText(tempFileName));
			File.Delete(tempFileName);

			string lastLevel = currentRun.levels.Last().id;

			if(Globals.FightZones.Contains(lastLevel)) {
				SaveStats(currentRun);
			} else if(Globals.SafeZones.Contains(lastLevel)) {
				UploadStats(currentRun);
			} else {
				Console.WriteLine("Error: Unknown zone!");
			}
		}

		// Save the stats at the beginning of the zone to compare them later at the end
		static void SaveStats(Run currentRun) {
			Console.WriteLine("Entering fight zone, saving stats...");

			savedStats = new Stats(currentRun, gameProcess);
			savedStats.PrintValues();

			Console.WriteLine("Stats saved successfully!");

			if(currentRun.levels.Last().id.Equals(Globals.FightZones.First())) {
				Task.Run(() => SaveStartingGold(false));
			}
		}

		// Upload the stats to the google doc
		static void UploadStats(Run currentRun) {
			Console.WriteLine("Entering safe zone, uploading stats...");

			string levelToSave = currentRun.levels.ElementAt(currentRun.levels.Length - 2).id;
			string sheetRange = GetSheetRange(levelToSave);

			Stats stats = new Stats(currentRun, gameProcess);
			stats.PrintValues(savedStats);

			if(sheetRange.Length == 0) {
				LocalSave(stats);
				return;
			}

			string updateRange = Globals.Range + '!' + sheetRange;
			Console.WriteLine("Saving run in " + updateRange + "...");

			ValueRange valueRange = new ValueRange();
			valueRange.MajorDimension = "ROWS";
			valueRange.Range = updateRange;
			valueRange.Values = stats.GetValues(savedStats);

			SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, Globals.SpreadsheetId, updateRange);
			update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
			UpdateValuesResponse result = update.Execute();

			Console.WriteLine("Stats saved to Google Doc!");
		}

		// Save the starting gold in the first level, 10s after the beginning or manually if needed
		static void SaveStartingGold(bool manual) {
			if(!manual) {
				Console.WriteLine("First level detected, saving gold in 10s...");
				Thread.Sleep(10000);
			}

			int newGold = savedStats.UpdateGoldValue(gameProcess);

			Console.WriteLine("Gold value saved! (" + newGold + ")");
			Console.WriteLine("Press \'g\' to save it manually if needed.");
		}

		// Get the google doc cell where data should be written
		static string GetSheetRange(string levelToSave) {
			SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(Globals.SpreadsheetId, Globals.Range);
			ValueRange response = request.Execute();
			IList<IList<object>> values = response.Values;

			string result = string.Empty;
			if(values != null) {
				int i = 0;
				while(i < values.Count && (values[i].Count == 0 || !values[i][0].Equals(levelToSave))) {
					i++;
				}
				if(i < values.Count) {
					result = GetColumnName(values[i + 1].Count + 1) + Convert.ToString(i + 2);
				} else {
					Console.WriteLine("Can't find level in spreadsheet, data will be saved in a local file.");
				}
			} else {
				Console.WriteLine("Error while reading the sheet, data will be saved in a local file.");
			}

			return result;
		}

		// Convert integer to an excel column name
		static string GetColumnName(int colIndex) {
			int dividend = colIndex;
			string columnName = string.Empty;
			int modulo;

			while(dividend > 0) {
				modulo = (dividend - 1) % 26;
				columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
				dividend = (dividend - modulo) / 26;
			}

			return columnName;
		}

		// Save the stats in a local file if an error happened with the google doc
		static void LocalSave(Stats stats) {
			// TODO
		}
	}
}