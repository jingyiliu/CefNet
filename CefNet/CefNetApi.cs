using CefNet.Internal;
using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public static class CefNetApi
	{
		public static int GetNativeKeyCode(CefKeyEventType eventType, int repeatCount, VirtualKeys key, bool isSystemKey, bool isExtended)
		{
			if (PlatformInfo.IsWindows)
				return GetWindowsNativeKeyCode(eventType, repeatCount, (byte)NativeMethods.MapVirtualKey((uint)key, MapVirtualKeyType.MAPVK_VK_TO_VSC), isSystemKey, isExtended);
			
			throw new NotImplementedException();
		}

		public static int GetWindowsNativeKeyCode(CefKeyEventType eventType, int repeatCount, byte scanCode, bool isSystemKey, bool isExtended)
		{
			if (repeatCount < 0)
				throw new ArgumentOutOfRangeException(nameof(repeatCount));

			//const int KF_MENUMODE = 0x1000; // 28 bit
			//const int KF_DLGMODE = 0x0800; // 27 bit
			//0x400 not used 26 bit
			//0x200 not used 25 bit

			const int KF_EXTENDED = 0x100; // 24 bit
			const int KF_ALTDOWN = 0x2000; // 29 bit
			const int KF_UP = 0x8000; // 31 bit
			const int KF_REPEAT = 0x4000; // 30 bit

			int keyInfo = scanCode;

			if (eventType == CefKeyEventType.KeyUp)
			{
				repeatCount = 1;
				keyInfo |= KF_UP;
			}

			if (repeatCount > 0)
				keyInfo |= KF_REPEAT;
			else
				repeatCount = 1;

			if (isSystemKey)
				keyInfo |= KF_ALTDOWN;
			if (isExtended)
				keyInfo |= KF_EXTENDED;

			return (keyInfo << 16) | (repeatCount & 0xFFFF);
		}

		/// <summary>
		/// Post an action for execution on the specified thread. This function may be
		/// called on any thread. It is an error to request a thread from the wrong
		/// process.
		/// </summary>
		public static bool Post(CefThreadId threadId, Action action)
		{
			return CefApi.PostTask(threadId, new CefActionTask(action));
		}

		/// <summary>
		/// Post an action for execution on the specified thread. This function may be
		/// called on any thread. It is an error to request a thread from the wrong
		/// process.
		/// </summary>
		public static bool Post(CefThreadId threadId, Action action, long delay)
		{
			return CefApi.PostTask(threadId, new CefActionTask(action), delay);
		}

	}
}
