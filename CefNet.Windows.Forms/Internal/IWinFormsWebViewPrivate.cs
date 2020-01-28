using CefNet.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	public interface IWinFormsWebViewPrivate: IChromiumWebViewPrivate
	{
		void RaiseCefCursorChange(CursorChangeEventArgs e);
		void CefSetToolTip(string text);
		void CefSetStatusText(string statusText);
		void RaiseStartDragging(StartDraggingEventArgs e);
	}
}
