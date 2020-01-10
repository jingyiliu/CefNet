using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CefNet.Windows.Forms
{
	public static class CefNetWinformsExtensions
	{
		public static Rectangle ToRectangle(ref this CefRect self)
		{
			return new Rectangle(self.X, self.Y, self.Width, self.Height);
		}


	}
}
