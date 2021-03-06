﻿// --------------------------------------------------------------------------------------------
// Copyright (c) 2019 The CefNet Authors. All rights reserved.
// Licensed under the MIT license.
// See the licence file in the project root for full license information.
// --------------------------------------------------------------------------------------------
// Generated by CefGen
// Source: include/capi/cef_client_capi.h
// --------------------------------------------------------------------------------------------﻿
// DO NOT MODIFY! THIS IS AUTOGENERATED FILE!
// --------------------------------------------------------------------------------------------

#pragma warning disable 0169, 1591, 1573

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CefNet.WinApi;

namespace CefNet.CApi
{
	/// <summary>
	/// Implement this structure to provide handler implementations.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe partial struct cef_client_t
	{
		/// <summary>
		/// Base structure.
		/// </summary>
		public cef_base_ref_counted_t @base;

		/// <summary>
		/// _cef_context_menu_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_context_menu_handler;

		/// <summary>
		/// Return the handler for context menus. If no handler is provided the default
		/// implementation will be used.
		/// </summary>
		[NativeName("get_context_menu_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_context_menu_handler_t* GetContextMenuHandler();

		/// <summary>
		/// _cef_dialog_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_dialog_handler;

		/// <summary>
		/// Return the handler for dialogs. If no handler is provided the default
		/// implementation will be used.
		/// </summary>
		[NativeName("get_dialog_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_dialog_handler_t* GetDialogHandler();

		/// <summary>
		/// _cef_display_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_display_handler;

		/// <summary>
		/// Return the handler for browser display state events.
		/// </summary>
		[NativeName("get_display_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_display_handler_t* GetDisplayHandler();

		/// <summary>
		/// _cef_download_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_download_handler;

		/// <summary>
		/// Return the handler for download events. If no handler is returned downloads
		/// will not be allowed.
		/// </summary>
		[NativeName("get_download_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_download_handler_t* GetDownloadHandler();

		/// <summary>
		/// _cef_drag_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_drag_handler;

		/// <summary>
		/// Return the handler for drag events.
		/// </summary>
		[NativeName("get_drag_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_drag_handler_t* GetDragHandler();

		/// <summary>
		/// _cef_find_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_find_handler;

		/// <summary>
		/// Return the handler for find result events.
		/// </summary>
		[NativeName("get_find_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_find_handler_t* GetFindHandler();

		/// <summary>
		/// _cef_focus_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_focus_handler;

		/// <summary>
		/// Return the handler for focus events.
		/// </summary>
		[NativeName("get_focus_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_focus_handler_t* GetFocusHandler();

		/// <summary>
		/// _cef_jsdialog_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_jsdialog_handler;

		/// <summary>
		/// Return the handler for JavaScript dialogs. If no handler is provided the
		/// default implementation will be used.
		/// </summary>
		[NativeName("get_jsdialog_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_jsdialog_handler_t* GetJSDialogHandler();

		/// <summary>
		/// _cef_keyboard_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_keyboard_handler;

		/// <summary>
		/// Return the handler for keyboard events.
		/// </summary>
		[NativeName("get_keyboard_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_keyboard_handler_t* GetKeyboardHandler();

		/// <summary>
		/// _cef_life_span_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_life_span_handler;

		/// <summary>
		/// Return the handler for browser life span events.
		/// </summary>
		[NativeName("get_life_span_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_life_span_handler_t* GetLifeSpanHandler();

		/// <summary>
		/// _cef_load_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_load_handler;

		/// <summary>
		/// Return the handler for browser load status events.
		/// </summary>
		[NativeName("get_load_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_load_handler_t* GetLoadHandler();

		/// <summary>
		/// _cef_render_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_render_handler;

		/// <summary>
		/// Return the handler for off-screen rendering events.
		/// </summary>
		[NativeName("get_render_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_render_handler_t* GetRenderHandler();

		/// <summary>
		/// _cef_request_handler_t* (*)(_cef_client_t* self)*
		/// </summary>
		public void* get_request_handler;

		/// <summary>
		/// Return the handler for browser request events.
		/// </summary>
		[NativeName("get_request_handler")]
		[MethodImpl(MethodImplOptions.ForwardRef)]
		public unsafe extern cef_request_handler_t* GetRequestHandler();

		/// <summary>
		/// int (*)(_cef_client_t* self, _cef_browser_t* browser, _cef_frame_t* frame, cef_process_id_t source_process, _cef_process_message_t* message)*
		/// </summary>
		public void* on_process_message_received;

		/// <summary>
		/// Called when a new message is received from a different process. Return true
		/// (1) if the message was handled or false (0) otherwise. Do not keep a
		/// reference to or attempt to access the message outside of this callback.
		/// </summary>
		[MethodImpl(MethodImplOptions.ForwardRef)]
		[NativeName("on_process_message_received")]
		public unsafe extern int OnProcessMessageReceived(cef_browser_t* browser, cef_frame_t* frame, CefProcessId source_process, cef_process_message_t* message);
	}
}

