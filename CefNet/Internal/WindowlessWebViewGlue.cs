using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CefNet.Internal
{
	public class WindowlessWebViewGlue : WebViewGlue
	{
		public WindowlessWebViewGlue(IChromiumWebViewPrivate view) 
			: base(view)
		{

		}

		/// <summary>
		/// Called to run a file chooser dialog. To display a custom dialog return true and execute |callback| either inline or at a later time. To display the default dialog return false.
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="mode">
		/// Represents the type of dialog to display.
		/// </param>
		/// <param name="title">
		/// The title to be used for the dialog and may be empty to show the default title ("Open" or "Save" depending on the mode).
		/// </param>
		/// <param name="defaultFilePath">
		///  The path with optional directory and/or file name component that should be initially selected in the dialog.
		/// </param>
		/// <param name="acceptFilters">
		/// Used to restrict the selectable file types and may any combination of
		/// (a) valid lower-cased MIME types (e.g. "text/*" or "image/*"),
		/// (b) individual file extensions (e.g. ".txt" or ".png"), or
		/// (c) combined description and file extension delimited using "|" and ";" (e.g. "Image Types|.png;.gif;.jpg").
		/// </param>
		/// <param name="selectedAcceptFilter">
		/// The 0-based index of the filter that should be selected by default.
		/// </param>
		/// <param name="callback">
		/// </param>
		/// <returns></returns>
		internal protected override bool OnFileDialog(CefBrowser browser, CefFileDialogMode mode, string title, string defaultFilePath, CefStringList acceptFilters, bool selectedAcceptFilter, CefFileDialogCallback callback)
		{
			callback.Cancel();
			return true;
		}

		protected internal override bool OnJSDialog(CefBrowser browser, string originUrl, CefJSDialogType dialogType, string messageText, string defaultPromptText, CefJSDialogCallback callback, ref int suppressMessage)
		{
			return base.OnJSDialog(browser, originUrl, dialogType, messageText, defaultPromptText, callback, ref suppressMessage);
		}

		protected internal override bool OnBeforeUnloadDialog(CefBrowser browser, string messageText, bool isReload, CefJSDialogCallback callback)
		{
			return base.OnBeforeUnloadDialog(browser, messageText, isReload, callback);
		}

		protected internal override void OnResetDialogState(CefBrowser browser)
		{
			base.OnResetDialogState(browser);
		}

	}
}
