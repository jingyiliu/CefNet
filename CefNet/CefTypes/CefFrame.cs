using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public partial class CefFrame
	{
		public override bool Equals(object obj)
		{
			var frame = obj as CefFrame;
			if (frame is null)
				return false;
			return this == frame;
		}

		public unsafe override int GetHashCode()
		{
			if (CefApi.UseUnsafeImplementation)
				return _instance.GetHashCode();
			return IsDisposed ? 0 : NativeInstance->GetIdentifier().GetHashCode();
		}

		public static bool operator ==(CefFrame a, CefFrame b)
		{
			if (ReferenceEquals(a, b))
				return true;

			if (!CefApi.UseUnsafeImplementation)
			{
				if (a is null)
					return b is null;
				if (b is null)
					return false;

				try
				{
					return a.Identifier == b.Identifier;
				}
				catch (ObjectDisposedException) { }
			}
			return false;
		}

		public static bool operator !=(CefFrame a, CefFrame b)
		{
			return !(a == b);
		}
	}
}
