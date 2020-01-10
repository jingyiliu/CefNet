using CefNet.CApi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;


namespace CefNet
{
	[StructLayout(LayoutKind.Sequential)]
	public struct CefColor
	{
		private cef_color_t _instance;

		private CefColor(cef_color_t instance)
		{
			_instance = instance;
		}

		public byte A
		{
			get { return (byte)((_instance.Base >> 24) & 0xFF); }
		}

		public byte R
		{
			get { return (byte)((_instance.Base >> 16) & 0xFF); }
		}

		public byte G
		{
			get { return (byte)((_instance.Base >> 8) & 0xFF); }
		}

		public byte B
		{
			get { return (byte)(_instance.Base & 0xFF); }
		}

		public int ToArgb()
		{
			return (int)_instance.Base;
		}

		public static CefColor FromArgb(int argb)
		{
			return new CefColor { _instance = { Base = (uint)argb } };
		}

		public static implicit operator cef_color_t(CefColor instance)
		{
			return instance._instance;
		}

		public static implicit operator CefColor(cef_color_t instance)
		{
			return new CefColor(instance);
		}


		public static implicit operator int(CefColor color)
		{
			return (int)color._instance.Base;
		}

		public static implicit operator CefColor(int argb)
		{
			return new CefColor { _instance = { Base = (uint)argb } };
		}

	}
}	

