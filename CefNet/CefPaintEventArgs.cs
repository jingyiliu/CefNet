using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public class CefPaintEventArgs : EventArgs
	{
		public CefPaintEventArgs(CefBrowser browser, CefPaintElementType type, CefRect[] dirtyRects, IntPtr buffer, int width, int height)
		{
			Browser = browser;
			PaintElementType = type;
			DirtyRects = dirtyRects;
			Buffer = buffer;
			Width = width;
			Height = height;
		}

		public CefBrowser Browser { get; }

		public CefPaintElementType PaintElementType { get; }

		public CefRect[] DirtyRects { get; }

		public IntPtr Buffer { get; }

		public int Width { get; }

		public int Height { get; }
	}
}
