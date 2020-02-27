//using CefNet.DOM;
using CefNet.Internal;
//using CefNet.JSInterop;


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefNet
{
	public partial class WindowlessWebView : IDisposable
	{
		private object SyncRoot = new object();

		private bool _layoutOff;
		private CefRect _bounds;
		private float _devicePixelRatio = 1;
		private Thread _uiThread;

		private EventHandler<ITextFoundEventArgs> TextFoundEvent;
		private EventHandler<IPdfPrintFinishedEventArgs> PdfPrintFinishedEvent;

		public event EventHandler PopupShow;

		public WindowlessWebView()
			: this(null, null, null, null)
		{

		}

		public WindowlessWebView(WindowlessWebView opener)
		{
			if (opener != null)
			{
				this.Opener = opener;
				this.BrowserSettings = opener.BrowserSettings;
			}

			using (var windowInfo = new CefWindowInfo())
			{
				Initialize(windowInfo);
			}
		}

		public WindowlessWebView(string url, CefBrowserSettings settings, CefDictionaryValue extraInfo, CefRequestContext requestContext)
		{
			CefWindowInfo windowInfo = null;
			try
			{
				windowInfo = new CefWindowInfo();
				Initialize(windowInfo);
				if (!CefApi.CreateBrowser(windowInfo, ViewGlue.Client, url ?? "about:blank", settings ?? DefaultBrowserSettings, extraInfo, requestContext))
					throw new InvalidOperationException();
			}
			finally
			{
				windowInfo?.Dispose();
			}
		}

		~WindowlessWebView()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.ViewGlue != null)
			{
				BrowserObject?.Host.CloseBrowser(true);
				this.ViewGlue = null;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private void SetInitProperty(InitialPropertyKeys key, object value)
		{
			throw new InvalidOperationException("This property must be set before the underlying CEF browser is created.");
		}

		private T GetInitProperty<T>(InitialPropertyKeys key)
		{
			return default;
		}

		private void InitializeInternal(CefWindowInfo windowInfo)
		{
			_uiThread = Thread.CurrentThread;
			this.ViewGlue = CreateWebViewGlue();
			Initialize(windowInfo);
		}

		protected virtual void Initialize(CefWindowInfo windowInfo)
		{
			windowInfo.SetAsWindowless(IntPtr.Zero);
			Width = Math.Max(200, windowInfo.Width);
			Height = Math.Max(100, windowInfo.Height);
		}

		protected bool InvokeRequired
		{
			get { return Thread.CurrentThread != _uiThread; }
		}

		protected void VerifyAccess()
		{
			if (InvokeRequired)
				throw new InvalidOperationException("Cross-thread operation not valid. The WebView accessed from a thread other than the thread it was created on.");
		}

		private WindowlessWebViewGlue WindowlessViewGlue
		{
			get { return (WindowlessWebViewGlue)ViewGlue; }
		}

		protected virtual WindowlessWebViewGlue CreateWebViewGlue()
		{
			return new WindowlessWebViewGlue(this);
		}

		protected virtual void RaiseCrossThreadEvent<TEventArgs>(Action<TEventArgs> raiseEvent, TEventArgs e, bool synchronous)
		{
			raiseEvent(e);
		}

		private void AddHandler<TEventHadler>(in TEventHadler eventKey, TEventHadler handler)
			where TEventHadler : Delegate
		{
			TEventHadler current;
			TEventHadler key = eventKey;
			do
			{
				current = key;
				key = CefNetApi.CompareExchange<TEventHadler>(in eventKey, (TEventHadler)Delegate.Combine(current, handler), current);
			}
			while (key != current);
		}

		private void RemoveHandler<TEventHadler>(in TEventHadler eventKey, TEventHadler handler)
			where TEventHadler : Delegate
		{
			TEventHadler current;
			TEventHadler key = eventKey;
			do
			{
				current = key;
				key = CefNetApi.CompareExchange<TEventHadler>(in eventKey, (TEventHadler)Delegate.Combine(current, handler), current);
			}
			while (key != current);
		}

		float IChromiumWebViewPrivate.GetDevicePixelRatio()
		{
			return 1;
		}

		CefRect IChromiumWebViewPrivate.GetCefRootBounds()
		{
			return _bounds;
		}

		CefRect IChromiumWebViewPrivate.GetCefViewBounds()
		{
			return _bounds;
		}

		bool IChromiumWebViewPrivate.GetCefScreenInfo(ref CefScreenInfo screenInfo)
		{
			return GetCefScreenInfo(ref screenInfo);
		}

		protected virtual bool GetCefScreenInfo(ref CefScreenInfo screenInfo)
		{
			return false;
		}

		bool IChromiumWebViewPrivate.CefPointToScreen(ref CefPoint point)
		{
			return false;
		}

		void IChromiumWebViewPrivate.RaisePopupBrowserCreating()
		{
			SetState(State.Creating, true);
			SyncRoot = new object();
		}

		protected virtual void OnBrowserCreated(EventArgs e)
		{
			BrowserCreated?.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="CefPaint"/> event.
		/// </summary>
		/// <param name="e">A <see cref="CefPaintEventArgs"/> that contains the event data.</param>
		/// <remarks>This method can be called on a thread other than the UI thread.</remarks>
		protected virtual void OnCefPaint(CefPaintEventArgs e)
		{
			CefPaint?.Invoke(this, e);
		}

		protected virtual void OnPopupShow(PopupShowEventArgs e)
		{
			PopupShow?.Invoke(this, e);
		}

		bool IChromiumWebViewPrivate.RaiseRunContextMenu(CefFrame frame, CefContextMenuParams menuParams, CefMenuModel model, CefRunContextMenuCallback callback)
		{
			callback.Cancel();
			return true;
		}

		protected virtual void OnLoadingStateChange(LoadingStateChangeEventArgs e)
		{
			LoadingStateChange?.Invoke(this, e);
		}

		//#region RenderHandler

		////protected virtual void GetViewRect(object sender, CfxGetViewRectEventArgs e)
		////{
		////	CfxRect r = e.Rect;
		////	r.X = X;
		////	r.Y = Y;
		////	r.Width = _rlWidth > 0 ? _rlWidth : Width;
		////	r.Height = _rlHeight > 0 ? _rlHeight : Height;
		////	System.Diagnostics.Debug.WriteLine("w = {0}; h = {1}", r.Width, r.Height);
		////}

		//private int _rlWidth;
		//private int _rlHeight;


		//public void SetRenderLayoutSize(int width, int height)
		//{
		//	_rlWidth = width;
		//	_rlHeight =height;
		//	BrowserObject?.GetHost().Invalidate(CefPaintElementType.View);
		//}

		//private TaskCompletionSource<bool> _screenshotTask;
		//public async Task CreateScreenshot(int width, int height)
		//{
		//	var tcs = new TaskCompletionSource<bool>();
		//	if (Interlocked.CompareExchange(ref _screenshotTask, tcs, default) != null)
		//		throw new InvalidOperationException();

		//	SetRenderLayoutSize(width, height);
		//	await tcs.Task;
		//	Volatile.Write(ref _screenshotTask, null);

		//	//Interlocked.Exchange(ref _screenshotTask, null);
		//	//SetRenderLayoutSize(0, 0);
		//}

		////protected virtual void GetScreenPoint(object sender, CfxGetScreenPointEventArgs e)
		////{
		////	e.ScreenX = e.ViewX + X;
		////	e.ScreenY = e.ViewY + Y;
		////	e.Handled = true;
		////}


		////protected virtual void GetRootScreenRect(object sender, CfxGetRootScreenRectEventArgs e) { }

		//#endregion RenderHandler



		//		internal protected virtual void OnAfterCreated(CefBrowser browser)
		//		{
		//			this.BrowserObject = browser;
		//#if USERAGENTOVERRIDE
		//			OnSetUserAgentInNewTab();
		//#endif
		//		}

		protected virtual CefPoint PointToViewport(CefPoint point)
		{
			return point;
		}

		public float DevicePixelRatio
		{
			get
			{
				return _devicePixelRatio;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value));
				_devicePixelRatio = value;
			}
		}

		public int X
		{
			get { return _bounds.X; }
			set { _bounds.X = value; }
		}

		public int Y
		{
			get { return _bounds.Y; }
			set { _bounds.Y = value; }
		}

		public int Width
		{
			get
			{
				return _bounds.Width;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value));
				_bounds.Width = value;
				PerformLayout(false);
			}
		}
		
		public int Height
		{
			get
			{
				return _bounds.Height;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException(nameof(value));
				_bounds.Height = value;
				PerformLayout(false);
			}
		}

		public void SuspendLayout()
		{
			_layoutOff = true;
		}

		public void ResumeLayout()
		{
			_layoutOff = false;
			PerformLayout(true);
		}

		public void ResumeLayout(bool performLayout)
		{
			_layoutOff = false;
			PerformLayout(performLayout);
		}

		private void PerformLayout(bool immediate)
		{
			if (!_layoutOff || immediate)
			{
				ViewGlue.BrowserObject?.Host.WasResized();
			}
		}

		public void Invalidate()
		{
			PerformLayout(true);
		}

		public void SetFocus(bool focus)
		{
			this.BrowserObject.Host.SetFocus(focus);
		}

	}
}
