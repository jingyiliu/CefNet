using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	partial class WebViewGlue
	{
		internal CefContextMenuHandler GetContextMenuHandler()
		{
			return ContextMenuGlue;
		}

		internal CefDialogHandler GetDialogHandler()
		{
			return DialogGlue;
		}

		internal CefDisplayHandler GetDisplayHandler()
		{
			return DisplayGlue;
		}

		internal CefDownloadHandler GetDownloadHandler()
		{
			return null;
		}

		internal CefDragHandler GetDragHandler()
		{
			return null;
		}

		internal CefFindHandler GetFindHandler()
		{
			return null;
		}

		internal CefFocusHandler GetFocusHandler()
		{
			return FocusGlue;
		}

		internal CefJSDialogHandler GetJSDialogHandler()
		{
			return null;
		}

		internal CefKeyboardHandler GetKeyboardHandler()
		{
			return null;
		}

		internal CefLifeSpanHandler GetLifeSpanHandler()
		{
			return LifeSpanGlue;
		}

		internal CefLoadHandler GetLoadHandler()
		{
			return LoadGlue;
		}

		internal CefRenderHandler GetRenderHandler()
		{
			return RenderGlue;
		}

		internal CefRequestHandler GetRequestHandler()
		{
			return RequestGlue;
		}

		internal bool AvoidOnProcessMessageReceived()
		{
			return false;
		}

		protected internal virtual bool OnProcessMessageReceived(CefBrowser browser, CefFrame frame, CefProcessId sourceProcess, CefProcessMessage message)
		{
			var ea = new CefProcessMessageReceivedEventArgs(browser, frame, sourceProcess, message);
			CefNetApplication.Instance.OnCefProcessMessageReceived(ea);
			return ea.Handled;
		}
	}
}
