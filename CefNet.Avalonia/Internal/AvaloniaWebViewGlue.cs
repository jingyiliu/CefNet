using CefNet.Avalonia;
using System;

namespace CefNet.Internal
{
	public class AvaloniaWebViewGlue : WebViewGlue
	{
		public AvaloniaWebViewGlue(IAvaloniaWebViewPrivate view)
			: base(view)
		{
		}

		public new IAvaloniaWebViewPrivate WebView
		{
			get { return (IAvaloniaWebViewPrivate)base.WebView; }
		}

		protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
		{
			WebView.RaiseCefCursorChange(
				new CursorChangeEventArgs(type != CefCursorType.Custom ? CursorInteropHelper.Create(cursorHandle) : CustomCursor.Create(ref customCursorInfo), type)
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
