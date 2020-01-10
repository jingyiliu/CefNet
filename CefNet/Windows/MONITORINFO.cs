using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet.WinApi
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MONITORINFO
	{
		public int Size;
		public RECT Monitor;
		public RECT Work;
		public int Flags;
	}
}
