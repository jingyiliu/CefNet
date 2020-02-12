using CefNet.Internal;
using CefNet.WinApi;
using CefNet.Linux;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CefNet
{
	/// <summary>
	/// Provide global methods.
	/// </summary>
	public static class CefNetApi
	{
		/// <summary>
		/// Translates a virtual key to the corresponding native key code for the current keyboard.
		/// </summary>
		/// <param name="eventType">The key event type.</param>
		/// <param name="repeatCount">The repeat count for a message.</param>
		/// <param name="modifiers">A bitwise combination of the <see cref="CefEventFlags"/> values.</param>
		/// <param name="key">The virtual key.</param>
		/// <param name="isExtended">The extended key flag.</param>
		/// <returns>A native key code for the current keyboard.</returns>
		public static int GetNativeKeyCode(CefKeyEventType eventType, int repeatCount, VirtualKeys key, CefEventFlags modifiers, bool isExtended)
		{
			if (PlatformInfo.IsWindows)
				return GetWindowsNativeKeyCode(eventType, repeatCount, (byte)NativeMethods.MapVirtualKey((uint)key, MapVirtualKeyType.MAPVK_VK_TO_VSC), modifiers.HasFlag(CefEventFlags.AltDown), isExtended);
			if (PlatformInfo.IsLinux)
				return GetLinuxNativeKeyCode(key, modifiers.HasFlag(CefEventFlags.ShiftDown));

			return 0;
		}

		/// <summary>
		/// Translates a virtual key to the corresponding native key code for the current keyboard.
		/// </summary>
		/// <param name="eventType">The key event type.</param>
		/// <param name="repeatCount">The repeat count for a message.</param>
		/// <param name="scanCode">The scan code for a key.</param>
		/// <param name="isSystemKey">The system key flag.</param>
		/// <param name="isExtended">The extended key flag.</param>
		/// <returns>A native key code for the current keyboard.</returns>
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
		/// Translates a character to the corresponding virtual-key code for the current keyboard.
		/// </summary>
		/// <param name="c">The character to be translated into a virtual-key code.</param>
		/// <returns>The virtual key code.</returns>
		/// <exception cref="InvalidOperationException">
		/// The function finds no key that translates to the passed character code.
		/// Perhaps the wrong locale is being used.
		/// </exception>
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

		/// <summary>
		/// Translates a KeySym to the corresponding KeySym from latin range for the current keyboard.
		/// </summary>
		/// <param name="keysym">The KeySym to be translated.</param>
		/// <returns>The KeySym.</returns>
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

		/// <summary>
		/// Returns a hardware key code for the specified X keysym (only Linux OS).
		/// </summary>
		/// <param name="keysym">Specifies the KeySym.</param>
		/// <returns>A hardware key code.</returns>
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

		/// <summary>
		/// Returns a native key code for the specified virtual key.
		/// </summary>
		/// <param name="key">Specifies the virtual key.</param>
		/// <param name="shift">Specifies a Shift key state.</param>
		/// <returns>A native key code for the current keyboard.</returns>
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
		/// <param name="c">The character.</param>
		/// <param name="repeatCount">The repeat count for a message.</param>
		/// <param name="modifiers">A bitwise combination of the <see cref="CefEventFlags"/> values.</param>
		/// <param name="isExtended">The extended key flag.</param>
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

		/// <summary>
		/// Converts the value of a UTF-16 encoded character into a native key code.
		/// </summary>
		/// <param name="c">The character to be converted.</param>
		public static int GetLinuxNativeKeyCode(char c)
		{
			XKeySym keysym = Linux.KeyInterop.CharToXKeySym(c);
			if (keysym == XKeySym.None)
				return 0;
			return GetHardwareKeyCode(keysym);
		}

		/// <summary>
		/// Determines that a character requires the Shift modifier key.
		/// </summary>
		/// <param name="c">The Unicode character to evaluate.</param>
		/// <returns>
		/// Returns true if a character requires the Shift modifier key;
		/// otherwise, false.
		/// </returns>
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

		/// <summary>
		/// Compares two instances of the specified reference type <typeparamref name="T"/>
		/// for reference equality and, if they are equal, replaces the first one.
		/// </summary>
		/// <typeparam name="T">
		/// The type to be used for <paramref name="location"/>, <paramref name="value"/>,
		/// and <paramref name="comparand"/>. This type must be a reference type.
		/// </typeparam>
		/// <param name="location">
		/// The destination, whose value is compared by reference with <paramref name="comparand"/>
		/// and possibly replaced.
		/// </param>
		/// <param name="value">
		/// The value that replaces the destination value if the comparison by reference
		/// results in equality.
		/// </param>
		/// <param name="comparand">
		/// The value that is compared by reference to the value at <paramref name="location"/>.
		/// </param>
		/// <returns></returns>
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
