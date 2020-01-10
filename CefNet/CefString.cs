// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------

using CefNet.CApi;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet
{
	public static unsafe class CefString
	{
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private delegate void CefStringDestructorDelegate(IntPtr str);

		private static readonly CefStringDestructorDelegate fnDtor = Marshal.FreeHGlobal;

		internal static readonly void* DestructorAddress = (void*)Marshal.GetFunctionPointerForDelegate(fnDtor);

		public static cef_string_userfree_t Create(string s)
		{
			cef_string_userfree_t str = default;
			str.Base = CefNativeApi.cef_string_userfree_utf16_alloc();
			if (s != null)
			{
				str.Base.Base->str = (char*)Marshal.StringToHGlobalUni(s);
				str.Base.Base->length = (UIntPtr)s.Length;
				str.Base.Base->dtor = DestructorAddress;
			}
			return str;
		}

		public static string Read(cef_string_t* str)
		{
			unchecked
			{
				cef_string_utf16_t* s = (cef_string_utf16_t*)str;
				if (s == default || s->str == default)
					return null;
				return Marshal.PtrToStringUni((IntPtr)s->str, (int)s->length);
			}
		}

		public static string ReadAndFree(cef_string_t* str)
		{
			cef_string_utf16_t* s = (cef_string_utf16_t*)str;
			if (s == null || s->str == null)
				return null;

			string rv = Marshal.PtrToStringUni((IntPtr)s->str, (int)s->length);
			if (s->dtor != default)
			{
				s->Dtor(s->str);
				s->dtor = default;
			}
			s->str = default;
			s->length = UIntPtr.Zero;
			return rv;
		}

		public static string ReadAndFree(cef_string_userfree_t str)
		{
			cef_string_utf16_t* s = str.Base.Base;
			if (s == null)
				return null;
			string rv = Marshal.PtrToStringUni((IntPtr)s->str, (int)s->length);
			CefNativeApi.cef_string_userfree_utf16_free(str.Base);
			return rv;
		}

		public static void Free(cef_string_t* str)
		{
			cef_string_utf16_t* s = (cef_string_utf16_t*)str;
			if (s->dtor != null)
			{
				s->Dtor(s->str);
				s->dtor = default;
			}
			s->str = default;
			s->length = UIntPtr.Zero;
		}

		public static void Free(cef_string_userfree_t str)
		{
			CefNativeApi.cef_string_userfree_utf16_free(str.Base);
		}

		public static void Replace(cef_string_t* str, string value)
		{
			unchecked
			{
				cef_string_utf16_t* s = (cef_string_utf16_t*)str;
				if (s->str != default && s->dtor != default)
				{
					s->Dtor(s->str);
				}
				if (value == null)
				{
					s->str = null;
					s->length = default;
					s->dtor = null;
				}
				else
				{
					s->str = (char*)Marshal.StringToHGlobalUni(value);
					s->length = unchecked((UIntPtr)value.Length);
					s->dtor = CefString.DestructorAddress;
				}
			}
		}

	}


}
