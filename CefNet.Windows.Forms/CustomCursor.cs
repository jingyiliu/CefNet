using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CefNet.Windows.Forms
{
	public sealed class CustomCursor
	{
		private readonly IntPtr _cursorHandle;
		private Cursor _cursor;

		public static Cursor Create(ref CefCursorInfo cursorInfo)
		{
			if (cursorInfo.Buffer == IntPtr.Zero)
				throw new ArgumentOutOfRangeException(nameof(cursorInfo));

			CefSize size = cursorInfo.Size;
			using (var bitmap = new Bitmap(size.Width, size.Height, 4 * size.Width, System.Drawing.Imaging.PixelFormat.Format32bppArgb, cursorInfo.Buffer))
			{
				IntPtr iconHandle = bitmap.GetHicon();
				try
				{
					if (NativeMethods.GetIconInfo(iconHandle, out ICONINFO iconInfo))
					{
						iconInfo.Hotspot = cursorInfo.Hotspot;
						iconInfo.IsIcon = false;
						IntPtr cursorHandle = NativeMethods.CreateIconIndirect(ref iconInfo);
						if (cursorHandle == IntPtr.Zero)
							return Cursors.Default;

						return new CustomCursor(cursorHandle)._cursor;
					}
					else
					{
						return Cursors.Default;
					}
				}
				finally
				{
					NativeMethods.DestroyIcon(iconHandle);
				}
			}
		}

		private CustomCursor(IntPtr cursorHandle)
		{
			_cursorHandle = cursorHandle;
			_cursor = new Cursor(cursorHandle) { Tag = this };
		}

		~CustomCursor()
		{
			if (Interlocked.Exchange(ref _cursor, null) != null)
			{
				NativeMethods.DestroyIcon(_cursorHandle);
			}
		}

	}
}
