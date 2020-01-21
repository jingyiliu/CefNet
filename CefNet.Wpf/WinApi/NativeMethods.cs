using System;
using System.Runtime.InteropServices;

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
	}
}
