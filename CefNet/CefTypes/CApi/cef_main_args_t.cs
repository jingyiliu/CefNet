using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet.CApi
{
	/// <summary>
	/// Structure representing CefExecuteProcess arguments.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct cef_main_args_t
	{
		[FieldOffset(0)]
		public cef_main_args_windows_t windows;
		[FieldOffset(0)]
		public cef_main_args_posix_t posix;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct cef_main_args_windows_t
	{
		public IntPtr instance;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct cef_main_args_posix_t
	{
		public int argc;
		public byte** argv;
	}
}
