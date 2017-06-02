using System;

namespace DeadCellsStats {
	class Run {
		public DateTime start { get; set; }
		public Level[] levels { get; set; }
		public int gameSeed { get; set; }
		public string build { get; set; }
		public GcStats gcStats { get; set; }
		public DateTime lastUpdate { get; set; }
		public DateTime[] reloads { get; set; }
	}
}
