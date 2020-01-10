using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	public partial class WebViewGlue
	{
		internal bool AvoidOnDragEnter()
		{
			return false;
		}

		internal protected virtual bool OnDragEnter(CefBrowser browser, CefDragData dragData, CefDragOperationsMask mask)
		{
			return false;
		}

		internal bool AvoidOnDraggableRegionsChanged()
		{
			return false;
		}

		internal protected virtual void OnDraggableRegionsChanged(CefBrowser browser, CefFrame frame, CefDraggableRegion[] regions)
		{

		}
	}
}
