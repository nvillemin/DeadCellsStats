namespace DeadCellsStats {
	public class Pointer {
		public int address { get; private set; }
		public int[] offsets { get; private set; }

		public Pointer(int address, int[] offsets) {
			this.address = address;
			this.offsets = offsets;
		}
	}
}
