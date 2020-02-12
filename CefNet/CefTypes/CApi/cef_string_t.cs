using System;
using System.Runtime.InteropServices;

namespace CefNet.CApi
{
	/// <summary>
	/// Represents CEF string.
	/// </summary>
	public unsafe partial struct cef_string_t
	{
		/// <summary>
		/// Gets and sets the pointer to allocated memory for the current string.
		/// </summary>
		public char* Str
		{
			get { return Base.str; }
			set { Base.str = value; }
		}

		/// <summary>
		/// Gets and sets the size of the current CEF string.
		/// </summary>
		public int Length
		{
			get { return (int)Base.length; }
			set { Base.length = unchecked((UIntPtr)value); }
		}
	}
}
