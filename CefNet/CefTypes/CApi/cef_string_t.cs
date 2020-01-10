using System;
using System.Runtime.InteropServices;

namespace CefNet.CApi
{
	public unsafe partial struct cef_string_t
	{
		internal char* Str
		{
			get { return Base.str; }
			set { Base.str = value; }
		}

		internal int Length
		{
			get { return (int)Base.length; }
			set { Base.length = unchecked((UIntPtr)value); }
		}
	}
}
