using Google.Apis.Sheets.v4;

namespace DeadCellsStats {
	public static class Globals {
		public static string[] Scopes = { SheetsService.Scope.Spreadsheets };
		public static string ApplicationName = "DeadCellsStats";
		public static string ProcessName = "deadcells";
		public static string RunFilePath = "C:\\Jeux\\Steam\\steamapps\\common\\Dead Cells\\save\\run.dat";
		public static string SpreadsheetId = "15GBkK5ugKrg0Xx3-Y1C0splhMk1ZHoAZYEELvZnLQ5U";
		public static string Range = "DATA";
		public static string[] FightZones = { "PrisonStart", "PrisonCourtyard", "SewerShort", "Ossuary", "PrisonRoof",
			"SewerDepths", "PrisonDepths", "Bridge", "StiltVillage", "Cemetery", "BeholderPit" };
		public static string[] SafeZones = { "T_Courtyard", "T_SewerShort", "T_Ossuary", "T_Roof",
			"T_SewerDepths", "T_PrisonDepths", "T_Bridge", "T_StiltVillage", "T_Cemetery", "T_BeholderPit", "BoatDock" };
	}
}
