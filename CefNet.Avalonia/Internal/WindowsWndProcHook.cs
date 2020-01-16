using System;
using System.ComponentModel;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using CefNet.WinApi;

namespace CefNet.Internal
{

	internal delegate void WindowsWndProcDelegate(ref CWPSTRUCT msg);

	internal sealed class WindowsWndProcHook : CriticalFinalizerObject, IDisposable
	{
		private delegate IntPtr CallWindProc(int nCode, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, CallWindProc callback, IntPtr hInstance, uint threadId);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		[DllImport("user32.dll")]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, int wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr pPid);


		private IntPtr hhook;
		private readonly CallWindProc wndprocHook;


		public static WindowsWndProcHook FromHwnd(IntPtr hwnd)
		{
			const int WH_CALLWNDPROC = 4;

			uint tid = GetWindowThreadProcessId(hwnd, IntPtr.Zero);
			if (tid == 0)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			var source = new WindowsWndProcHook(hwnd);
			source.hhook = SetWindowsHookEx(WH_CALLWNDPROC, source.wndprocHook, IntPtr.Zero, tid);
			if (source.hhook == IntPtr.Zero)
				throw new Win32Exception(Marshal.GetLastWin32Error());
			return source;
		}

		private WindowsWndProcHook(IntPtr hwnd)
		{
			hhook = IntPtr.Zero;
			this.Handle = hwnd;
			wndprocHook = new CallWindProc(WndProcHook);
		}

		~WindowsWndProcHook()
		{
			Dispose();
		}

		public void Dispose()
		{
			if (hhook != IntPtr.Zero)
			{
				UnhookWindowsHookEx(hhook);
				hhook = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		public IntPtr Handle { get; }

		public WindowsWndProcDelegate WndProcCallback { get; set; }

		private unsafe IntPtr WndProcHook(int code, IntPtr wParam, IntPtr lParam)
		{
			if (code == 0)
			{
				CWPSTRUCT data = *(CWPSTRUCT*)lParam;
				if (data.hwnd == this.Handle)
				{
					WndProcCallback?.Invoke(ref data);
				}
			}
			return CallNextHookEx(hhook, code, (int)wParam, lParam);
		}

	}

}
