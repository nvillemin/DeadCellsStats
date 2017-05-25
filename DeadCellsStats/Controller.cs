using System.Runtime.InteropServices;

namespace DeadCellsStats {
	public class Controller {
		[DllImport("XInput1_4.dll")]
		public static extern int XInputGetState(int dwUserIndex, ref XInputState pState);

		[StructLayout(LayoutKind.Explicit)]
		public struct XInputState {
			[FieldOffset(0)]
			public int PacketNumber;

			[FieldOffset(4)]
			public XInputGamepad Gamepad;

			public void Copy(XInputState source) {
				PacketNumber = source.PacketNumber;
				Gamepad.Copy(source.Gamepad);
			}

			public override bool Equals(object obj) {
				if((obj == null) || (!(obj is XInputState)))
					return false;
				XInputState source = (XInputState)obj;

				return ((PacketNumber == source.PacketNumber) && (Gamepad.Equals(source.Gamepad)));
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct XInputGamepad {
			[MarshalAs(UnmanagedType.I2)]
			[FieldOffset(0)]
			public short wButtons;

			[MarshalAs(UnmanagedType.I1)]
			[FieldOffset(2)]
			public byte bLeftTrigger;

			[MarshalAs(UnmanagedType.I1)]
			[FieldOffset(3)]
			public byte bRightTrigger;

			[MarshalAs(UnmanagedType.I2)]
			[FieldOffset(4)]
			public short sThumbLX;

			[MarshalAs(UnmanagedType.I2)]
			[FieldOffset(6)]
			public short sThumbLY;

			[MarshalAs(UnmanagedType.I2)]
			[FieldOffset(8)]
			public short sThumbRX;

			[MarshalAs(UnmanagedType.I2)]
			[FieldOffset(10)]
			public short sThumbRY;


			public bool IsButtonPressed(int buttonFlags) {
				return (wButtons & buttonFlags) == buttonFlags;
			}

			public bool IsButtonPresent(int buttonFlags) {
				return (wButtons & buttonFlags) == buttonFlags;
			}

			public void Copy(XInputGamepad source) {
				sThumbLX = source.sThumbLX;
				sThumbLY = source.sThumbLY;
				sThumbRX = source.sThumbRX;
				sThumbRY = source.sThumbRY;
				bLeftTrigger = source.bLeftTrigger;
				bRightTrigger = source.bRightTrigger;
				wButtons = source.wButtons;
			}

			public override bool Equals(object obj) {
				if(!(obj is XInputGamepad))
					return false;
				XInputGamepad source = (XInputGamepad)obj;
				return ((sThumbLX == source.sThumbLX)
				&& (sThumbLY == source.sThumbLY)
				&& (sThumbRX == source.sThumbRX)
				&& (sThumbRY == source.sThumbRY)
				&& (bLeftTrigger == source.bLeftTrigger)
				&& (bRightTrigger == source.bRightTrigger)
				&& (wButtons == source.wButtons));
			}
		}
	}
}
