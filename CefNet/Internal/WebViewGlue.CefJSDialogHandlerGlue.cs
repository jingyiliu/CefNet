using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace CefNet.Internal
{
	public partial class WebViewGlue
	{
		[MethodImpl(MethodImplOptions.ForwardRef)]
		internal extern bool AvoidOnJSDialog();

		/// <summary>
		/// Called to run a JavaScript dialog. Return true if the application will use a custom dialog or
		/// if the callback has been executed immediately. Custom dialogs may be either modal or modeless.
		/// If a custom dialog is used the application must execute <paramref name="callback"/> once the
		/// custom dialog is dismissed.
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="originUrl">
		/// If <paramref name="originUrl"/> is non-empty it can be passed to the CefFormatUrlForSecurityDisplay
		/// function to retrieve a secure and user-friendly display string.
		/// </param>
		/// <param name="dialogType"></param>
		/// <param name="messageText"></param>
		/// <param name="defaultPromptText">
		/// The <paramref name="defaultPromptText"/> value will be specified for prompt dialogs only.
		/// </param>
		/// <param name="callback"></param>
		/// <param name="suppressMessage">
		/// Set <paramref name="suppressMessage"/> to 1 and return false to suppress the message (suppressing messages
		/// is preferable to immediately executing the callback as this is used to detect presumably malicious behavior
		/// like spamming alert messages in onbeforeunload). Set <paramref name="suppressMessage"/> to 0 and return false
		/// to use the default implementation (the default implementation will show one modal dialog at a time and suppress
		/// any additional dialog request until the displayed dialog is dismissed).
		/// </param>
		/// <returns></returns>
		internal protected virtual bool OnJSDialog(CefBrowser browser, string originUrl, CefJSDialogType dialogType, string messageText, string defaultPromptText, CefJSDialogCallback callback, ref int suppressMessage)
		{
			return false;
		}

		[MethodImpl(MethodImplOptions.ForwardRef)]
		internal extern bool AvoidOnBeforeUnloadDialog();

		/// <summary>
		/// Called to run a dialog asking the user if they want to leave a page. Return false to use the default dialog
		/// implementation. Return true if the application will use a custom dialog or if the callback has been executed
		/// immediately. Custom dialogs may be either modal or modeless. If a custom dialog is used the application must
		/// execute <paramref name="callback"/> once the custom dialog is dismissed.
		/// </summary>
		/// <param name="browser"></param>
		/// <param name="messageText"></param>
		/// <param name="isReload"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		internal protected virtual bool OnBeforeUnloadDialog(CefBrowser browser, string messageText, bool isReload, CefJSDialogCallback callback)
		{
			return false;
		}

		[MethodImpl(MethodImplOptions.ForwardRef)]
		internal extern bool AvoidOnResetDialogState();

		/// <summary>
		/// Called to cancel any pending dialogs and reset any saved dialog state. Will be called due to events like
		/// page navigation irregardless of whether any dialogs are currently pending.
		/// </summary>
		/// <param name="browser"></param>
		internal protected virtual void OnResetDialogState(CefBrowser browser)
		{

		}

		[MethodImpl(MethodImplOptions.ForwardRef)]
		internal extern bool AvoidOnDialogClosed();

		/// <summary>
		/// Called when the default implementation dialog is closed.
		/// </summary>
		/// <param name="browser"></param>
		internal protected virtual void OnDialogClosed(CefBrowser browser)
		{

		}
	}
}
