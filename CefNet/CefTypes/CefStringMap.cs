using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CefNet.CApi;

namespace CefNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct CefStringMap
	{
		private cef_string_map_t _instance;

		private CefStringMap(cef_string_map_t instance)
		{
			_instance = instance;
		}

		public static implicit operator cef_string_map_t(CefStringMap instance)
		{
			return instance._instance;
		}

		public static implicit operator CefStringMap(cef_string_map_t instance)
		{
			return new CefStringMap(instance);
		}
	}
}
