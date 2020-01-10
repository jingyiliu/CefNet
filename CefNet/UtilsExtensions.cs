using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace CefNet
{
	static class UtilsExtensions
	{
#if NETFRAMEWORK
		public static bool Remove<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, out TValue value)
		{
			if (!self.TryGetValue(key, out value))
				return false;
			return self.Remove(key);
		}
#endif

		/// <summary>
		/// Initializes a block of memory at the given location with a given initial value without assuming architecture dependent alignment of the address.
		/// </summary>
		/// <param name="startAddress">The address of the start of the memory block to initialize.</param>
		/// <param name="value">The value to initialize the block to.</param>
		/// <param name="size">The number of bytes to initialize.</param>
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public static extern void InitBlock(this IntPtr startAddress, byte value, int size);

	}
}
