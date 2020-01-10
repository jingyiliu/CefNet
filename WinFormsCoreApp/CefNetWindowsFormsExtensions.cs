using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CefNet.WinApi;

namespace CefNet
{
	public static class CefNetWindowsFormsExtensions
	{
		public static TabControl FindTabControl(this TabPage tab)
		{
			Control control = tab;
			while (control != null)
			{
				if (control is TabControl tabControl)
					return tabControl;
				control = control.Parent;
			}
			return null;
		}
	}
}
