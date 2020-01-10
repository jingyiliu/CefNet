using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CefNet
{
	public class CefProcessMessageReceivedEventArgs : HandledEventArgs
	{
		private string _name;

		public CefProcessMessageReceivedEventArgs(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
		{
			this.Browser = browser;
			this.Frame = frame;
			this.SourceProcess = sourceProcess;
			this.Message = message;
		}

		public string Name
		{
			get
			{
				if (_name == null)
					_name = Message.Name;
				return _name;
			}
		}

		public CefBrowser Browser { get; }

		public CefFrame Frame { get; }

		public CefProcessId SourceProcess { get; }

		public CefProcessMessage Message { get; }
	}
}
