using System;
using CefNet.Unsafe;

namespace CefNet.CApi
{
#pragma warning disable CS1591
	public unsafe partial struct cef_v8value_t
#pragma warning restore CS1591
	{
		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A hash code for the <see cref="cef_v8value_t"/> object.</returns>
		public override int GetHashCode()
		{
			if (IsValid() == 0)
				return 0;

			fixed (cef_v8value_t* self = &this)
			{
				RefCountedWrapperStruct* ws = RefCountedWrapperStruct.FromRefCounted(self);
				V8ValueImplLayout* cppobj = ((V8ValueImplLayout*)(ws->cppObject));
				switch (cppobj->Type)
				{
					case CefV8ValueType.Object:
						V8ValueImplHandleLayout* v8ValueHandle = cppobj->handle;
						if (v8ValueHandle == null)
							return 0;
						IntPtr* handle = v8ValueHandle->handle;
						return (handle != null) ? (*handle).GetHashCode() : 0;
					case CefV8ValueType.Bool:
						return cppobj->value.bool_value_ | (int)CefV8ValueType.Bool;
					case CefV8ValueType.Double:
						return cppobj->value.double_value_.GetHashCode();
					case CefV8ValueType.Int:
					case CefV8ValueType.UInt:
						return cppobj->value.int_value_;
					case CefV8ValueType.Null:
					case CefV8ValueType.Undefined:
						return (int)cppobj->Type;
					case CefV8ValueType.String:
						return cppobj->value.string_value_.GetHashCode();
					case CefV8ValueType.Date:
						return cppobj->value.date_value_.GetHashCode();
				}
			}
			return 0;
		}
	}
}
