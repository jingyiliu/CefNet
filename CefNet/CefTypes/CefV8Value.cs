using CefNet.CApi;
using CefNet.Unsafe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CefNet
{
	public unsafe partial class CefV8Value
	{

		/// <summary>
		/// Create a new CefV8Value object of type bool.
		/// </summary>
		public CefV8Value(bool value)
			: this(CefNativeApi.cef_v8value_create_bool(value ? 1 : 0))
		{

		}

		/// <summary>
		/// Create a new CefV8Value object of type int.
		/// </summary>
		public CefV8Value(int value)
			: this(CefNativeApi.cef_v8value_create_int(value))
		{

		}

		/// <summary>
		/// Create a new CefV8Value object of type uint.
		/// </summary>
		public CefV8Value(uint value)
			: this(CefNativeApi.cef_v8value_create_uint(value))
		{

		}

		/// <summary>
		/// Create a new CefV8Value object of type double.
		/// </summary>
		public CefV8Value(double value)
			: this(CefNativeApi.cef_v8value_create_double(value))
		{

		}

		/// <summary>
		/// Create a new CefV8Value object of type Date. This function should only be
		/// called from within the scope of a CefRenderProcessHandler, CefV8Handler or
		/// CefV8Accessor callback, or in combination with calling Enter() and Exit()
		/// on a stored CefV8Context reference.
		/// </summary>
		public CefV8Value(DateTime value)
			: this(CreateDate(value))
		{

		}

		private static cef_v8value_t* CreateDate(DateTime date)
		{
			CefTime t = CefTime.FromDateTime(date);
			return CefNativeApi.cef_v8value_create_date((cef_time_t*)&t);
		}

		/// <summary>
		/// Create a new CefV8Value object of type string.
		/// </summary>
		public CefV8Value(string value)
			: this(CreateString(value))
		{

		}

		private static cef_v8value_t* CreateString(string value)
		{
			fixed (char* s = value)
			{
				var cstr = new cef_string_t { Str = s, Length = (value != null ? value.Length : 0) };
				return CefNativeApi.cef_v8value_create_string(&cstr);
			}
		}

		/// <summary>
		/// Create a new cef_v8value_t object of type object with optional accessor
		/// and/or interceptor. This function should only be
		/// called from within the scope of a CefRenderProcessHandler, CefV8Handler or
		/// CefV8Accessor callback, or in combination with calling Enter() and Exit()
		/// on a stored CefV8Context reference.
		/// </summary>
		public CefV8Value(CefV8Accessor accessor, CefV8Interceptor interceptor)
			: this(CefNativeApi.cef_v8value_create_object(accessor != null ? accessor.GetNativeInstance() : null, interceptor != null ? interceptor.GetNativeInstance() : null))
		{

		}

		/// <summary>
		/// Create a new CefV8Value object of type array with the specified |length|.
		/// This function should only be called from within the scope of a
		/// CefRenderProcessHandler, CefV8Handler or CefV8Accessor callback, or in
		/// combination with calling Enter() and Exit() on a stored CefV8Context reference.
		/// </summary>
		/// <param name="length">
		/// If |length| is negative the returned array will have length 0.
		/// </param>
		public static CefV8Value CreateArray(int length)
		{
			return new CefV8Value(CefNativeApi.cef_v8value_create_array(length));
		}


		/// <summary>
		/// Create a new CefV8Value object of type ArrayBuffer which wraps the
		/// provided |buffer| of size |length| bytes. The ArrayBuffer is externalized,
		/// meaning that it does not own |buffer|. The caller is responsible for freeing
		/// |buffer| when requested via a call to CefV8ArrayBufferReleaseCallback::ReleaseBuffer().
		/// This function should only be called from within the scope of a
		/// CefRenderProcessHandler, CefV8Handler or CefV8Accessor callback, or in
		/// combination with calling Enter() and Exit() on a stored CefV8Context reference.
		/// </summary>
		public static CefV8Value CreateArrayBuffer(void* buffer, int length, CefV8ArrayBufferReleaseCallback callback)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));
			if (length < 0)
				throw new ArgumentOutOfRangeException(nameof(length));
			return new CefV8Value(CefNativeApi.cef_v8value_create_array_buffer(buffer, unchecked((UIntPtr)length), callback.GetNativeInstance()));
		}

		/// <summary>
		/// Create a new CefV8Value object of type ArrayBuffer which wraps the
		/// provided |buffer| of size |length| bytes. The ArrayBuffer is externalized,
		/// meaning that it does not own |buffer|.
		/// This function should only be called from within the scope of a
		/// CefRenderProcessHandler, CefV8Handler or CefV8Accessor callback, or in
		/// combination with calling Enter() and Exit() on a stored CefV8Context reference.
		/// </summary>
		public static CefV8Value CreateArrayBuffer(byte[] buffer)
		{
			if (buffer == null)
				throw new ArgumentNullException(nameof(buffer));
			var instance = new ArrayBuffer(buffer);
			return new CefV8Value(CefNativeApi.cef_v8value_create_array_buffer(instance.GetBuffer(), instance.Length, instance.GetNativeInstance()));
		}

		/// <summary>
		/// Create a new CefV8Value object of type function.
		/// This function should only be called from within the scope of a
		/// CefRenderProcessHandler, CefV8Handler or CefV8Accessor callback, or in
		/// combination with calling Enter() and Exit() on a stored CefV8Context reference.
		/// </summary>
		public static CefV8Value CreateFunction(string name, CefV8Handler handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			fixed (char* s = name)
			{
				var aName = new cef_string_t { Str = s, Length = (name != null ? name.Length : 0) };
				return new CefV8Value(CefNativeApi.cef_v8value_create_function(&aName, handler.GetNativeInstance()));
			}
		}

		/// <summary>
		/// Create a new CefV8Value object of type undefined.
		/// </summary>
		public static CefV8Value CreateUndefined()
		{
			return new CefV8Value(CefNativeApi.cef_v8value_create_undefined());
		}

		/// <summary>
		/// Create a new CefV8Value object of type null.
		/// </summary>
		public static CefV8Value CreateNull()
		{
			return new CefV8Value(CefNativeApi.cef_v8value_create_null());
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, CefV8Value value, CefV8PropertyAttribute attributes)
		{
			return SetValueByKey(key, value, attributes);
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, int value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, double value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, bool value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, uint value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, DateTime value, CefV8PropertyAttribute attributes)
		{
			using (var aValue = new CefV8Value(value))
			{
				return SetValueByKey(key, aValue, attributes);
			}
		}

		/// <summary>
		/// Assign a value to a property of an object.
		/// </summary>
		/// <param name="key">The name of the property to be defined or modified.</param>
		/// <param name="value">The new value for the specified property.</param>
		/// <param name="attributes">The property attributes.</param>
		/// <returns>
		/// Returns true on success. Returns false if this function is called incorrectly or an
		/// exception is thrown.
		/// </returns>
		/// <remarks>
		/// For read-only values this function will return true even though assignment failed.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SetValue(string key, string value, CefV8PropertyAttribute attribute)
		{
			using (CefV8Value str = new CefV8Value(value))
			{
				return SetValueByKey(key, str, attribute);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(int index, CefV8Value value)
		{
			SetValueByIndex(index, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetValue(string key, CefV8AccessControl settings, CefV8PropertyAttribute attributes)
		{
			SetValueByAccessor(key, settings, attributes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CefV8Value GetValue(string key)
		{
			return GetValueByKey(key);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CefV8Value GetValue(int index)
		{
			return GetValueByIndex(index);
		}

		public bool CopyV8StringToCefValue(CefValue value)
		{
			if (value == null)
				throw new ArgumentNullException(nameof(value));
			if (!IsString)
				throw new InvalidOperationException();

			cef_string_userfree_t userfreeStr = NativeInstance->GetStringValue();
			try
			{
				return value.NativeInstance->SetString((cef_string_t*)userfreeStr.Base.Base) != 0;
			}
			finally
			{
				CefString.Free(userfreeStr);
			}
		}

		public CefV8ValueType Type
		{
			get
			{
				if (CefApi.UseUnsafeImplementation)
				{
					RefCountedWrapperStruct* ws = RefCountedWrapperStruct.FromRefCounted(this.NativeInstance);
					return ((V8ValueImplLayout*)(ws->cppObject))->Type;
				}

				if (NativeInstance->IsUndefined() != 0) // TYPE_UNDEFINED
					return CefV8ValueType.Undefined;
				if (NativeInstance->IsNull() != 0)
					return CefV8ValueType.Null;
				if (NativeInstance->IsBool() != 0)
					return CefV8ValueType.Bool;
				if (NativeInstance->IsInt() != 0) // TYPE_INT, TYPE_UINT 
					return CefV8ValueType.Int;
				if (NativeInstance->IsDouble() != 0)  // TYPE_INT, TYPE_UINT, TYPE_DOUBLE
					return CefV8ValueType.Double;
				if (NativeInstance->IsDate() != 0)
					return CefV8ValueType.Date;
				if (NativeInstance->IsString() != 0) //TYPE_STRING
					return CefV8ValueType.String;
				if (NativeInstance->IsObject() != 0) //TYPE_OBJECT
					return CefV8ValueType.Object;
				if (NativeInstance->IsUInt() != 0) // TYPE_INT, TYPE_UINT
					return CefV8ValueType.UInt;
				return CefV8ValueType.Invalid;
			}
		}

		private static readonly HashSet<WeakReference<CefV8Value>> WeakRefs = new HashSet<WeakReference<CefV8Value>>();

		private WeakReference<CefV8Value> _weakRef;

		public WeakReference<CefV8Value> WeakRef
		{
			get
			{
				if (_weakRef == null)
				{
					lock (WeakRefs)
					{
						if (_weakRef == null)
							_weakRef = new WeakReference<CefV8Value>(this);
					}
				}
				return _weakRef;
			}
		}

		protected override void Dispose(bool disposing)
		{
			lock (WeakRefs)
			{
				WeakRefs.Remove(WeakRef);
			}
			base.Dispose(disposing);
		}

		public unsafe static CefV8Value Wrap(Func<IntPtr, CefV8Value> create, cef_v8value_t* instance)
		{
			if (instance == null)
				return null;

			IntPtr key = new IntPtr(instance);
			lock (WeakRefs)
			{
				CefV8Value wrapper;
				foreach (WeakReference<CefV8Value> weakRef in WeakRefs)
				{
					if (weakRef.TryGetTarget(out wrapper) && instance->IsSame(wrapper.GetNativeInstance()) != 0)
					{
						instance->@base.Release();
						Debug.Print("V8Value type: {0}", wrapper.Type);
						return wrapper;
					}
				}
				wrapper = create(key);
				WeakRefs.Add(wrapper.WeakRef);
				return wrapper;
			}
		}


	}



}
