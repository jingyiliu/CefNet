using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using System.Runtime.InteropServices;
using Avalonia.VisualTree;
using CefNet.WinApi;
using CefNet.Avalonia;

namespace CefNet.Internal
{
	sealed class GlobalHooks
	{
		private static bool IsInitialized;

		private static Dictionary<IntPtr, GlobalHooks> _HookedWindows = new Dictionary<IntPtr, GlobalHooks>();
		private static List<WeakReference<WebView>> _Views = new List<WeakReference<WebView>>();

		internal static void Initialize(WebView view)
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				return;

			lock(_Views)
			{
				_Views.Add(new WeakReference<WebView>(view));
			}

			if (IsInitialized)
				return;

			IsInitialized = true;

			var lifetime = Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
			if (lifetime is null)
				return;


			Window.WindowOpenedEvent.AddClassHandler(typeof(Window), TryAddGlobalHook, handledEventsToo: true);
			//EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.SizeChangedEvent, new RoutedEventHandler(TryAddGlobalHook));

			foreach (Window window in lifetime.Windows)
			{
				TryAddGlobalHook(window);
			}
		}

		private WindowsWndProcHook _wndprocHook;
		private WeakReference<Window> _windowRef;

		private GlobalHooks(WindowsWndProcHook source, Window window)
		{
			_wndprocHook = source;
			_windowRef = new WeakReference<Window>(window);
			source.WndProcCallback = WndProc;
		}

		private unsafe void WndProc(ref CWPSTRUCT msg)
		{
			switch (msg.message)
			{
				case 0x0002: // WM_DESTROY
					_wndprocHook.WndProcCallback = null;
					foreach (var tuple in GetViews(msg.hwnd))
					{
						tuple.Item1.Close();
					}
					lock (_HookedWindows)
					{
						_HookedWindows.Remove(_wndprocHook.Handle);
						_wndprocHook.Dispose();
					}
					break;
				case 0x0047: //WM_WINDOWPOSCHANGED
					WINDOWPOS* windowPos = (WINDOWPOS*)msg.lParam;
					if ((windowPos->flags & 0x0002) != 0) // SWP_NOMOVE
						break;
					foreach (var tuple in GetViews(msg.hwnd))
					{
						Rect bounds = tuple.Item2.Bounds;
						tuple.Item1.RootBoundsChanged(new CefRect((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height));
					}
					break;
				case 0x0231: // WM_ENTERSIZEMOVE
					foreach (var tuple in GetViews(msg.hwnd))
					{
						tuple.Item1.OnRootResizeBegin(EventArgs.Empty);
					}
					break;
				case 0x0232: // WM_EXITSIZEMOVE
					foreach (var tuple in GetViews(msg.hwnd))
					{
						tuple.Item1.OnRootResizeEnd(EventArgs.Empty);
					}
					break;
			}
		}

		private IEnumerable<ValueTuple<WebView, Window>> GetViews(IntPtr hwnd)
		{
			if (hwnd != _wndprocHook.Handle)
				yield break;

			Window window;
			if (!_windowRef.TryGetTarget(out window))
				yield break;

			lock (_Views)
			{
				for (int i = 0; i < _Views.Count; i++)
				{
					WeakReference<WebView> viewRef = _Views[i];
					if (viewRef.TryGetTarget(out WebView view))
					{
						if (view.GetVisualRoot() == window)
						{
							yield return new ValueTuple<WebView, Window>(view, window);
						}
					}
					else
					{
						_Views.RemoveAt(i--);
					}
				}
			}
		}

		private static void TryAddGlobalHook(object sender, RoutedEventArgs e)
		{
			TryAddGlobalHook(e.Source as Window);
		}

		private static void TryAddGlobalHook(Window window)
		{
			if (window == null)
				return;

			IntPtr hwnd = window.PlatformImpl.Handle.Handle;

			if (_HookedWindows.ContainsKey(hwnd))
				return;

			WindowsWndProcHook source = WindowsWndProcHook.FromHwnd(hwnd);
			if (source == null)
				return;

			_HookedWindows.Add(hwnd, new GlobalHooks(source, window));
		}
	
	}
}
