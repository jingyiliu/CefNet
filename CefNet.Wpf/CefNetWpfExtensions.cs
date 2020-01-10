using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CefNet.Wpf
{
	public static class CefNetWpfExtensions
	{
		private delegate object PropertyGetterInvokeDelegate(object obj, object[] parameters);

		private static PropertyGetterInvokeDelegate GetIsExtendedKey;

		static CefNetWpfExtensions()
		{
			try
			{
				PropertyInfo propertyInfo = typeof(KeyEventArgs).GetProperty("IsExtendedKey", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance, null, typeof(bool), Type.EmptyTypes, null);
				if (propertyInfo != null)
				{
					MethodInfo method = propertyInfo.GetGetMethod(true);
					if (method != null)
					{
						GetIsExtendedKey = method.Invoke;
					}
				}
			}
			catch { }
		}

		public static void Union(this ref Int32Rect self, CefRect rect)
		{
			int x = Math.Min(self.X, rect.X);
			int right = Math.Max(self.X + self.Width, rect.X + rect.Width);
			int y = Math.Min(self.Y, rect.Y);
			int bottom = Math.Max(self.Y + self.Height, rect.Y + rect.Height);
			self = new Int32Rect(x, y, right - x, bottom - y);
		}

		public static VirtualKeys ToVirtualKey(this Key key)
		{
			if (key >= Key.LeftShift && key <= Key.RightAlt)
				return (VirtualKeys)((key - Key.LeftShift) >> 1) | VirtualKeys.ShiftKey; // VK_SHIFT, VK_CONTROL, VK_MENU
			if (key == Key.System)
				return VirtualKeys.Menu; // VK_MENU
			return (VirtualKeys)KeyInterop.VirtualKeyFromKey(key);
		}

		public static int ToNativeKeyCode(this VirtualKeys key, CefKeyEventType eventType, bool isRepeat, bool isSystemKey, bool isExtended)
		{
			return CefNetApi.GetNativeKeyCode(eventType, isRepeat ? 1 : 0, key, isSystemKey, isExtended);
		}

		public static int ToNativeKeyCode(this Key key, CefKeyEventType eventType, bool isRepeat, bool isSystemKey, bool isExtended)
		{
			return CefNetApi.GetNativeKeyCode(eventType, isRepeat ? 1 : 0, ToVirtualKey(key), isSystemKey, isExtended);
		}

		public static bool IsExtendedKey(this KeyEventArgs e)
		{
			return (GetIsExtendedKey != null) ? (bool)GetIsExtendedKey(e, null) : false;
		}

		public static Color ToColor(this CefColor color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

#if DEBUG
		internal static void Save(this BitmapSource source, string filename)
		{
			using (var file = new FileStream(filename, FileMode.Create))
			{
				PngBitmapEncoder encoder = new PngBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(source));
				encoder.Save(file);
				file.Flush();
			}

		}
#endif

	}
}
