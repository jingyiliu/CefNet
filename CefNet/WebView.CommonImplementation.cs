//using CefNet.DOM;
using CefNet.Internal;
//using CefNet.JSInterop;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#if WPF
namespace CefNet.Wpf
#elif WINFORMS
namespace CefNet.Windows.Forms
#elif AVALONIA
namespace CefNet.Avalonia
#else
namespace CefNet
#endif
{
#if WINDOWLESS
	partial class WindowlessWebView
#else
	partial class WebView
#endif
		: IChromiumWebView, IChromiumWebViewPrivate
	{
		[Flags]
		private enum State
		{
			NotInitialized = 0,
			Creating = 1 << 0,
			Created = 1 << 1,
			Closing = 1 << 2,
			Closed = 1 << 3,
		}

		private volatile State _state;
		private CefBrowserSettings _browserSettings;
		private WeakReference openerWeakRef;
		private CefMouseEvent _mouseEventProxy = new CefMouseEvent();

		/// <summary>
		/// Occurs before a new browser window is opened.
		/// </summary>
		public event EventHandler<CreateWindowEventArgs> CreateWindow;

		public event EventHandler<BeforeBrowseEventArgs> BeforeBrowse;
		public event EventHandler<BeforeBrowseEventArgs> Navigating;
		public event EventHandler<NavigatedEventArgs> Navigated;
		public event EventHandler<AddressChangeEventArgs> AddressChange;
		public event EventHandler<LoadingStateChangeEventArgs> LoadingStateChange;

		/// <summary>
		/// 
		/// </summary>
		public event EventHandler Closed;
		/// <summary>
		/// Occurs when a browser has recieved a request to close.
		/// </summary>
		public event EventHandler<CancelEventArgs> Closing;

		public event EventHandler<CefPaintEventArgs> CefPaint;
		public event EventHandler BrowserCreated;
		public event EventHandler<DocumentTitleChangedEventArgs> DocumentTitleChanged;
		


		private static CefBrowserSettings _DefaultBrowserSettings;
		
		//private LifeSpanGlue lifeSpanHandler;
		//private CefRequestHandler requestHandler;
		//private CefDisplayHandler _displayHandler;
		//private CefResourceRequestHandler resourceRequestHandler;
		//private CefCookieAccessFilter cookieAccessFilter;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool GetState(State state)
		{
			return _state.HasFlag(state);
		}

		private void SetState(State flag, bool value)
		{
			_state = (value ? (_state | flag) : (_state & ~flag));
		}


		public void Close()
		{
			((IDisposable)this).Dispose();
		}

		protected WebViewGlue ViewGlue { get; private set; }

		/// <summary>
		/// The CefBrowserSettings applied for new instances of WevView.
		/// Any changes to these settings will only apply to new browsers,
		/// leaving already created browsers unaffected.
		/// </summary>
		[Browsable(false)]
		public static CefBrowserSettings DefaultBrowserSettings
		{
			get
			{
				if (_DefaultBrowserSettings == null)
				{
					_DefaultBrowserSettings = new CefBrowserSettings();
				}
				return _DefaultBrowserSettings;
			}
		}

		/// <summary>
		/// The CefBrowserSettings applied for new instances of WevView.
		/// Any changes to these settings will only apply to new browsers,
		/// leaving already created browsers unaffected.
		/// </summary>
		[Browsable(false)]
		public CefBrowserSettings BrowserSettings
		{
			get
			{
				return _browserSettings ?? DefaultBrowserSettings;
				//return GetInitProperty<CefBrowserSettings>(InitialPropertyKeys.BrowserSettings);
			}
			set
			{
				SetInitProperty(InitialPropertyKeys.BrowserSettings, value);
				_browserSettings = value;
			}
		}

		[Browsable(false)]
		public CefRequestContext RequestContext
		{
			get
			{
				if (GetState(State.Created))
					return BrowserObject?.Host.RequestContext;
				return GetInitProperty<CefRequestContext>(InitialPropertyKeys.RequestContext);
			}
			set
			{
				SetInitProperty(InitialPropertyKeys.RequestContext, value);
			}
		}

		public string InitialUrl
		{
			get
			{
				if (GetState(State.Created))
					return "about:blank";
				return GetInitProperty<string>(InitialPropertyKeys.Url);
			}
			set
			{
				SetInitProperty(InitialPropertyKeys.Url, value);
			}
		}

		public CefBrowser BrowserObject
		{
			get
			{
				return ViewGlue?.BrowserObject;
			}
		}

		public CefClient Client
		{
			get
			{
				SetState(State.Creating, true);
				return ViewGlue?.Client;
			}
		}

		protected CefBrowser AliveBrowserObject
		{
			get { return BrowserObject; }
		}

		protected CefBrowserHost AliveBrowserHost
		{
			get { return AliveBrowserObject.Host; }
		}

		[Browsable(false)]
		public IChromiumWebView Opener
		{
			get
			{
				WeakReference weakRef = openerWeakRef;
				if (weakRef != null && weakRef.IsAlive)
				{
					return weakRef.Target as IChromiumWebView;
				}
				return null;
			}
			protected set
			{
				openerWeakRef = value != null ? new WeakReference(value) : null;
			}
		}

		/// <summary>
		/// Gets a value indicating whether a previous page in navigation history is available, which allows the GoBack() method to succeed.
		/// </summary>
		public bool CanGoBack
		{
			get
			{
				return AliveBrowserObject.CanGoBack;
			}
		}

		/// <summary>
		/// Navigates the WebView control to the previous page in the navigation history, if one is available.
		/// </summary>
		/// <returns>
		/// True if the navigation succeeds; false if a previous page in the navigation history is not available.
		/// </returns>
		public bool GoBack()
		{
			CefBrowser browser = AliveBrowserObject;
			if (!browser.CanGoBack)
				return false;
			browser.GoBack();
			GC.KeepAlive(browser);
			return true;
		}

		/// <summary>
		/// Gets a value indicating whether a subsequent page in navigation history is available, which allows the GoForward() method to succeed.
		/// </summary>
		public bool CanGoForward
		{
			get
			{
				return AliveBrowserObject.CanGoForward;
			}
		}

		/// <summary>
		/// Navigates the WebView control to the next page in the navigation history, if one is available.
		/// </summary>
		/// <returns>
		/// True if the navigation succeeds; false if a subsequent page in the navigation history is not available.
		/// </returns>
		public bool GoForward()
		{
			CefBrowser browser = AliveBrowserObject;
			if (!browser.CanGoForward)
				return false;
			browser.GoForward();
			GC.KeepAlive(browser);
			return true;
		}

		/// <summary>
		/// Gets a value indicating whether the WebView control is currently loading.
		/// </summary>
		public bool IsBusy
		{
			get
			{
				return AliveBrowserObject.IsLoading;
			}
		}

		/// <summary>
		/// Reload the current page.
		/// </summary>
		public void Reload()
		{
			AliveBrowserObject.Reload();
		}

		/// <summary>
		/// Reload the current page ignoring any cached data.
		/// </summary>
		public void ReloadIgnoreCache()
		{
			AliveBrowserObject.ReloadIgnoreCache();
		}

		/// <summary>
		/// Stop loading the page.
		/// </summary>
		public void Stop()
		{
			AliveBrowserObject.StopLoad();
		}

		/// <summary>
		/// Get the globally unique identifier for this browser. This value is also
		/// used as the tabId for extension APIs.
		/// </summary>
		public int Identifier
		{
			get { return AliveBrowserObject.Identifier; }
		}

		/// <summary>
		/// Gets a value indicating whether a document has been loaded in the browser.
		/// </summary>
		public bool HasDocument
		{
			get
			{
				return AliveBrowserObject.HasDocument;
			}
		}

		/// <summary>
		/// Returns the main (top-level) frame for the browser window.
		/// </summary>
		public CefFrame GetMainFrame()
		{
			return BrowserObject?.MainFrame;
		}

		/// <summary>
		/// Returns the focused frame for the browser window.
		/// </summary>
		public CefFrame GetFocusedFrame()
		{
			return BrowserObject?.FocusedFrame;
		}

		/// <summary>
		/// Returns the frame with the specified identifier, or null if not found.
		/// </summary>
		public CefFrame GetFrame(long identifier)
		{
			return BrowserObject?.GetFrameByIdent(identifier);
		}

		/// <summary>
		/// Returns the frame with the specified name, or null if not found.
		/// </summary>
		public CefFrame GetFrame(string name)
		{
			return BrowserObject?.GetFrame(name);
		}

		/// <summary>
		/// Returns the number of frames that currently exist.
		/// </summary>
		public int GetFrameCount()
		{
			return (int)AliveBrowserObject.FrameCount;
		}

		/// <summary>
		/// Gets the identifiers of all existing frames.
		/// </summary>
		public long[] GetFrameIdentifiers()
		{
			CefBrowser browser = BrowserObject;
			if (browser == null)
				return new long[0];

			long count = browser.FrameCount << 1;
			var identifiers = new long[count];
			browser.GetFrameIdentifiers(ref count, ref identifiers);
			GC.KeepAlive(browser);
			return identifiers;
		}

		/// <summary>
		/// Gets the names of all existing frames.
		/// </summary>
		public string[] GetFrameNames()
		{
			using (var names = new CefStringList())
			{
				AliveBrowserObject.GetFrameNames(names);
				return names.ToArray();
			}
		}

		/// <summary>
		/// Get and set the current zoom level. The default zoom level is 0.
		/// </summary>
		public double ZoomLevel
		{
			get
			{
				VerifyAccess();
				return AliveBrowserHost.ZoomLevel;
			}
			set
			{
				AliveBrowserHost.ZoomLevel = value;
			}
		}

		/// <summary>
		/// Download the file at |url|.
		/// </summary>
		/// <param name="url">
		/// The URL of the file to load.
		/// </param>
		public void DownloadFile(string url)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			AliveBrowserHost.StartDownload(url);
		}

		/// <summary>
		/// Download |image_url| and execute |callback| on completion with the images  received from
		/// the renderer.  
		/// </summary>
		/// <param name="imageUrl">
		/// </param>
		/// <param name="isFavicon">
		/// If |is_favicon| is True then cookies are not sent and not accepted during download.
		/// </param>
		/// <param name="maxImageSize">
		/// Images with density independent pixel (DIP) sizes larger than |max_image_size| are filtered
		/// out from the image results. Versions of the image at different scale factors may be downloaded
		/// up to the maximum scale factor supported by the system. If there are no image results
		/// &lt; = |max_image_size| then the smallest image is resized to |max_image_size| and is the only
		/// result. A |max_image_size| of 0 means unlimited.
		/// </param>
		/// <param name="bypassCache">
		/// If |bypass_cache| is True then |image_url| is requested from the server even if it is present
		/// in the browser cache.
		/// </param>
		public void DownloadImage(string imageUrl, bool isFavicon, int maxImageSize, bool bypassCache, CefDownloadImageCallback callback)
		{
			if (imageUrl == null)
				throw new ArgumentNullException(nameof(imageUrl));

			AliveBrowserHost.DownloadImage(imageUrl, isFavicon, (uint)maxImageSize, bypassCache, callback);
		}

		/// <summary>
		/// Print the current browser contents.
		/// </summary>
		public void Print()
		{
			AliveBrowserHost.Print();
		}

		/// <summary>
		/// Print the current browser contents to the PDF file and execute |callback| on completion.
		/// </summary>
		/// <param name="path">
		///  The PDF file path.
		/// </param>
		public void PrintToPdf(string path, CefPdfPrintSettings settings, CefPdfPrintCallback callback)
		{
			AliveBrowserHost.PrintToPdf(path, settings, callback);
		}

		/// <summary>
		/// Search for |searchText|. The cef_find_handler_t
		/// instance, if any, returned via cef_client_t::GetFindHandler will be called
		/// to report find results.
		/// </summary>
		/// <param name="identifier">
		/// An unique ID and these IDs must strictly increase so that newer requests always
		/// have greater IDs than older requests. If |identifier| is zero or less than the
		/// previous ID value then it will be automatically assigned a new valid ID.
		/// </param>
		/// <param name="forward">
		/// Indicates whether to search forward or backward within the page.
		/// </param>
		/// <param name="matchCase">
		/// Indicates whether the search should be case-sensitive.
		/// </param>
		/// <param name="findNext">
		/// Indicates whether this is the first request or a follow-up.
		/// </param>
		public Task Find(int identifier, string searchText, bool forward, bool matchCase, bool findNext)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Open developer tools.
		/// </summary>
		public void ShowDevTools()
		{
			ShowDevTools(new CefPoint());
		}

		/// <summary>
		/// Open developer tools (DevTools). If the DevTools is already open then it will be focused.
		/// </summary>
		/// <param name="inspectElementAt">
		/// If |inspect_element_at| is non-empty then the element at the specified (x,y) location will be inspected.
		/// </param>
		public virtual void ShowDevTools(CefPoint inspectElementAt)
		{
			var windowInfo = new CefWindowInfo();
			windowInfo.SetAsPopup(IntPtr.Zero, "DevTools");
			BrowserObject?.Host?.ShowDevTools(windowInfo, null, BrowserSettings, inspectElementAt);
			windowInfo.Dispose();
		}

		/// <summary>
		/// Explicitly close the associated developer tools, if any.
		/// </summary>
		public void CloseDevTools()
		{
			AliveBrowserHost.CloseDevTools();
		}

		public bool MouseCursorChangeDisabled
		{
			get { return AliveBrowserHost.MouseCursorChangeDisabled; }
			set { AliveBrowserHost.MouseCursorChangeDisabled = value; }
		}

		/// <summary>
		/// If a misspelled word is currently selected in an editable node calling this
		/// function will replace it with the specified |word|.
		/// </summary>
		public void ReplaceMisspelling(string word)
		{
			if (word == null)
				throw new ArgumentNullException(nameof(word));
			AliveBrowserHost.ReplaceMisspelling(word);
		}

		/// <summary>
		/// Add the specified |word| to the spelling dictionary.
		/// </summary>
		public void AddWordToDictionary(string word)
		{
			if (word == null)
				throw new ArgumentNullException(nameof(word));
			AliveBrowserHost.AddWordToDictionary(word);
		}

		/// <summary>
		/// Invalidate the view. The browser will call cef_render_handler_t::OnPaint
		/// asynchronously. This function is only used when window rendering is
		/// disabled.
		/// </summary>
		public void Invalidate(CefPaintElementType type)
		{
			AliveBrowserHost.Invalidate(type);
		}

		/// <summary>
		/// Gets and sets the maximum rate in frames per second (fps) that CefPaint will be occurred
		/// for a windowless browser. The actual fps may be lower if the browser cannot generate
		/// frames at the requested rate. The minimum value is 1 and the maximum value is 60
		/// (default 30).
		/// </summary>
		public int WindowlessFrameRate
		{
			get
			{
				VerifyAccess();
				return AliveBrowserHost.WindowlessFrameRate;
			}
			set
			{
				if (value < 1 || value > 60)
					throw new ArgumentOutOfRangeException(nameof(value));
				WindowlessFrameRate = value;
			}
		}

		/// <summary>
		/// Gets and sets a value indicating whether the browser&apos;s audio is muted.
		/// </summary>
		public bool AudioMuted
		{
			get
			{
				VerifyAccess();
				return AliveBrowserHost.AudioMuted;
			}
			set
			{
				AliveBrowserHost.AudioMuted = value;
			}
		}


		//private ScriptableObjectProvider _provider;

		//private ScriptableObjectProvider Provider
		//{
		//	get
		//	{
		//		if (_provider == null && BrowserObject != null)
		//			_provider = new ScriptableObjectProvider(BrowserObject.GetMainFrame());
		//		return _provider;
		//	}
		//}


		//public Window GetWindow()
		//{
		//	return new Window(Provider.GetGlobal(), Provider);
		//}

		public void NotifyRootMovedOrResized()
		{
			this.BrowserObject?.Host.NotifyScreenInfoChanged();
		}

		public void Navigate(string url)
		{
			AliveBrowserObject.MainFrame.LoadUrl(url);
		}

#if NAVIGATEWITHPARAMS
		public void Navigate(string url, string referrer)
		{
			if (referrer == null || "about:blank".Equals(referrer, StringComparison.OrdinalIgnoreCase))
			{
				Navigate(url);
				return;
			}
			BrowserObject.Host.LoadUrlwithParams(url, referrer, (int)PageTransition.Link, IntPtr.Zero, 0, null);
		}

		public unsafe void NavigateWithParams(string url, string referrer, PageTransition navigationType, PostData postData, string extraHeaders)
		{
			if (postData != null)
			{
				if (navigationType != PageTransition.FormSubmit)
					throw new ArgumentOutOfRangeException(nameof(navigationType));

				if (string.IsNullOrEmpty(extraHeaders))
					extraHeaders = "Content-Type: " + postData.ContentType;
				else
					extraHeaders = "\nContent-Type: " + postData.ContentType;

				fixed (byte* ptr = postData.Content)
				{
					BrowserObject.Host.LoadUrlwithParams(url, referrer, (int)navigationType, new IntPtr(ptr), (ulong)postData.Length, extraHeaders);
				}
				return;
			}

			BrowserObject.Host.LoadUrlwithParams(url, referrer, (int)navigationType, IntPtr.Zero, 0, extraHeaders);
		}

		public void LoadContent(string url, string referrer, string contentType, string content, Encoding encoding)
		{
			LoadContent(url, referrer, contentType, encoding.GetBytes(content));
		}

		public unsafe void LoadContent(string url, string referrer, string contentType, byte[] content)
		{
			var r = new CefRequest();
			r.Url = "http://hello.world";

			var request = new ContentRequest(requestHandler);
			request.ContentType = contentType;
			request.Url = r.Url;
			request.Content = content;
			request.Subscribe();
			r.Dispose();
			//lock (contentHandlers)
			//{
			//	contentHandlers.Add(url, request);
			//}
			BrowserObject.Host.LoadUrlwithParams(url, referrer, (int)PageTransition.Link, IntPtr.Zero, 0, "");
			//BrowserObject.MainFrame.LoadUrl(request.Url);
		}
#endif
		public void Post(string url, string referrer, string contentType, byte[] content)
		{

		}

		void IChromiumWebViewPrivate.RaiseCefBrowserCreated()
		{
			SetState(State.Created, true);
#if USERAGENTOVERRIDE
			OnSetUserAgentInNewTab();
#endif
			RaiseCrossThreadEvent(OnBrowserCreated, EventArgs.Empty, true);
		}

		bool IChromiumWebViewPrivate.RaiseClosing()
		{
			if (GetState(State.Closing))
				return false; // don't stop, in the process of disposing

			var ea = new CancelEventArgs();
			RaiseCrossThreadEvent(OnClosing, ea, true);
			if (!ea.Cancel)
			{
				SetState(State.Closing, true);
				return false;
			}
			return true;
		}

		protected virtual void OnClosing(CancelEventArgs e)
		{
			Closing?.Invoke(this, e);
		}

		void IChromiumWebViewPrivate.RaiseClosed()
		{
			SetState(State.Closed, true);
			RaiseCrossThreadEvent(OnClosed, EventArgs.Empty, false);
		}

		protected virtual void OnClosed(EventArgs e)
		{
			Closed?.Invoke(this, e);
		}

		void IChromiumWebViewPrivate.RaiseCefCreateWindow(CreateWindowEventArgs e)
		{
			RaiseCrossThreadEvent(OnCreateWindow, e, true);
		}

		protected virtual void OnCreateWindow(CreateWindowEventArgs e)
		{
			CreateWindow?.Invoke(this, e);
		}

		void IChromiumWebViewPrivate.RaiseBeforeBrowse(BeforeBrowseEventArgs e)
		{
			RaiseCrossThreadEvent(OnBeforeBrowse, e, true);
		}

		protected virtual void OnBeforeBrowse(BeforeBrowseEventArgs e)
		{
			BeforeBrowse?.Invoke(this, e);

			CefFrame frame = e.Frame;
			if (frame != null && frame.IsMain)
			{
				Navigating?.Invoke(this, e);
			}
		}

		void IChromiumWebViewPrivate.RaiseCefPaint(CefPaintEventArgs e)
		{
			OnCefPaint(e);
		}

		void IChromiumWebViewPrivate.RaisePopupShow(PopupShowEventArgs e)
		{
			RaiseCrossThreadEvent(OnPopupShow, e, false);
		}

		void IChromiumWebViewPrivate.RaiseAddressChange(AddressChangeEventArgs e)
		{
			RaiseCrossThreadEvent(OnAddressChange, e, false);
		}

		protected virtual void OnNavigated(AddressChangeEventArgs e)
		{
			Navigated?.Invoke(this, e);
		}

		protected virtual void OnAddressChange(AddressChangeEventArgs e)
		{
			if (e.IsMainFrame)
			{
				OnNavigated(e);
			}
			AddressChange?.Invoke(this, e);
		}

		void IChromiumWebViewPrivate.RaiseTitleChange(DocumentTitleChangedEventArgs e)
		{
			RaiseCrossThreadEvent(OnDocumentTitleChanged, e, false);
		}

		protected virtual void OnDocumentTitleChanged(DocumentTitleChangedEventArgs e)
		{
			DocumentTitleChanged?.Invoke(this, e);
		}

		void IChromiumWebViewPrivate.RaiseLoadingStateChange(LoadingStateChangeEventArgs e)
		{
			RaiseCrossThreadEvent(OnLoadingStateChange, e, false);
		}

		private void InitMouseEvent(int x, int y, CefEventFlags modifiers)
		{
			CefPoint point = PointToViewport(new CefPoint(x, y));
			_mouseEventProxy.X = point.X;
			_mouseEventProxy.Y = point.Y;
			_mouseEventProxy.Modifiers = (uint)modifiers;
		}

		public void SendMouseMoveEvent(int x, int y, CefEventFlags modifiers)
		{
			InitMouseEvent(x, y, modifiers);
			this.BrowserObject?.Host.SendMouseMoveEvent(_mouseEventProxy, false);
		}

		public void SendMouseLeaveEvent()
		{
			_mouseEventProxy.Modifiers = (uint)CefEventFlags.None;
			this.BrowserObject?.Host.SendMouseMoveEvent(_mouseEventProxy, true);
		}

		public void SendMouseClickEvent(int x, int y, CefMouseButtonType button, bool mouseUp, int clicks, CefEventFlags modifiers)
		{
			CefBrowserHost browserHost = this.BrowserObject?.Host;
			if (browserHost == null)
				return;

			InitMouseEvent(x, y, modifiers);
			browserHost.SendFocusEvent(true);
			browserHost.SendMouseClickEvent(_mouseEventProxy, button, mouseUp, clicks);
		}

		public void SendMouseWheelEvent(int x, int y, int deltaX, int deltaY)
		{
			CefBrowserHost browserHost = this.BrowserObject?.Host;
			if (browserHost == null)
				return;

			InitMouseEvent(x, y, CefEventFlags.None);
			browserHost.SendMouseWheelEvent(_mouseEventProxy, deltaX, deltaY);
		}


#if USERAGENTOVERRIDE

		protected virtual void OnSetUserAgentInNewTab()
		{
			string useragent = Opener?.GetUserAgentOverride();
			if (!string.IsNullOrWhiteSpace(useragent))
				SetUserAgentOverride(useragent);
		}

		public virtual void SetUserAgentOverride(string useragent)
		{
			BrowserObject?.Host.SetUserAgentOverride(useragent, true);
		}

		public string GetUserAgentOverride()
		{
			return BrowserObject?.Host.GetUserAgentOverride();
		}

#endif //USERAGENTOVERRIDE

	}
}
