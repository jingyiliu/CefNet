using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	internal unsafe sealed class ArrayBuffer : CefV8ArrayBufferReleaseCallback
	{
		private static Dictionary<IntPtr, ArrayBuffer> Root = new Dictionary<IntPtr, ArrayBuffer>();

		private byte[] _buffer;

		public ArrayBuffer(byte[] buffer)
		{
			_buffer = buffer;
			lock (Root)
			{
				Root.Add((IntPtr)this.GetBuffer(), this);
			}
		}

		public void* GetBuffer()
		{
			return null;
		}

		public UIntPtr Length
		{
			get { return new UIntPtr(unchecked((uint)_buffer.Length)); }
		}

		public override void ReleaseBuffer(IntPtr buffer)
		{
			ArrayBuffer instance;
			lock (Root)
			{
				Root.Remove(buffer, out instance);
			}
			if (instance != null)
			{
				Dispose();
			}
		}

		protected override void Dispose(bool disposing)
		{

		}
	}
}
