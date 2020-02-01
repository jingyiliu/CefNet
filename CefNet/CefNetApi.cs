using CefNet.Internal;
using CefNet.WinApi;
using CefNet.Linux;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CefNet
{
	public static class CefNetApi
	{
		public static int GetNativeKeyCode(CefKeyEventType eventType, int repeatCount, VirtualKeys key, CefEventFlags modifiers, bool isExtended)
		{
			if (PlatformInfo.IsWindows)
				return GetWindowsNativeKeyCode(eventType, repeatCount, (byte)NativeMethods.MapVirtualKey((uint)key, MapVirtualKeyType.MAPVK_VK_TO_VSC), modifiers.HasFlag(CefEventFlags.AltDown), isExtended);
			if (PlatformInfo.IsLinux)
				return GetLinuxNativeKeyCode(key, modifiers.HasFlag(CefEventFlags.ShiftDown));

			return 0;
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

		public static VirtualKeys GetVirtualKey(char c)
		{
			if (PlatformInfo.IsWindows)
			{
				int virtualKeyCode = (WinApi.NativeMethods.VkKeyScan(c) & 0xFF);
				if (virtualKeyCode == 0xFF)
					throw new InvalidOperationException("Incompatible input locale.");
				return (VirtualKeys)virtualKeyCode;
			}

			if (PlatformInfo.IsLinux)
			{
				XKeySym keysym = Linux.KeyInterop.CharToXKeySym(c);
				if (keysym == XKeySym.None)
					keysym = Linux.NativeMethods.XStringToKeysym("U" + ((int)c).ToString("X"));
				return Linux.KeyInterop.XKeySymToVirtualKey(TranslateXKeySymToAsciiXKeySym(keysym));
			}

			throw new NotImplementedException();
		}

		public static XKeySym TranslateXKeySymToAsciiXKeySym(XKeySym keysym)
		{
			IntPtr display = Linux.NativeMethods.XOpenDisplay(IntPtr.Zero);
			try
			{
				byte keycode = Linux.NativeMethods.XKeysymToKeycode(display, keysym);
				return Linux.NativeMethods.XKeycodeToKeysym(display, keycode, 0);
			}
			finally
			{
				Linux.NativeMethods.XCloseDisplay(display);
			}
		}

		public static byte GetHardwareKeyCode(XKeySym keysym)
		{
			IntPtr display = Linux.NativeMethods.XOpenDisplay(IntPtr.Zero);
			try
			{
				return Linux.NativeMethods.XKeysymToKeycode(display, keysym);
			}
			finally
			{
				Linux.NativeMethods.XCloseDisplay(display);
			}
		}

		public static int GetLinuxNativeKeyCode(VirtualKeys key, bool shift)
		{
			XKeySym keysym = Linux.KeyInterop.VirtualKeyToXKeySym(key, shift);
			if (keysym == XKeySym.None)
				return 0;
			return GetHardwareKeyCode(keysym);
		}

		/// <summary>
		/// Converts the value of a UTF-16 encoded character into a native key code.
		/// </summary>
		/// <param name="c"></param>
		/// <param name="repeatCount"></param>
		/// <param name="modifiers"></param>
		/// <param name="isExtended"></param>
		public static int GetNativeKeyCode(char c, int repeatCount, CefEventFlags modifiers, bool isExtended)
		{
			if (PlatformInfo.IsWindows)
			{
				return GetWindowsNativeKeyCode(CefKeyEventType.Char, repeatCount,
					(byte)(WinApi.NativeMethods.VkKeyScan(c) & 0xFF),
					modifiers.HasFlag(CefEventFlags.AltDown), isExtended);
			}

			if (PlatformInfo.IsLinux)
				return GetLinuxNativeKeyCode(c);

			throw new NotImplementedException();
		}

		public static int GetLinuxNativeKeyCode(char c)
		{
			XKeySym keysym = Linux.KeyInterop.CharToXKeySym(c);
			if (keysym == XKeySym.None)
				return 0;
			return GetHardwareKeyCode(keysym);
		}

		public static bool IsShiftRequired(this char c)
		{
			if (c >= '!' && c <= '+')
				return c != '\'';
			if (c == ':')
				return true;
			if (c >= '<' && c <= '@')
				return c != '=';
			if (c == '^' && c == '_')
				return true;
			if (c >= '{' && c <= '~')
				return true;
			return char.IsUpper(c);
		}

		public static T CompareExchange<T>(in T location, T value, T comparand)
			where T : class
		{
			return UtilsExtensions.As<T>(Interlocked.CompareExchange(ref UtilsExtensions.AsRef<T, object>(in location), value, comparand));
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
