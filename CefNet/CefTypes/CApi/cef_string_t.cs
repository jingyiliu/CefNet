using System;
using System.Runtime.InteropServices;

namespace CefNet.CApi
{
	public unsafe partial struct cef_string_t
	{
		public char* Str
		{
			get { return Base.str; }
			set { Base.str = value; }
		}

		public int Length
		{
			get { return (int)Base.length; }
			set { Base.length = unchecked((UIntPtr)value); }
		}
	}
}
