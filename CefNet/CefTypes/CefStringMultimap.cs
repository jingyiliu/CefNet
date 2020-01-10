using CefNet.CApi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet
{
	[StructLayout(LayoutKind.Sequential)]
	public class CefStringMultimap
	{
		private cef_string_multimap_t _instance;

		private CefStringMultimap(cef_string_multimap_t instance)
		{
			_instance = instance;
		}

		public static implicit operator cef_string_multimap_t(CefStringMultimap instance)
		{
			return instance._instance;
		}

		public static implicit operator CefStringMultimap(cef_string_multimap_t instance)
		{
			return new CefStringMultimap(instance);
		}
	}
}
