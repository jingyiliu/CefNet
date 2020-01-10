using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	partial class CefClientGlue
	{
		public void NotifyPopupBrowserCreating()
		{
			_implementation.NotifyPopupBrowserCreating();
		}
	}
}
