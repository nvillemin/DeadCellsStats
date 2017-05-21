using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeadCellsStats {
	class Stats {
		string build;
		int gameSeed, cells, gold, statsGained, time;
		DateTime date;

		public Stats(Run currentRun, Process gameProcess) {
			this.build = currentRun.build;
			this.gameSeed = currentRun.gameSeed;
			this.date = currentRun.lastUpdate;
			this.cells = Memory.ReadPointerInteger(gameProcess, Memory.CellsPointer);
			this.gold = Memory.ReadPointerInteger(gameProcess, Memory.GoldPointer);
			this.time = (int)Memory.ReadPointerDouble(gameProcess, Memory.TimePointer);

			int weaponsLevel = Memory.ReadPointerInteger(gameProcess, Memory.WeaponsLvlPointer);
			int skillsLevel = Memory.ReadPointerInteger(gameProcess, Memory.SkillsLvlPointer);
			int healthLevel = Memory.ReadPointerInteger(gameProcess, Memory.HealthLvlPointer);

			this.statsGained = weaponsLevel + skillsLevel + healthLevel;
		}

		public List<IList<object>> GetValues() {
			return new List<IList<object>> {
				new List<object>() { "=LEFT(\"" + this.build + "\", 8)" },
				new List<object>() { this.gameSeed },
				new List<object>() { this.date.ToShortDateString() },
				new List<object>() { this.cells },
				new List<object>() { this.gold },
				new List<object>() { this.statsGained },
				new List<object>() { this.time }
			};
		}

		public void PrintValues() {
			Console.WriteLine("BUILD : " + this.build);
			Console.WriteLine("SEED  : " + this.gameSeed);
			Console.WriteLine("DATE  : " + this.date.ToShortDateString());
			Console.WriteLine("CELLS : " + this.cells);
			Console.WriteLine("GOLD  : " + this.gold);
			Console.WriteLine("STATS : " + this.statsGained);
			Console.WriteLine("TIME  : " + this.time);
		}
	}
}
