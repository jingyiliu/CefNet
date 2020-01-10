using System;
using System.Collections.Generic;
using CefNet.CApi;

namespace CefNet
{
	public unsafe partial class CefRequestContext
	{

#if USESAFECACHE

		private static readonly HashSet<WeakReference<CefRequestContext>> WeakRefs = new HashSet<WeakReference<CefRequestContext>>();

		public unsafe static CefRequestContext Wrap(Func<IntPtr, CefRequestContext> create, cef_request_context_t* instance)
		{
			if (instance == null)
				return null;

			IntPtr key = new IntPtr(instance);
			lock (WeakRefs)
			{
				CefRequestContext wrapper;
				foreach (WeakReference<CefRequestContext> weakRef in WeakRefs)
				{
					if (weakRef.TryGetTarget(out wrapper))
					{
						if (wrapper._instance == key
							|| instance->IsSame(wrapper.GetNativeInstance()) != 0)
						{
							instance->@base.Release();
							return wrapper;
						}
					}
				}
				wrapper = CefBaseRefCounted<cef_request_context_t>.Wrap(create, instance);
				WeakRefs.Add(wrapper.WeakRef);
				return wrapper;
			}
		}

#endif // USESAFECACHE

		/// <summary>
		/// Returns the global context object.
		/// </summary>
		public static CefRequestContext GetGlobalContext()
		{
			return CefRequestContext.Wrap(CefRequestContext.Create, CefNativeApi.cef_request_context_get_global_context());
		}

		/// <summary>
		/// Creates a new context object with the specified |settings|.
		/// </summary>
		public CefRequestContext(CefRequestContextSettings settings)
			: this(settings, null)
		{

		}

		/// <summary>
		/// Creates a new context object with the specified |settings| and optional |handler|.
		/// </summary>
		public CefRequestContext(CefRequestContextSettings settings, CefRequestContextHandler handler)
			: this(CefNativeApi.cef_request_context_create_context(
				(settings ?? throw new ArgumentNullException(nameof(settings))).GetNativeInstance(),
				handler != null ? handler.GetNativeInstance() : null))
		{
#if USESAFECACHE
			lock (WeakRefs)
			{
				WeakRefs.Add(this.WeakRef);
			}
#endif
		}

		/// <summary>
		/// Creates a new context object that shares storage with |other|.
		/// </summary>
		public CefRequestContext(CefRequestContext other)
			: this(other, null)
		{

		}

		/// <summary>
		/// Creates a new context object that shares storage with |other| and uses an optional |handler|.
		/// </summary>
		public CefRequestContext(CefRequestContext other, CefRequestContextHandler handler)
			: this(CefNativeApi.cef_create_context_shared(
				(other ?? throw new ArgumentNullException(nameof(other))).GetNativeInstance(),
				handler != null ? handler.GetNativeInstance() : null))
		{
#if USESAFECACHE
			lock (WeakRefs)
			{
				WeakRefs.Add(this.WeakRef);
			}
#endif
		}

#if USESAFECACHE

		protected override void Dispose(bool disposing)
		{
			lock (WeakRefs)
			{
				WeakRefs.Remove(WeakRef);
			}
			base.Dispose(disposing);
		}

		private WeakReference<CefRequestContext> _weakRef;

		public WeakReference<CefRequestContext> WeakRef
		{
			get
			{
				if (_weakRef == null)
				{
					lock (WeakRefs)
					{
						if (_weakRef == null)
							_weakRef = new WeakReference<CefRequestContext>(this);
					}
				}
				return _weakRef;
			}
		}

#endif // USESAFECACHE

	}
}
