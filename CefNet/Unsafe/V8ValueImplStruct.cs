using CefNet.CApi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet.Unsafe
{
	[StructLayout(LayoutKind.Sequential)]
#if DEBUG
	public
#endif
	unsafe struct V8ValueImplLayout
	{
		public IntPtr v8value_vtable;
		public IntPtr refcounted_vtable;
		public IntPtr isolate;
		public IntPtr type;
		public V8ValueImpl_ValueUnion value;
		public IntPtr handle;
		public IntPtr last_exception;

		public CefV8ValueType Type
		{
			get { return (CefV8ValueType)(type.ToInt64() & 0xFFFFFFFF); }
		}
	}


	[StructLayout(LayoutKind.Explicit)]
#if DEBUG
	public
#endif
	unsafe struct V8ValueImpl_ValueUnion
	{
		[FieldOffset(0)]
		public byte bool_value_;
		[FieldOffset(0)]
		public int int_value_;
		[FieldOffset(0)]
		public uint uint_value_;
		[FieldOffset(0)]
		public double double_value_;
		[FieldOffset(0)]
		public cef_time_t date_value_;
		[FieldOffset(0)]
		public cef_string_t string_value_;
	}
}
