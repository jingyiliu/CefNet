using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using CefNet.Windows.Forms;

namespace CefNet.Internal
{
	public class WinFormsWebViewGlue : WebViewGlue
	{
		public WinFormsWebViewGlue(IWinFormsWebViewPrivate view)
			: base(view)
		{

		}

		protected new IWinFormsWebViewPrivate WebView
		{
			get { return (IWinFormsWebViewPrivate)base.WebView; }
		}

		protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
		{
			WebView.RaiseCefCursorChange(
				new CursorChangeEventArgs(type != CefCursorType.Custom ? new Cursor(cursorHandle) : CustomCursor.Create(ref customCursorInfo), type)
			);
		}

		protected override bool OnTooltip(CefBrowser browser, ref string text)
		{
			WebView.CefSetToolTip(text);
			return true;
		}

		protected override void OnStatusMessage(CefBrowser browser, string message)
		{
			WebView.CefSetStatusText(message);
		}

	}
}
