using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public class LoadingStateChangeEventArgs : EventArgs
	{
		public LoadingStateChangeEventArgs(bool busy, bool canGoBack, bool canGoForward)
		{
			this.Busy = busy;
			this.CanGoBack = canGoBack;
			this.CanGoForward = canGoForward;
		}

		public bool Busy { get; }

		public bool CanGoBack { get; }

		public bool CanGoForward { get; }

	}
}
