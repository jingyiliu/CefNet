// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------

using CefNet.CApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace CefNet
{
	public abstract class CefBaseScoped<T> : CefBaseScoped
		where T : unmanaged
	{
		[UnmanagedFunctionPointer(CallingConvention.Winapi)]
		private unsafe delegate void CefActionDelegate(cef_base_scoped_t* self);


		private static readonly unsafe CefActionDelegate fnDel = DelImpl;
		private static readonly Dictionary<IntPtr, CefBaseScoped> Scope = new Dictionary<IntPtr, CefBaseScoped>();

		public unsafe CefBaseScoped()
			: base(Allocate(sizeof(T)))
		{
			lock (Scope)
			{
				Scope.Add((IntPtr)_instance, this);
			}
		}

		public unsafe CefBaseScoped(cef_base_scoped_t* instance)
			: base(instance)
		{

		}

		public unsafe static TClass Wrap<TClass>(Func<IntPtr, TClass> create, T* instance)
		{
			return create(unchecked((IntPtr)instance));
		}

		public new unsafe T* NativeInstance
		{
			get
			{
				return (T*)_instance;
			}
		}

		public new unsafe T* GetNativeInstance()
		{
			return (T*)_instance;
		}

		public static CefBaseScoped GetInstance(IntPtr ptr)
		{
			lock (Scope)
			{
				if (Scope.TryGetValue(ptr, out CefBaseScoped instance))
					return instance;
			}
			return null;
		}

		private unsafe static cef_base_scoped_t* Allocate(int size)
		{
			cef_base_scoped_t* instance = (cef_base_scoped_t*)CefStructure.Allocate(size);
			instance->del = (void*)Marshal.GetFunctionPointerForDelegate(fnDel);
			return instance;
		}

		protected unsafe override void Dispose(bool disposing)
		{
			IntPtr mem = (IntPtr)_instance;
			bool alive;
			lock (Scope)
			{
				alive = Scope.Remove(mem);
			}
			if (alive)
			{
				CefStructure.Free(mem);
				_instance = null;
			}
		}

		private unsafe static void DelImpl(cef_base_scoped_t* self)
		{
			CefBaseScoped instance;
			lock(Scope)
			{
				Scope.TryGetValue((IntPtr)self, out instance);
			}
			((IDisposable)instance)?.Dispose();
		}
	}

	public unsafe abstract class CefBaseScoped : IDisposable
	{
		protected cef_base_scoped_t* _instance;

		public CefBaseScoped(cef_base_scoped_t* instance)
		{
			_instance = instance;
		}

		~CefBaseScoped()
		{
			Dispose(false);
		}

		protected abstract void Dispose(bool disposing);

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		public unsafe cef_base_scoped_t* NativeInstance
		{
			get
			{
				return _instance;
			}
		}

		public unsafe cef_base_scoped_t* GetNativeInstance()
		{
			return _instance;
		}

		public void Del()
		{
			_instance->Del();
		}

		/// <summary>
		/// Makes himself ineligible for garbage collection from the start of the current routine
		/// to the point where this method is called (like &apos;GC.KeepAlive(this)&apos;) and
		/// returns passed <paramref name="result"/>.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <param name="result">Any value of <typeparamref name="T"/> type.</param>
		/// <returns>Returns the passed parameter of <typeparamref name="T"/> type.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public T SafeCall<T>(T result)
		{
			return result;
		}
	}
}
