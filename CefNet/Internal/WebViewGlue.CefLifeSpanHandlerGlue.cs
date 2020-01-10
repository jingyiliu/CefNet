using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	public partial class WebViewGlue
	{
		internal bool AvoidOnBeforePopup()
		{
			return false;
		}

		internal protected virtual bool OnBeforePopup(CefBrowser browser, CefFrame frame, string targetUrl, string targetFrameName, CefWindowOpenDisposition targetDisposition,
			bool userGesture, CefPopupFeatures popupFeatures, CefWindowInfo windowInfo, ref CefClient client, CefBrowserSettings settings, ref CefDictionaryValue extraInfo, ref int noJavascriptAccess)
		{
#if DEBUG
			if (!BrowserObject.IsSame(browser))
				throw new InvalidOperationException();
#endif
			var ea = new CreateWindowEventArgs(frame, targetUrl, targetFrameName, targetDisposition, userGesture, popupFeatures, windowInfo, client, settings, extraInfo, noJavascriptAccess != 0);
			WebView.RaiseCefCreateWindow(ea);
			extraInfo = ea.ExtraInfo;
			noJavascriptAccess = ea.NoJavaScriptAccess ? 1 : 0;
			client = ea.Client;
			if (!ea.Cancel)
			{
				(client as CefClientGlue)?.NotifyPopupBrowserCreating();
			}
			return ea.Cancel;
		}

		internal bool AvoidOnAfterCreated()
		{
			return false;
		}

		internal protected virtual void OnAfterCreated(CefBrowser browser)
		{
			this.BrowserObject = browser;
			WebView.RaiseCefBrowserCreated();
		}

		internal bool AvoidDoClose()
		{
			return false;
		}

		internal protected virtual bool DoClose(CefBrowser browser)
		{
#if DEBUG
			if (!BrowserObject.IsSame(browser))
				throw new InvalidOperationException();
#endif
			return WebView.RaiseClosing();
		}

		internal bool AvoidOnBeforeClose()
		{
			return false;
		}

		internal protected virtual void OnBeforeClose(CefBrowser browser)
		{
#if DEBUG
			if (!BrowserObject.IsSame(browser))
				throw new InvalidOperationException();
#endif
			try
			{
				WebView.RaiseClosed();
			}
			finally
			{
				this.BrowserObject = null;
			}
		}
	}
}
