using Avalonia.Input;
using CefNet.Avalonia;
using System;
using System.Diagnostics;

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

		/// <summary>
		/// Called when the user starts dragging content in the web view. OS APIs that run a system message
		/// loop may be used within the StartDragging call. Don't call any of CefBrowserHost::DragSource*Ended*
		/// methods after returning false. Call CefBrowserHost::DragSourceEndedAt and DragSourceSystemDragEnded
		/// either synchronously or asynchronously to inform the web view that the drag operation has ended.
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="dragData">The contextual information about the dragged content.</param>
		/// <param name="allowedOps"></param>
		/// <param name="x">The X-location in screen coordinates.</param>
		/// <param name="y">The Y-location in screen coordinates.</param>
		/// <returns>Return false to abort the drag operation or true to handle the drag operation.</returns>
		protected override bool StartDragging(CefBrowser browser, CefDragData dragData, CefDragOperationsMask allowedOps, int x, int y)
		{
			var e = new StartDraggingEventArgs(dragData, allowedOps, x, y);
			WebView.RaiseStartDragging(e);
			return e.Handled;
		}

		/// <summary>
		/// Called when the web view wants to update the mouse cursor during a drag &amp; drop operation.
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="operation">Describes the allowed operation (none, move, copy, link).</param>
		protected override void UpdateDragCursor(CefBrowser browser, CefDragOperationsMask operation)
		{
			Cursor cursor;
			CefCursorType cursorType;
			switch(operation)
			{
				case CefDragOperationsMask.None:
					cursor = CursorInteropHelper.Create(StandardCursorType.No);
					cursorType = CefCursorType.Notallowed;
					break;
				case CefDragOperationsMask.Move:
					cursor = CursorInteropHelper.Create(StandardCursorType.DragMove);
					cursorType = CefCursorType.Move;
					break;
				case CefDragOperationsMask.Link:
					cursor = CursorInteropHelper.Create(StandardCursorType.DragLink);
					cursorType = CefCursorType.Alias;
					break;
				default:
					cursor = CursorInteropHelper.Create(StandardCursorType.DragCopy);
					cursorType = CefCursorType.Copy;
					break;
			}
			WebView.RaiseCefCursorChange(new CursorChangeEventArgs(cursor, cursorType));
		}

		protected override void OnFindResult(CefBrowser browser, int identifier, int count, CefRect selectionRect, int activeMatchOrdinal, bool finalUpdate)
		{
			WebView.RaiseTextFound(new TextFoundRoutedEventArgs(identifier, count, selectionRect, activeMatchOrdinal, finalUpdate));
		}

	}
}
