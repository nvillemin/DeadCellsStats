using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DeadCellsStats {
	class Memory {
		const int ProcessAllAccess = 0x1F0FFF;

		[DllImport("kernel32")]
		static extern int OpenProcess(int accessType, int inheritHandle, int processId);
		[DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
		static extern int ReadProcessMemoryInteger(int handle, int address, ref int value, int size, ref int bytesRead);
		[DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
		static extern double ReadProcessMemoryDouble(int handle, int address, ref double value, int size, ref int bytesRead);
		[DllImport("kernel32")]
		static extern int CloseHandle(int handle);

		public static Pointer CellsPointer = new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x68, 0x2E0 });
		public static Pointer GoldPointer = new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x5C, 0x38 });
		public static Pointer TimePointer = new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x5C, 0x20 });
		public static Pointer WeaponsLvlPointer = new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x68, 0xE8 });
		public static Pointer SkillsLvlPointer = new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x68, 0xE4 });
		public static Pointer HealthLvlPointer = new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x68, 0xEC });

		public static int ReadPointerInteger(Process gameProcess, Pointer pointer) {
			int value = -1;
			int pointerAddress = pointer.GetAddress(gameProcess);
			if(gameProcess != null) {
				int bytes = 0;
				int handle = OpenProcess(ProcessAllAccess, 0, gameProcess.Id);
				if(handle != 0) {
					foreach(int offset in pointer.offsets) {
						ReadProcessMemoryInteger(handle, pointerAddress, ref pointerAddress, 4, ref bytes);
						pointerAddress += offset;
					}
					ReadProcessMemoryInteger(handle, pointerAddress, ref value, 4, ref bytes);
					CloseHandle(handle);
				}
			}

			return value;
		}

		public static double ReadPointerDouble(Process gameProcess, Pointer pointer) {
			double value = -1;
			int pointerAddress = pointer.GetAddress(gameProcess);
			if(gameProcess != null) {
				int bytes = 0;
				int handle = OpenProcess(ProcessAllAccess, 0, gameProcess.Id);
				if(handle != 0) {
					foreach(int offset in pointer.offsets) {
						ReadProcessMemoryInteger(handle, pointerAddress, ref pointerAddress, 4, ref bytes);
						pointerAddress += offset;
					}
					ReadProcessMemoryDouble(handle, pointerAddress, ref value, 8, ref bytes);
					CloseHandle(handle);
				}
			}

			return value;
		}
	}
}
