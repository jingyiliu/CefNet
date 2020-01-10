using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	public partial class WebViewGlue
	{
		internal bool AvoidOnBeforeDownload()
		{
			return false;
		}

		internal protected virtual void OnBeforeDownload(CefBrowser browser, CefDownloadItem downloadItem, string suggestedName, CefBeforeDownloadCallback callback)
		{

		}

		internal bool AvoidOnDownloadUpdated()
		{
			return false;
		}

		internal protected virtual void OnDownloadUpdated(CefBrowser browser, CefDownloadItem downloadItem, CefDownloadItemCallback callback)
		{

		}
	}
}
