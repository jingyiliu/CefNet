using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet.WinApi
{
	static class NativeMethods
	{
		[DllImport("user32.dll")]
		public static extern IntPtr GetKeyboardLayout(ushort dwLayout);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, int flags);

		[DllImport("user32.dll", EntryPoint = "VkKeyScanW", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern ushort VkKeyScan(char ch);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr pPid);

		[DllImport("Dwmapi.dll", CharSet = CharSet.Auto)]
		public static unsafe extern int DwmGetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attribute, void* value, int size);

		[DllImport("Dwmapi.dll", CharSet = CharSet.Auto, PreserveSig = false)]
		public static extern bool DwmIsCompositionEnabled();
	}
}
