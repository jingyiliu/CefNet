﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: include/capi/cef_v8_capi.h
// --------------------------------------------------------------------------------------------﻿
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
// --------------------------------------------------------------------------------------------

#pragma warning disable 0169

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CefNet.WinApi;

namespace CefNet.CApi
{
	/// <summary>
	/// Structure that should be implemented to handle V8 interceptor calls. The
	/// functions of this structure will be called on the thread associated with the
	/// V8 interceptor. Interceptor&apos;s named property handlers (with first argument of
	/// type CefString) are called when object is indexed by string. Indexed property
	/// handlers (with first argument of type int) are called when object is indexed
	/// by integer.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe partial struct cef_v8interceptor_t
	{
		/// <summary>
		/// Base structure.
		/// </summary>
		public cef_base_ref_counted_t @base;

		/// <summary>
		/// int (*)(_cef_v8interceptor_t* self, const cef_string_t* name, _cef_v8value_t* object, _cef_v8value_t** retval, cef_string_t* exception)*
		/// </summary>
		public void* get_byname;

		/// <summary>
		/// Handle retrieval of the interceptor value identified by |name|. |object| is
		/// the receiver (&apos;this&apos; object) of the interceptor. If retrieval succeeds, set
		/// |retval| to the return value. If the requested value does not exist, don&apos;t
		/// set either |retval| or |exception|. If retrieval fails, set |exception| to
		/// the exception that will be thrown. If the property has an associated
		/// accessor, it will be called only if you don&apos;t set |retval|. Return true (1)
		/// if interceptor retrieval was handled, false (0) otherwise.
		/// </summary>
		[NativeName("get_byname")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern int GetByName([Immutable]cef_string_t* name, cef_v8value_t* @object, cef_v8value_t** retval, cef_string_t* exception);

		/// <summary>
		/// int (*)(_cef_v8interceptor_t* self, int index, _cef_v8value_t* object, _cef_v8value_t** retval, cef_string_t* exception)*
		/// </summary>
		public void* get_byindex;

		/// <summary>
		/// Handle retrieval of the interceptor value identified by |index|. |object|
		/// is the receiver (&apos;this&apos; object) of the interceptor. If retrieval succeeds,
		/// set |retval| to the return value. If the requested value does not exist,
		/// don&apos;t set either |retval| or |exception|. If retrieval fails, set
		/// |exception| to the exception that will be thrown. Return true (1) if
		/// interceptor retrieval was handled, false (0) otherwise.
		/// </summary>
		[NativeName("get_byindex")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern int GetByIndex(int index, cef_v8value_t* @object, cef_v8value_t** retval, cef_string_t* exception);

		/// <summary>
		/// int (*)(_cef_v8interceptor_t* self, const cef_string_t* name, _cef_v8value_t* object, _cef_v8value_t* value, cef_string_t* exception)*
		/// </summary>
		public void* set_byname;

		/// <summary>
		/// Handle assignment of the interceptor value identified by |name|. |object|
		/// is the receiver (&apos;this&apos; object) of the interceptor. |value| is the new
		/// value being assigned to the interceptor. If assignment fails, set
		/// |exception| to the exception that will be thrown. This setter will always
		/// be called, even when the property has an associated accessor. Return true
		/// (1) if interceptor assignment was handled, false (0) otherwise.
		/// </summary>
		[NativeName("set_byname")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern int SetByName([Immutable]cef_string_t* name, cef_v8value_t* @object, cef_v8value_t* value, cef_string_t* exception);

		/// <summary>
		/// int (*)(_cef_v8interceptor_t* self, int index, _cef_v8value_t* object, _cef_v8value_t* value, cef_string_t* exception)*
		/// </summary>
		public void* set_byindex;

		/// <summary>
		/// Handle assignment of the interceptor value identified by |index|. |object|
		/// is the receiver (&apos;this&apos; object) of the interceptor. |value| is the new
		/// value being assigned to the interceptor. If assignment fails, set
		/// |exception| to the exception that will be thrown. Return true (1) if
		/// interceptor assignment was handled, false (0) otherwise.
		/// </summary>
		[NativeName("set_byindex")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern int SetByIndex(int index, cef_v8value_t* @object, cef_v8value_t* value, cef_string_t* exception);
	}
}

