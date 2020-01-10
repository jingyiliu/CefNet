using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Windows.Forms
{
	public sealed class TooltipEventArgs : EventArgs
	{
		public TooltipEventArgs(string text)
		{
			this.Text = text;
		}

		public string Text { get; }
	}
}
