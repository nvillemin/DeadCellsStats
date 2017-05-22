using System.Diagnostics;

namespace DeadCellsStats {
	public class Pointer {
		int address;
		public int[] offsets { get; private set; }

		public Pointer(int address, int[] offsets) {
			this.address = address;
			this.offsets = offsets;
		}

		public int GetAddress(Process gameProcess) {
			return this.address + gameProcess.MainModule.BaseAddress.ToInt32();
		}
	}
}
