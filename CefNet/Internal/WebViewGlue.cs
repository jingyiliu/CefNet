using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	public partial class WebViewGlue
	{
		private bool _isResourceRequestGlueInitialized;
		private CefResourceRequestHandler _resourceRequestGlue;
		private bool _partialAvoidResourceRequestGlue;

		private bool _isCookieAccessFilterGlueInitialized;
		private CefCookieAccessFilter _cookieAccessFilterGlue;

		private bool _isFocusGlueInitialized;
		private CefFocusHandlerGlue _focusGlue;

		private bool _isDragGlueInitialized;
		private CefDragHandlerGlue _dragGlue;

		private bool _isKeyboardGlueInitialized;
		private CefKeyboardHandlerGlue _keyboardGlue;

		private bool _isJSDialogGlueInitialized;
		private CefJSDialogHandlerGlue _jsDialogGlue;
		private bool _avoidJsDialogGlue;


		protected IChromiumWebViewPrivate WebView { get; private set; }

		public CefBrowser BrowserObject { get; protected set; }

		public CefClient Client { get; private set; }
		private CefLifeSpanHandlerGlue LifeSpanGlue { get; }
		private CefRenderHandlerGlue RenderGlue { get; }
		private CefDisplayHandlerGlue DisplayGlue { get; }
		private CefRequestHandlerGlue RequestGlue { get; }
		private CefDialogHandlerGlue DialogGlue { get; }
		private CefDownloadHandlerGlue DownloadGlue { get; }
		private CefFindHandlerGlue FindGlue { get; }

		private CefContextMenuHandlerGlue ContextMenuGlue { get; }
		private CefLoadHandlerGlue LoadGlue { get; }

		public WebViewGlue(IChromiumWebViewPrivate view)
		{
			this.WebView = view;
			this.Client = new CefClientGlue(this);
			this.LifeSpanGlue = new CefLifeSpanHandlerGlue(this);
			this.RenderGlue = new CefRenderHandlerGlue(this);
			this.DisplayGlue = new CefDisplayHandlerGlue(this);
			this.RequestGlue = new CefRequestHandlerGlue(this);
			this.DialogGlue = new CefDialogHandlerGlue(this);
			this.DownloadGlue = new CefDownloadHandlerGlue(this);
			this.FindGlue = new CefFindHandlerGlue(this);

			this.ContextMenuGlue = new CefContextMenuHandlerGlue(this);
			this.LoadGlue = new CefLoadHandlerGlue(this);

			_partialAvoidResourceRequestGlue = AvoidOverloadOnBeforeResourceLoad() && AvoidGetResourceHandler()
				&& AvoidOnResourceRedirect() && AvoidOnResourceResponse() && AvoidGetResourceResponseFilter()
				&& AvoidOnResourceLoadComplete() && AvoidOnProtocolExecution();

			_avoidJsDialogGlue = AvoidOnJSDialog() && AvoidOnBeforeUnloadDialog() && AvoidOnResetDialogState()
				&& AvoidOnDialogClosed();
		}


		private CefResourceRequestHandler ResourceRequestGlue
		{
			get
			{
				if (_isResourceRequestGlueInitialized)
					return _resourceRequestGlue;

				if (_partialAvoidResourceRequestGlue && AvoidGetCookieAccessFilter())
				{
					_resourceRequestGlue = null;
				}
				else
				{
					_resourceRequestGlue = new CefResourceRequestHandlerGlue(this);
				}

				_isResourceRequestGlueInitialized = true;
				return _resourceRequestGlue;
			}
		}

		private CefCookieAccessFilter CookieAccessFilterGlue
		{
			get
			{
				if (_isCookieAccessFilterGlueInitialized)
					return _cookieAccessFilterGlue;

				if (AvoidOverloadCanSendCookie() && AvoidOverloadCanSaveCookie())
				{
					_cookieAccessFilterGlue = null;
				}
				else
				{
					_cookieAccessFilterGlue = new CefCookieAccessFilterGlue(this);
				}

				_isCookieAccessFilterGlueInitialized = true;
				return _cookieAccessFilterGlue;
			}
		}

		private CefFocusHandlerGlue FocusGlue
		{
			get
			{
				if (_isFocusGlueInitialized)
					return _focusGlue;

				if (AvoidOnTakeFocus()
					&& AvoidOnSetFocus()
					&& AvoidOnGotFocus())
				{
					_focusGlue = null;
				}
				else
				{
					_focusGlue = new CefFocusHandlerGlue(this);
				}

				_isFocusGlueInitialized = true;
				return _focusGlue;
			}
		}

		private CefDragHandlerGlue DragGlue
		{
			get
			{
				if (_isDragGlueInitialized)
					return _dragGlue;

				if (AvoidOnDragEnter() && AvoidOnDraggableRegionsChanged())
				{
					_dragGlue = null;
				}
				else
				{
					_dragGlue = new CefDragHandlerGlue(this);
				}

				_isDragGlueInitialized = true;
				return _dragGlue;
			}
		}

		private CefKeyboardHandlerGlue KeyboardGlue
		{
			get
			{
				if (_isKeyboardGlueInitialized)
					return _keyboardGlue;

				if (AvoidOnPreKeyEvent() && AvoidOnKeyEvent())
				{
					_keyboardGlue = null;
				}
				else
				{
					_keyboardGlue = new CefKeyboardHandlerGlue(this);
				}

				_isKeyboardGlueInitialized = true;
				return _keyboardGlue;
			}
		}

		private CefJSDialogHandlerGlue JSDialogGlue
		{
			get
			{
				if (_isJSDialogGlueInitialized)
					return _jsDialogGlue;

				if (_avoidJsDialogGlue)
				{
					_jsDialogGlue = null;
				}
				else
				{
					_jsDialogGlue = new CefJSDialogHandlerGlue(this);
				}

				_isJSDialogGlueInitialized = true;
				return _jsDialogGlue;
			}
		}


		internal void NotifyPopupBrowserCreating()
		{
			WebView.RaisePopupBrowserCreating();
		}

	}
}
