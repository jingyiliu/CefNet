using CefNet.Wpf;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Interop;

namespace CefNet.Internal
{
	public class WpfWebViewGlue : WebViewGlue
	{
		public WpfWebViewGlue(IWpfWebViewPrivate view)
			: base(view)
		{
		}

		public new IWpfWebViewPrivate WebView
		{
			get { return (IWpfWebViewPrivate)base.WebView; }
		}

		protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
		{
			WebView.RaiseCefCursorChange(
				new CursorChangeEventArgs(type != CefCursorType.Custom ? CursorInteropHelper.Create(new SafeFileHandle(cursorHandle, false)) : CustomCursor.Create(ref customCursorInfo), type)
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
