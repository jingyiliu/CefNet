using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	public partial class WebViewGlue
	{
		internal bool AvoidOnPreKeyEvent()
		{
			return false;
		}

		internal protected virtual bool OnPreKeyEvent(CefBrowser browser, CefKeyEvent @event, CefEventHandle osEvent, ref int isKeyboardShortcut)
		{
			return false;
		}

		internal bool AvoidOnKeyEvent()
		{
			return false;
		}

		internal protected virtual bool OnKeyEvent(CefBrowser browser, CefKeyEvent @event, CefEventHandle osEvent)
		{
			return false;
		}
	}
}
