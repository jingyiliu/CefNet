// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------

using CefNet.CApi;
using CefNet.Unsafe;
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
	sealed class RefCountedReference
	{
		public RefCountedReference(WeakReference<CefBaseRefCounted> weakRef)
		{
			_count = 0;
			Instance = weakRef;
		}

		private int _count;
		public readonly WeakReference<CefBaseRefCounted> Instance;
		private CefBaseRefCounted Root;

		public int Count
		{
			get
			{
				lock (this)
				{
					return _count;
				}
			}
		}

		public bool IsRooted
		{
			get
			{
				lock (this)
				{
					return Root != null;
				}
			}
		}

		public void AddRef()
		{
			lock (this)
			{
				_count++;
				if (_count == 1)
				{
					if (Instance.TryGetTarget(out CefBaseRefCounted instance))
						Root = instance;
					else
						throw new InvalidCefObjectException();
				}
			}
		}

		public int Release()
		{
			int count;
			lock(this)
			{
				count = --_count;
				if (count == 0)
				{
					Root = null;
				}
			}
			return count;
		}
	}
	
	[UnmanagedFunctionPointer(CallingConvention.Winapi)]
	internal unsafe delegate void CefActionDelegate(cef_base_ref_counted_t* self);

	[UnmanagedFunctionPointer(CallingConvention.Winapi)]
	internal unsafe delegate int CefIntFunctionDelegate(cef_base_ref_counted_t* self);

	public abstract class CefBaseRefCounted<T> : CefBaseRefCounted
		where T : unmanaged
	{
		private static readonly unsafe CefActionDelegate fnAddRef = AddRefImpl;
		private static readonly unsafe CefIntFunctionDelegate fnRelease = ReleaseImpl;
		private static readonly unsafe CefIntFunctionDelegate fnHasOneRef = HasOneRefImpl;
		private static readonly unsafe CefIntFunctionDelegate fnHasAtLeastOneRef = HasAtLeastOneRefImpl;
		internal static readonly Dictionary<IntPtr, RefCountedReference> RefCounted = new Dictionary<IntPtr, RefCountedReference>();
		internal static readonly Dictionary<IntPtr, WeakReference<CefBaseRefCounted>> UnsafeRefCounted = new Dictionary<IntPtr, WeakReference<CefBaseRefCounted>>();
		public static readonly ReaderWriterLockSlim GlobalSyncRoot = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

		protected static IEnumerable<CefBaseRefCounted> GetCached<TClass>()
			where TClass : CefBaseRefCounted
		{
			GlobalSyncRoot.EnterReadLock();
			try
			{
				foreach (RefCountedReference reference in RefCounted.Values)
				{
					if (reference.Instance.TryGetTarget(out CefBaseRefCounted instance)
						&& instance is TClass wrapper)
					{
						yield return wrapper; 
					}
				}
			}
			finally
			{
				GlobalSyncRoot.ExitReadLock();
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe static RefCountedWrapperStruct* GetWrapperStructPtr(void* instance)
		{
			if (CefStructure.IsAllocated(new IntPtr(instance)))
				return null;
			return RefCountedWrapperStruct.FromRefCounted(instance);
		}

		public unsafe static TClass Wrap<TClass>(Func<IntPtr, TClass> create, T* instance)
			where TClass : CefBaseRefCounted<T>
		{
			if (instance == null)
				return null;

			RefCountedWrapperStruct* ws = null;
			CefBaseRefCounted wrapper;
			IntPtr key = new IntPtr(instance);
			GlobalSyncRoot.EnterUpgradeableReadLock();
			try
			{
				if (CefApi.UseUnsafeImplementation)
				{
					ws = GetWrapperStructPtr(instance);
					if (ws != null && UnsafeRefCounted.TryGetValue(ws->cppObject, out WeakReference<CefBaseRefCounted> weakRef)
						&& weakRef.TryGetTarget(out wrapper))
					{
						((cef_base_ref_counted_t*)instance)->Release();
						return (TClass)wrapper;
					}
				}

				if (RefCounted.TryGetValue(key, out RefCountedReference reference)
					&& reference.Instance.TryGetTarget(out wrapper))
				{
					((cef_base_ref_counted_t*)instance)->Release();
					return (TClass)wrapper;
				}
#if DEBUG
				else if(CefStructure.IsAllocated(key))
				{
					throw new InvalidCefObjectException(string.Format("Unexpected access to {0}.", typeof(TClass).Name));
				}
#endif
				else
				{
					GlobalSyncRoot.EnterWriteLock();
					try
					{
						TClass typedWrapper = create(key);
						var weakRef = new WeakReference<CefBaseRefCounted>(typedWrapper);
						RefCounted[key] = new RefCountedReference(weakRef);
						if (ws != null)
						{
							UnsafeRefCounted[ws->cppObject] = weakRef;
						}
						return typedWrapper;
					}
					finally
					{
						GlobalSyncRoot.ExitWriteLock();
					}
				}
			}
			finally
			{
				GlobalSyncRoot.ExitUpgradeableReadLock();
			}
		}

		public unsafe CefBaseRefCounted()
			: base(Allocate(sizeof(T)))
		{
			var reference = new RefCountedReference(new WeakReference<CefBaseRefCounted>(this));
			GlobalSyncRoot.EnterWriteLock();
			try
			{
				RefCounted.Add(_instance, reference);
			}
			finally
			{
				GlobalSyncRoot.ExitWriteLock();
			}
		}

		public unsafe CefBaseRefCounted(cef_base_ref_counted_t* instance)
			: base(instance)
		{

		}

		protected unsafe override void Dispose(bool disposing)
		{
			IntPtr key = Volatile.Read(ref _instance);
			if (key != IntPtr.Zero)
			{
				GlobalSyncRoot.EnterWriteLock();
				try
				{
					if (CefApi.UseUnsafeImplementation)
					{
						RefCountedWrapperStruct* ws = GetWrapperStructPtr((void*)key);
						if (ws != null) UnsafeRefCounted.Remove(ws->cppObject);
					}
					RefCounted.Remove(key);
				}
				finally
				{
					GlobalSyncRoot.ExitWriteLock();
				}
#if NETFRAMEWORK
				if (Environment.HasShutdownStarted)
				{
					if (CefStructure.IsAllocated(key)) // allow leaks to fix potential UAF
						return;
				}
				else
#endif
				if (CefStructure.Free(key))
					return;
				
				base.Dispose(disposing);
			}
		}

		public new unsafe T* NativeInstance
		{
			get
			{
				return (T*)base.NativeInstance;
			}
		}

		public new unsafe T* GetNativeInstance()
		{
			return (T*)base.GetNativeInstance();
		}

		public static CefBaseRefCounted GetInstance(IntPtr ptr)
		{
			RefCountedReference reference;
			GlobalSyncRoot.EnterReadLock();
			try
			{
				RefCounted.TryGetValue(ptr, out reference);
				if (reference != null && reference.Instance.TryGetTarget(out CefBaseRefCounted instance))
					return instance;
			}
			finally
			{
				GlobalSyncRoot.ExitReadLock();
			}
			return null;
		}

		private unsafe static cef_base_ref_counted_t* Allocate(int size)
		{
			cef_base_ref_counted_t* instance = (cef_base_ref_counted_t*)CefStructure.Allocate(size);
			instance->add_ref = (void*)Marshal.GetFunctionPointerForDelegate(fnAddRef);
			instance->release = (void*)Marshal.GetFunctionPointerForDelegate(fnRelease);
			instance->has_one_ref = (void*)Marshal.GetFunctionPointerForDelegate(fnHasOneRef);
			instance->has_at_least_one_ref = (void*)Marshal.GetFunctionPointerForDelegate(fnHasAtLeastOneRef);
			return instance;
		}

		private unsafe static void AddRefImpl(cef_base_ref_counted_t* self)
		{
			RefCountedReference reference;
			GlobalSyncRoot.EnterReadLock();
			try
			{
				RefCounted.TryGetValue(new IntPtr(self), out reference);
			}
			finally
			{
				GlobalSyncRoot.ExitReadLock();
			}
			if (reference == null)
				throw new InvalidOperationException();
			reference.AddRef();
		}

		private unsafe static int ReleaseImpl(cef_base_ref_counted_t* self)
		{
			RefCountedReference reference;
			GlobalSyncRoot.EnterReadLock();
			try
			{
				RefCounted.TryGetValue(new IntPtr(self), out reference);
			}
			finally
			{
				GlobalSyncRoot.ExitReadLock();
			}
			if (reference == null)
			{
				if (Environment.HasShutdownStarted)
					return 0;
				throw new InvalidOperationException();
			}
			return reference.Release();
		}

		private unsafe static int HasOneRefImpl(cef_base_ref_counted_t* self)
		{
			RefCountedReference reference;
			GlobalSyncRoot.EnterReadLock();
			try
			{
				RefCounted.TryGetValue(new IntPtr(self), out reference);
			}
			finally
			{
				GlobalSyncRoot.ExitReadLock();
			}
			return (reference != null && reference.Count == 1) ? 1 : 0;
		}

		private unsafe static int HasAtLeastOneRefImpl(cef_base_ref_counted_t* self)
		{
			RefCountedReference reference;
			GlobalSyncRoot.EnterReadLock();
			try
			{
				RefCounted.TryGetValue(new IntPtr(self), out reference);
			}
			finally
			{
				GlobalSyncRoot.ExitReadLock();
			}
			return (reference != null && reference.IsRooted) ? 1 : 0;
		}
	}

	public unsafe abstract class CefBaseRefCounted : IDisposable//, IAsyncDisposable
	{
		private protected IntPtr _instance;

		public CefBaseRefCounted(cef_base_ref_counted_t* instance)
		{
			_instance = new IntPtr(instance);
		}

		~CefBaseRefCounted()
		{
			Dispose(false);
		}

		public unsafe cef_base_ref_counted_t* NativeInstance
		{
			get
			{
				cef_base_ref_counted_t* instance = (cef_base_ref_counted_t*)Volatile.Read(ref _instance);
				if (instance == null)
					throw new ObjectDisposedException(this.GetType().Name);
				return instance;
			}
		}

		public unsafe cef_base_ref_counted_t* GetNativeInstance()
		{
			cef_base_ref_counted_t* instance = (cef_base_ref_counted_t*)Volatile.Read(ref _instance);
			if (instance == null)
				throw new ObjectDisposedException(this.GetType().Name);
			instance->AddRef();
			return instance;
		}

		public bool IsDisposed
		{
			get { return _instance == IntPtr.Zero; }
		}

		public void AddRef()
		{
			NativeInstance->AddRef();
		}

		public bool Release()
		{
			return NativeInstance->Release() != 0;
		}

		public bool HasOneRef()
		{
			return NativeInstance->HasOneRef() != 0;
		}

		public bool HasAtLeastOneRef()
		{
			return NativeInstance->HasAtLeastOneRef() != 0;
		}

		protected virtual void Dispose(bool disposing)
		{
			ReleaseIfNonNull((cef_base_ref_counted_t*)Interlocked.Exchange(ref _instance, IntPtr.Zero));
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ReleaseIfNonNull(cef_base_ref_counted_t* instance)
		{
			if (instance != null)
				instance->Release();
		}

	}

	internal unsafe sealed class UnknownRefCounted
		: CefBaseRefCounted<cef_base_ref_counted_t>
	{
		internal static unsafe UnknownRefCounted Create(IntPtr instance)
		{
			return new UnknownRefCounted((cef_base_ref_counted_t*)instance);
		}

		public UnknownRefCounted(cef_base_ref_counted_t* instance)
			: base(instance)
		{

		}

	}
}
