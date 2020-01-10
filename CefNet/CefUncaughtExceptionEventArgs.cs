using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public class CefUncaughtExceptionEventArgs : EventArgs
	{
		public CefUncaughtExceptionEventArgs(CefBrowser browser, CefFrame frame, CefV8Context context, CefV8Exception exception, CefV8StackTrace stackTrace)
		{
			this.Browser = browser;
			this.Frame = frame;
			this.Context = context;
			this.Exception = exception;
			this.StackTrace = stackTrace;
		}

		public CefBrowser Browser { get; }

		public CefFrame Frame { get; }

		public CefV8Context Context { get; }

		public CefV8Exception Exception { get; }

		public CefV8StackTrace StackTrace { get; }
	}
}
