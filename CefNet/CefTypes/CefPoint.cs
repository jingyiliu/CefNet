using System;
using System.Collections.Generic;
using System.Globalization;
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

		public override string ToString()
		{
			return "{X=" + X.ToString(CultureInfo.InvariantCulture) + ",Y=" + Y.ToString(CultureInfo.InvariantCulture) + "}";
		}
	}
}
