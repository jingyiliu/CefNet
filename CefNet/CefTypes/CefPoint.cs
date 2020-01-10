using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public partial struct CefPoint
	{
		public CefPoint(int x, int y)
		{
			_instance.x = x;
			_instance.y = y;
		}

		public void Scale(float value)
		{
			_instance.x = (int)(_instance.x * value);
			_instance.y = (int)(_instance.y * value);
		}
	}
}
