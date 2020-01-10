using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	public partial class WebViewGlue
	{
		internal bool AvoidOnLoadingStateChange()
		{
			return false;
		}

		internal protected virtual void OnLoadingStateChange(CefBrowser browser, bool isLoading, bool canGoBack, bool canGoForward)
		{
#if DEBUG
			if (!BrowserObject.IsSame(browser))
				throw new InvalidOperationException();
#endif
			WebView.RaiseLoadingStateChange(new LoadingStateChangeEventArgs(isLoading, canGoBack, canGoForward));
		}

		internal bool AvoidOnLoadStart()
		{
			return false;
		}

		internal protected virtual void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
		{
#if DEBUG
			if (!BrowserObject.IsSame(browser))
				throw new InvalidOperationException();
#endif
		}

		internal bool AvoidOnLoadEnd()
		{
			return false;
		}

		internal protected virtual void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
		{
#if DEBUG
			if (!BrowserObject.IsSame(browser))
				throw new InvalidOperationException();
#endif
		}

		internal bool AvoidOnLoadError()
		{
			return false;
		}

		internal protected virtual void OnLoadError(CefBrowser browser, CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
		{
#if DEBUG
			if (!BrowserObject.IsSame(browser))
				throw new InvalidOperationException();
#endif
		}
	}
}
