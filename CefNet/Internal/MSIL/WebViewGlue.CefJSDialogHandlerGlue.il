.class public auto ansi beforefieldinit CefNet.Internal.WebViewGlue
{
	.method assembly hidebysig instance bool AvoidOnJSDialog()
	{
		.maxstack 2
		ldarg.0
		ldvirtftn instance bool CefNet.Internal.WebViewGlue::OnJsdialog(class CefNet.CefBrowser, string, valuetype CefNet.CefJSDialogType, string, string, class CefNet.CefJSDialogCallback, int32&)
		ldftn instance bool CefNet.Internal.WebViewGlue::OnJsdialog(class CefNet.CefBrowser, string, valuetype CefNet.CefJSDialogType, string, string, class CefNet.CefJSDialogCallback, int32&)
		ceq
		ret
	}

	.method assembly hidebysig instance bool AvoidOnBeforeUnloadDialog()
	{
		.maxstack 2
		ldarg.0
		ldvirtftn instance bool CefNet.Internal.WebViewGlue::OnBeforeUnloadDialog(class CefNet.CefBrowser, string, bool, class CefNet.CefJSDialogCallback)
		ldftn instance bool CefNet.Internal.WebViewGlue::OnBeforeUnloadDialog(class CefNet.CefBrowser, string, bool, class CefNet.CefJSDialogCallback)
		ceq
		ret
	}

	.method assembly hidebysig instance bool AvoidOnResetDialogState()
	{
		.maxstack 2
		ldarg.0
		ldvirtftn instance void CefNet.Internal.WebViewGlue::OnResetDialogState(class CefNet.CefBrowser)
		ldftn instance void CefNet.Internal.WebViewGlue::OnResetDialogState(class CefNet.CefBrowser)
		ceq
		ret
	}

	.method assembly hidebysig instance bool AvoidOnDialogClosed()
	{
		.maxstack 2
		ldarg.0
		ldvirtftn instance void CefNet.Internal.WebViewGlue::OnDialogClosed(class CefNet.CefBrowser)
		ldftn instance void CefNet.Internal.WebViewGlue::OnDialogClosed(class CefNet.CefBrowser)
		ceq
		ret
	}

}