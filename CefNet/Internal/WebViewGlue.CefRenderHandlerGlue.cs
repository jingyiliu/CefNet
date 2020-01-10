using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet.Internal
{
	public partial class WebViewGlue
	{

		internal CefAccessibilityHandler GetAccessibilityHandler()
		{
			return null;
		}

		internal bool AvoidGetRootScreenRect()
		{
			return false;
		}

		internal protected virtual bool GetRootScreenRect(CefBrowser browser, ref CefRect rect)
		{
			rect = WebView.GetCefRootBounds();
			return !rect.IsNullSize;
		}

		internal bool AvoidGetViewRect()
		{
			return false;
		}

		internal protected virtual void GetViewRect(CefBrowser browser, ref CefRect rect)
		{
			rect = WebView.GetCefViewBounds();
		}

		internal bool AvoidGetScreenPoint()
		{
			return false;
		}

		internal protected virtual bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
		{
			var point = new CefPoint(viewX, viewY);
			if (WebView.CefPointToScreen(ref point))
			{
				screenX = point.X;
				screenY = point.Y;
				return true;
			}
			return false;
		}

		internal bool AvoidGetScreenInfo()
		{
			return false;
		}

		internal protected virtual bool GetScreenInfo(CefBrowser browser, ref CefScreenInfo screenInfo)
		{
			if (WebView.GetCefScreenInfo(ref screenInfo))
				return true;

			if (!PlatformInfo.IsWindows)
				return false;

			//IntPtr hDesktop = WindowsNativeMethods.GetDesktopWindow();
			IntPtr hMonitor = NativeMethods.MonitorFromWindow(IntPtr.Zero, MonitorFlag.MONITOR_DEFAULTTOPRIMARY);

			var monitorInfo = new MONITORINFO();
			monitorInfo.Size = Marshal.SizeOf(typeof(MONITORINFO));
			NativeMethods.GetMonitorInfo(hMonitor, ref monitorInfo);
			screenInfo.Depth = 24;
			screenInfo.DepthPerComponent = 24;
			screenInfo.Rect = monitorInfo.Monitor.ToCefRect();
			screenInfo.AvailableRect = monitorInfo.Work.ToCefRect();
			screenInfo.DeviceScaleFactor = WebView.GetDevicePixelRatio();
			return true;
		}

		internal bool AvoidOnPopupShow()
		{
			return false;
		}

		internal protected virtual void OnPopupShow(CefBrowser browser, bool show)
		{
			if (!show) WebView.RaisePopupShow(new PopupShowEventArgs());
		}

		internal bool AvoidOnPopupSize()
		{
			return false;
		}

		internal protected virtual void OnPopupSize(CefBrowser browser, CefRect rect)
		{
			WebView.RaisePopupShow(new PopupShowEventArgs(rect));
		}

		internal bool AvoidOnPaint()
		{
			return false;
		}

		internal protected virtual void OnPaint(CefBrowser browser, CefPaintElementType type, CefRect[] dirtyRects, IntPtr buffer, int width, int height)
		{
			WebView.RaiseCefPaint(new CefPaintEventArgs(browser, type, dirtyRects, buffer, width, height));
		}

		internal bool AvoidOnAcceleratedPaint()
		{
			return false;
		}

		internal protected virtual void OnAcceleratedPaint(CefBrowser browser, CefPaintElementType type, CefRect[] dirtyRects, IntPtr sharedHandle)
		{

		}

		internal bool AvoidOnCursorChange()
		{
			return false;
		}

		internal protected virtual void OnCursorChange(CefBrowser browser, IntPtr cursor, CefCursorType type, CefCursorInfo customCursorInfo)
		{

		}

		internal bool AvoidStartDragging()
		{
			return false;
		}

		internal protected virtual bool StartDragging(CefBrowser browser, CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
		{
			return false;
		}

		internal bool AvoidUpdateDragCursor()
		{
			return false;
		}

		internal protected virtual void UpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation)
		{

		}

		internal bool AvoidOnScrollOffsetChanged()
		{
			return false;
		}

		internal protected virtual void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
		{

		}

		internal bool AvoidOnImeCompositionRangeChanged()
		{
			return false;
		}

		internal protected virtual void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRect[] characterBounds)
		{

		}

		internal bool AvoidOnTextSelectionChanged()
		{
			return false;
		}

		internal protected virtual void OnTextSelectionChanged(CefBrowser browser, string selectedText, CefRange selectedRange)
		{

		}

		internal bool AvoidOnVirtualKeyboardRequested()
		{
			return false;
		}

		internal protected virtual void OnVirtualKeyboardRequested(CefBrowser browser, CefTextInputMode inputMode)
		{

		}
	}
}
