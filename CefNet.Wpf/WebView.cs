using CefNet.Internal;
using CefNet.WinApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace CefNet.Wpf
{
	public partial class WebView : Control, IWpfWebViewPrivate, IDisposable
	{
		private CefRect _windowBounds;
		private bool _allowResizeNotifications = true;
		private IntPtr _keyboardLayout;
		private Dictionary<InitialPropertyKeys, object> InitialPropertyBag = new Dictionary<InitialPropertyKeys, object>();

		public static RoutedEvent StatusTextChangedEvent = EventManager.RegisterRoutedEvent("StatusTextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WebView));

		public event RoutedEventHandler StatusTextChanged
		{
			add { AddHandler(StatusTextChangedEvent, value); }
			remove { RemoveHandler(StatusTextChangedEvent, value); }
		}

		public WebView()
			: this(null)
		{

		}

		public WebView(WebView opener)
		{
			if (IsDesignMode)
			{
				//BackColor = System.Drawing.Color.White;
				return;
			}
			if (opener != null)
			{
				this.Opener = opener;
				this.BrowserSettings = opener.BrowserSettings;
			}
			Initialize();
		}

		protected bool IsDesignMode
		{
			get
			{
				return DesignerProperties.GetIsInDesignMode(this);
				//Windows.ApplicationModel.DesignMode.DesignModeEnabled
				// DesignerProperties.IsInDesignTool;
			}
		}

		protected OffscreenGraphics OffscreenGraphics { get; private set; }
		
		protected virtual void OnCreateBrowser()
		{
			if (this.Opener != null)
				return;

			if (GetState(State.Creating) || GetState(State.Created))
				throw new InvalidOperationException();

			SetState(State.Creating, true);

			Dictionary<InitialPropertyKeys, object> propertyBag = InitialPropertyBag;
			InitialPropertyBag = null;

			var wpfwindow = System.Windows.Window.GetWindow(this);
			if (wpfwindow == null)
				throw new InvalidOperationException("Window not found!");

			using (var windowInfo = new CefWindowInfo())
			{
				windowInfo.SetAsWindowless(new WindowInteropHelper(wpfwindow).Handle);

				string initialUrl = null;
				CefDictionaryValue extraInfo = null;
				CefRequestContext requestContext = null;
				CefBrowserSettings browserSettings = null;
				if (propertyBag != null)
				{
					object value;
					if (propertyBag.TryGetValue(InitialPropertyKeys.Url, out value))
						initialUrl = value as string;
					if (propertyBag.TryGetValue(InitialPropertyKeys.BrowserSettings, out value))
						browserSettings = value as CefBrowserSettings;
					if (propertyBag.TryGetValue(InitialPropertyKeys.RequestContext, out value))
						requestContext = value as CefRequestContext;
					if (propertyBag.TryGetValue(InitialPropertyKeys.ExtraInfo, out value))
						extraInfo = value as CefDictionaryValue;
				}

				if (initialUrl == null)
					initialUrl = "about:blank";
				if (browserSettings == null)
					browserSettings = DefaultBrowserSettings;

				if (!CefApi.CreateBrowser(windowInfo, ViewGlue.Client, initialUrl, browserSettings, extraInfo, requestContext))
					throw new InvalidOperationException("Failed to create browser instance.");
			}
		}

		private void SetInitProperty(InitialPropertyKeys key, object value)
		{
			var propertyBag = InitialPropertyBag;
			if (propertyBag != null)
			{
				propertyBag[key] = value;
			}
			else
			{
				throw new InvalidOperationException("This property must be set before the underlying CEF browser is created.");
			}
		}

		private T GetInitProperty<T>(InitialPropertyKeys key)
		{
			var propertyBag = InitialPropertyBag;
			if (propertyBag != null && propertyBag.TryGetValue(key, out object value))
			{
				return (T)value;
			}
			return default;
		}

		protected virtual void Initialize()
		{
			this.FocusVisualStyle = null;
			//defaultTooltip = new ToolTip();
			ToolTip = new ToolTip { Visibility = Visibility.Collapsed };
			this.ViewGlue = CreateWebViewGlue();
		}

		protected virtual WebViewGlue CreateWebViewGlue()
		{
			return new WpfWebViewGlue(this);
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);
			GlobalHooks.Initialize(this);
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.ViewGlue != null)
			{
				SetState(State.Closing, true);
				BrowserObject?.Host.CloseBrowser(true);
				this.ViewGlue = null;
			}
		}

		public string StatusText { get; protected set; }

		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			if (OffscreenGraphics != null)
			{
				OffscreenGraphics.Render(drawingContext);
			}
			else
			{
				drawingContext.DrawText(
					new FormattedText(this.GetType().Name,
					CultureInfo.InvariantCulture,
					FlowDirection.LeftToRight,
					new Typeface(FontFamily, FontStyle, FontWeight, FontStretch),
					FontSize,
					Brushes.Black,
					VisualTreeHelper.GetDpi(this).PixelsPerDip),
					new Point(10, 10));
			}
		}

		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			if (_allowResizeNotifications)
			{
				OnUpdateRootBounds();
				if (OffscreenGraphics != null && OffscreenGraphics.SetSize((int)this.ActualWidth, (int)this.ActualHeight))
				{
					OffscreenGraphics.Dpi = VisualTreeHelper.GetDpi(this);
					BrowserObject?.Host.WasResized();
				}
			}

			base.OnRenderSizeChanged(sizeInfo);
		}

		protected void OnUpdateRootBounds()
		{
			Window window = Window.GetWindow(this);
			if (window != null)
			{
				RootBoundsChanged(new CefRect((int)window.Left, (int)window.Top, (int)window.ActualWidth, (int)window.ActualHeight));
			}
		}

		protected void SuspendResizeNotifications()
		{
			_allowResizeNotifications = false;
		}

		protected void ResumeResizeNotifications()
		{
			_allowResizeNotifications = true;
			OnRenderSizeChanged(new SizeChangedInfo(this, new Size(ActualWidth, ActualHeight), false, false));
		}

		protected internal virtual void OnRootResizeBegin(EventArgs e)
		{
			SuspendResizeNotifications();
		}

		protected internal virtual void OnRootResizeEnd(EventArgs e)
		{
			ResumeResizeNotifications();
		}

		protected internal void RootBoundsChanged(CefRect bounds)
		{
			int previousWidth = _windowBounds.Width;
			int previousHeight = _windowBounds.Height;
			_windowBounds = bounds;

			if (_allowResizeNotifications)// || (previousWidth == bounds.Width && previousHeight == bounds.Height))
			{
				BrowserObject?.Host.NotifyScreenInfoChanged();
			}
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (constraint.IsEmpty)
				return new Size(1, 1);
			return base.MeasureOverride(constraint);
		}

		protected override Size ArrangeOverride(Size arrangeBounds)
		{
			arrangeBounds = base.ArrangeOverride(arrangeBounds);
			if (!IsDesignMode)
			{
				if (OffscreenGraphics == null)
				{
					OffscreenGraphics = new OffscreenGraphics();
					OnCreateBrowser();
				}
			}
			return arrangeBounds;
		}

		protected virtual void RaiseCrossThreadEvent<TEventArgs>(Action<TEventArgs> raiseEvent, TEventArgs e, bool synchronous)
			where TEventArgs : EventArgs
		{
			if (synchronous)
				Dispatcher.Invoke(new CrossThreadEventMethod<TEventArgs>(raiseEvent, e).Invoke, this);
			else
				Dispatcher.BeginInvoke(new CrossThreadEventMethod<TEventArgs>(raiseEvent, e).Invoke, this);
		}

		protected virtual void OnBrowserCreated(EventArgs e)
		{
			BrowserCreated?.Invoke(this, e);
		}

		protected virtual void OnLoadingStateChange(LoadingStateChangeEventArgs e)
		{
			LoadingStateChange?.Invoke(this, e);
		}

		protected virtual void OnCefPaint(CefPaintEventArgs e)
		{
			OffscreenGraphics.Draw(e);
			CefPaint?.Invoke(this, e);
			Dispatcher.BeginInvoke(new Action(() => { this.InvalidateVisual(); }), DispatcherPriority.Render);
		}

		protected virtual void OnPopupShow(PopupShowEventArgs e)
		{
			OffscreenGraphics.SetPopup(e);
		}

		private CefPoint PointToViewport(CefPoint point)
		{
			return point;
		}

		public new Point PointToScreen(Point point)
		{
			if (PresentationSource.FromVisual(this) != null)
				return base.PointToScreen(point);
			CefRect viewRect = OffscreenGraphics.GetBounds();
			point.Offset(viewRect.X, viewRect.Y);
			return point;
		}

		void IChromiumWebViewPrivate.RaisePopupBrowserCreating()
		{
			SetState(State.Creating, true);
			InitialPropertyBag = null;
		}

		bool IChromiumWebViewPrivate.GetCefScreenInfo(ref CefScreenInfo screenInfo)
		{
			return false;
		}

		unsafe bool IChromiumWebViewPrivate.CefPointToScreen(ref CefPoint point)
		{
			Point pt = new Point(point.X, point.Y);
			Thread.MemoryBarrier();
			Dispatcher.Invoke(new Action(() =>
			{
				Thread.MemoryBarrier();
				pt = PointToScreen(pt);
				Thread.MemoryBarrier();
			}), DispatcherPriority.Render);
			Thread.MemoryBarrier();

			//NativeMethods.MapWindowPoints(OffscreenGraphics.WidgetHandle, IntPtr.Zero, ref point, 1);
			
			point.X = (int)Math.Round(pt.X);
			point.Y = (int)Math.Round(pt.Y);
			return true;
		}

		float IChromiumWebViewPrivate.GetDevicePixelRatio()
		{
			return (float)OffscreenGraphics.Dpi.PixelsPerDip;
		}

		CefRect IChromiumWebViewPrivate.GetCefRootBounds()
		{
			return _windowBounds;
		}

		CefRect IChromiumWebViewPrivate.GetCefViewBounds()
		{
			return OffscreenGraphics.GetBounds();
		}

		bool IChromiumWebViewPrivate.RaiseRunContextMenu(CefFrame frame, CefContextMenuParams menuParams, CefMenuModel model, CefRunContextMenuCallback callback)
		{
			if (model.Count == 0)
			{
				callback.Cancel();
				return true;
			}
			return (bool)Dispatcher.Invoke(
				new Func<WpfContextMenuRunner, Point, bool>(RunContextMenu), 
				new WpfContextMenuRunner(model, callback),
				new Point(menuParams.XCoord, menuParams.YCoord)
			);
		}

		private bool RunContextMenu(WpfContextMenuRunner runner, Point position)
		{
			if (this.ContextMenu != null)
			{
				runner.Cancel();
				return true;
			}
			runner.Build();
			runner.RunMenuAt(this, position);
			return true;
		}

		void IWpfWebViewPrivate.RaiseCefCursorChange(CursorChangeEventArgs e)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				this.Cursor = e.Cursor;
			}), DispatcherPriority.Normal);
		}

		void IWpfWebViewPrivate.CefSetToolTip(string text)
		{
			Dispatcher.BeginInvoke(new Action<string>(OnSetToolTip), text);
		}

		protected virtual void OnSetToolTip(string text)
		{
			if (this.ToolTip is ToolTip tooltip)
			{
				if (string.IsNullOrWhiteSpace(text))
				{
					tooltip.IsOpen = false;
					tooltip.Visibility = Visibility.Collapsed;
				}
				else
				{
					if (!string.Equals(text, tooltip.Content as string))
						tooltip.Content = text;
					if (tooltip.Visibility != Visibility.Visible)
						tooltip.Visibility = Visibility.Visible;
					if (!tooltip.IsOpen)
						tooltip.IsOpen = true;
				}
			}
		}

		void IWpfWebViewPrivate.CefSetStatusText(string statusText)
		{
			this.StatusText = statusText;
			RaiseCrossThreadEvent(OnStatusTextChanged, EventArgs.Empty, false);
		}

		protected virtual void OnStatusTextChanged(EventArgs e)
		{
			RaiseEvent(new RoutedEventArgs(StatusTextChangedEvent, this));
		}


		protected override void OnGotFocus(RoutedEventArgs e)
		{
			BrowserObject?.Host.SetFocus(true);
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			BrowserObject?.Host.SetFocus(false);
			base.OnLostFocus(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			CefEventFlags modifiers = CefEventFlags.None;
			if (e.LeftButton == MouseButtonState.Pressed)
				modifiers |= CefEventFlags.LeftMouseButton;
			if (e.RightButton == MouseButtonState.Pressed)
				modifiers |= CefEventFlags.RightMouseButton;
			Point mousePos = e.GetPosition(this);
			SendMouseMoveEvent((int)mousePos.X, (int)mousePos.Y, modifiers);
			base.OnMouseMove(e);
		}

		protected override void OnMouseLeave(MouseEventArgs e)
		{
			SendMouseLeaveEvent();
			base.OnMouseLeave(e);
		}

		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			if (!IsFocused)
			{
				Focus();
			}
			if (e.ChangedButton <= MouseButton.Right)
			{
				Point mousePos = e.GetPosition(this);
				SendMouseClickEvent((int)mousePos.X, (int)mousePos.Y, GetButton(e), false, e.ClickCount, GetModifierKeys());
			}
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			if (e.ChangedButton > MouseButton.Right)
				return;

			Point mousePos = e.GetPosition(this);
			SendMouseClickEvent((int)mousePos.X, (int)mousePos.Y, GetButton(e), true, e.ClickCount, GetModifierKeys());
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			base.OnMouseWheel(e);
			
			if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
			{
				Point mousePos = e.GetPosition(this);
				SendMouseWheelEvent((int)mousePos.X, (int)mousePos.Y, 0, e.Delta);
				e.Handled = true;
				return;
			}
			OnMouseHWheel(e);
		}

		protected internal virtual void OnMouseHWheel(MouseWheelEventArgs e)
		{
			Point mousePos = e.GetPosition(this);
			SendMouseWheelEvent((int)mousePos.X, (int)mousePos.Y, e.Delta, 0);
			e.Handled = true;
		}

		protected virtual bool ProcessPreviewKey(CefKeyEventType eventType, KeyEventArgs e)
		{
			SetKeyboardLayoutForCefUIThreadIfNeeded();

			Key key = e.Key;
			VirtualKeys virtualKey = (key == Key.System ? e.SystemKey.ToVirtualKey() : key.ToVirtualKey());

			var k = new CefKeyEvent();
			k.Type = eventType;
			k.Modifiers = (uint)GetCefKeyboardModifiers(e);
			k.IsSystemKey = (key == Key.System);
			k.WindowsKeyCode = (int)virtualKey;
			k.NativeKeyCode = virtualKey.ToNativeKeyCode(eventType, e.IsRepeat, k.IsSystemKey, e.IsExtendedKey());
			this.BrowserObject?.Host.SendKeyEvent(k);

			if (k.IsSystemKey)
				return true;

			// Prevent keyboard navigation using arrows and home and end keys
			if (key >= Key.PageUp && key <= Key.Down)
				return true;

			if (key == Key.Tab)
				return true;

			// Allow Ctrl+A to work when the WebView control is put inside listbox.
			if (key == Key.A && ((CefEventFlags)k.Modifiers).HasFlag(CefEventFlags.ControlDown))
				return true;

			return false;
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			e.Handled = ProcessPreviewKey(CefKeyEventType.RawKeyDown, e);
		}

		protected override void OnPreviewKeyUp(KeyEventArgs e)
		{
			e.Handled = ProcessPreviewKey(CefKeyEventType.KeyUp, e);
		}

		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{
			foreach (char symbol in e.Text)
			{
				var k = new CefKeyEvent();
				k.Type = CefKeyEventType.Char;
				k.WindowsKeyCode = (int)symbol;
				k.NativeKeyCode = ((VirtualKeys)(NativeMethods.VkKeyScan(symbol) & 0xFF)).ToNativeKeyCode(CefKeyEventType.Char, false, false, false);
				k.Modifiers = (uint)GetModifierKeys();
				this.BrowserObject?.Host.SendKeyEvent(k);
			}
			e.Handled = true;
		}

		private static CefMouseButtonType GetButton(MouseButtonEventArgs e)
		{
			switch (e.ChangedButton)
			{
				case MouseButton.Right:
					return CefMouseButtonType.Right;
				case MouseButton.Middle:
					return CefMouseButtonType.Middle;
			}
			return CefMouseButtonType.Left;
		}

		protected static CefEventFlags GetModifierKeys()
		{
			CefEventFlags modifiers = CefEventFlags.None;
			ModifierKeys modKeys = Keyboard.Modifiers;
			if (modKeys.HasFlag(ModifierKeys.Shift))
				modifiers |= CefEventFlags.ShiftDown;
			if (modKeys.HasFlag(ModifierKeys.Control))
				modifiers |= CefEventFlags.ControlDown;
			if (modKeys.HasFlag(ModifierKeys.Alt))
				modifiers |= CefEventFlags.AltDown;
			return modifiers;
		}

		protected CefEventFlags GetCefKeyboardModifiers(KeyEventArgs e)
		{
			CefEventFlags modifiers = GetModifierKeys();

			if (Keyboard.IsKeyToggled(Key.NumLock))
				modifiers |= CefEventFlags.NumLockOn;
			if (Keyboard.IsKeyToggled(Key.CapsLock))
				modifiers |= CefEventFlags.CapsLockOn;
			
			switch (e.Key)
			{
				case Key.Return:
					if (e.IsExtendedKey())
						modifiers |= CefEventFlags.IsKeyPad;
					break;
				case Key.Insert:
				case Key.Delete:
				case Key.Home:
				case Key.End:
				case Key.Prior:
				case Key.Next:
				case Key.Up:
				case Key.Down:
				case Key.Left:
				case Key.Right:
					if (!e.IsExtendedKey())
						modifiers |= CefEventFlags.IsKeyPad;
					break;
				case Key.NumLock:
				case Key.NumPad0:
				case Key.NumPad1:
				case Key.NumPad2:
				case Key.NumPad3:
				case Key.NumPad4:
				case Key.NumPad5:
				case Key.NumPad6:
				case Key.NumPad7:
				case Key.NumPad8:
				case Key.NumPad9:
				case Key.Divide:
				case Key.Multiply:
				case Key.Subtract:
				case Key.Add:
				case Key.Decimal:
				case Key.Clear:
					modifiers |= CefEventFlags.IsKeyPad;
					break;
				case Key.LeftShift:
				case Key.LeftCtrl:
				case Key.LeftAlt:
				case Key.LWin:
					modifiers |= CefEventFlags.IsLeft;
					break;
				case Key.RightShift:
				case Key.RightCtrl:
				case Key.RightAlt:
				case Key.RWin:
					modifiers |= CefEventFlags.IsRight;
					break;
				case Key.System:
					if (e.SystemKey == Key.LeftAlt)
						modifiers |= CefEventFlags.IsLeft;
					else if (e.SystemKey == Key.RightAlt)
						modifiers |= CefEventFlags.IsRight;
					break;
			}
			return modifiers;
		}

		/// <summary>
		/// Sets the current input locale identifier for the UI thread in the browser.
		/// </summary>
		protected void SetKeyboardLayoutForCefUIThreadIfNeeded()
		{
			IntPtr hkl = NativeMethods.GetKeyboardLayout(0);
			if (_keyboardLayout == hkl)
				return;

			if (CefApi.CurrentlyOn(CefThreadId.UI))
			{
				_keyboardLayout = hkl;
			}
			else
			{
				CefNetApi.Post(CefThreadId.UI, () => {
					NativeMethods.ActivateKeyboardLayout(hkl, 0);
					_keyboardLayout = hkl;
				});
			}
		}

	}
}
