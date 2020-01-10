using CefNet.CApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public partial struct CefRect
	{
		public CefRect(int x, int y, int width, int height)
		{
			_instance = new cef_rect_t { x = x, y = y, width = width, height = height };
		}

		public bool IsNullSize
		{
			get
			{
				return _instance.width == 0 || _instance.height == 0;
			}
		}

		public int Right
		{
			get { return _instance.x + _instance.width; }
		}

		public int Bottom
		{
			get { return _instance.y + _instance.height; }
		}

		public CefSize Size
		{
			get { return new CefSize(_instance.width, _instance.height); }
		}

		public void Scale(float scale)
		{
			if (scale <= 0)
				throw new ArgumentOutOfRangeException(nameof(scale));

			if (scale == 1.0)
				return;
			_instance.x = (int)(_instance.x * scale);
			_instance.y = (int)(_instance.y * scale);
			_instance.width = (int)(_instance.width * scale);
			_instance.height = (int)(_instance.height * scale);
		}

		public void Inflate(int x, int y)
		{
			_instance.x -= x;
			_instance.y -= y;
			_instance.width += (x << 1);
			_instance.height += (y << 1);
		}

		public void Offset(int x, int y)
		{
			_instance.x += x;
			_instance.y += y;
		}

		public void Union(CefRect rect)
		{
			int x = Math.Min(X, rect.X);
			int right = Math.Max(X + Width, rect.X + rect.Width);
			int y = Math.Min(Y, rect.Y);
			int bottom = Math.Max(Y + Height, rect.Y + rect.Height);
			_instance.x = x;
			_instance.y = y;
			_instance.width = right - x;
			_instance.height = bottom - y;
		}

	}
}
