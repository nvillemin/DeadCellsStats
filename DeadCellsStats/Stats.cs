using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DeadCellsStats {
	class Stats {
		string build;
		int gameSeed, cells, gold, statsGained, time;
		DateTime date;

		public Stats(Run currentRun, Process gameProcess, string levelToSave) {
			this.build = currentRun.build;
			this.gameSeed = currentRun.gameSeed;
			this.date = currentRun.lastUpdate;

			if(levelToSave.Equals("BoatDock")) {
				this.cells = 30;
			} else {
				this.cells = Memory.ReadPointerInteger(gameProcess, Memory.CellsPointer);
			}
			
			this.gold = Memory.ReadPointerInteger(gameProcess, Memory.GoldPointer);
			this.time = (int)Memory.ReadPointerDouble(gameProcess, Memory.TimePointer);

			int weaponsLevel = Memory.ReadPointerInteger(gameProcess, Memory.WeaponsLvlPointer);
			int skillsLevel = Memory.ReadPointerInteger(gameProcess, Memory.SkillsLvlPointer);
			int healthLevel = Memory.ReadPointerInteger(gameProcess, Memory.HealthLvlPointer);

			this.statsGained = weaponsLevel + skillsLevel + healthLevel;
		}

		public int UpdateGoldValue(Process gameProcess) {
			this.gold = Memory.ReadPointerInteger(gameProcess, Memory.GoldPointer);
			return this.gold;
		}

		public void AddBuyValue(int buyValue) {
			this.gold += buyValue;
		}

		public List<IList<object>> GetValues(Stats savedStats) {
			return new List<IList<object>> {
				new List<object>() { "=LEFT(\"" + this.build + "\", 8)" },
				new List<object>() { savedStats.gameSeed },
				new List<object>() { this.date.ToShortDateString() },
				new List<object>() { this.cells - savedStats.cells },
				new List<object>() { this.gold - savedStats.gold },
				new List<object>() { this.statsGained - savedStats.statsGained },
				new List<object>() { this.time - savedStats.time }
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

		public void PrintValues(Stats savedStats) {
			Console.WriteLine("BUILD : " + this.build);
			Console.WriteLine("SEED  : " + this.gameSeed);
			Console.WriteLine("DATE  : " + this.date.ToShortDateString());
			Console.WriteLine("CELLS : " + (this.cells - savedStats.cells));
			Console.WriteLine("GOLD  : " + (this.gold - savedStats.gold));
			Console.WriteLine("STATS : " + (this.statsGained - savedStats.statsGained));
			Console.WriteLine("TIME  : " + (this.time - savedStats.time));
		}
	}
}
