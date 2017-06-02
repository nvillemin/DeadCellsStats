using System;
using System.Collections.Generic;
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

		public enum PointerType { Cells, Gold, Time, WeaponsLvl, SkillsLvl, HealthLvl };

		static Dictionary<Tuple<string, PointerType>, Pointer> Pointers = new Dictionary<Tuple<string, PointerType>, Pointer>() {
			{ new Tuple<string, PointerType>("d00e278c", PointerType.Cells),		new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x68, 0x2E0 }) },
			{ new Tuple<string, PointerType>("d00e278c", PointerType.Gold),			new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x5C, 0x38 }) },
			{ new Tuple<string, PointerType>("d00e278c", PointerType.Time),			new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x5C, 0x20 }) },
			{ new Tuple<string, PointerType>("d00e278c", PointerType.WeaponsLvl),	new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x68, 0xE8 }) },
			{ new Tuple<string, PointerType>("d00e278c", PointerType.SkillsLvl),	new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x68, 0xE4 }) },
			{ new Tuple<string, PointerType>("d00e278c", PointerType.HealthLvl),	new Pointer(0x00011880, new int[5] { 0x13C, 0x3E4, 0x18, 0x68, 0xEC }) },

			{ new Tuple<string, PointerType>("e2c7b364", PointerType.Cells),		new Pointer(0x00011880, new int[5] { 0x40, 0xEC, 0x368, 0x68, 0x2E8 }) },
			{ new Tuple<string, PointerType>("e2c7b364", PointerType.Gold),			new Pointer(0x00011880, new int[5] { 0x40, 0xEC, 0x368, 0x5C, 0x38 }) },
			{ new Tuple<string, PointerType>("e2c7b364", PointerType.Time),			new Pointer(0x00011880, new int[5] { 0x40, 0xEC, 0x368, 0x5C, 0x20 }) },
			{ new Tuple<string, PointerType>("e2c7b364", PointerType.WeaponsLvl),	new Pointer(0x00011880, new int[5] { 0x40, 0xEC, 0x368, 0x68, 0xF0 }) },
			{ new Tuple<string, PointerType>("e2c7b364", PointerType.SkillsLvl),	new Pointer(0x00011880, new int[5] { 0x40, 0xEC, 0x368, 0x68, 0xF4 }) },
			{ new Tuple<string, PointerType>("e2c7b364", PointerType.HealthLvl),	new Pointer(0x00011880, new int[5] { 0x40, 0xEC, 0x368, 0x68, 0xF8 }) },
		};

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

		public static Pointer GetPointer(string build, PointerType pointerType) {
			return Pointers[new Tuple<string, PointerType>(build.Substring(0, 8), pointerType)];
		}
	}
}
