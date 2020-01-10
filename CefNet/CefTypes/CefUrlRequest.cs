using System;
using System.Collections.Generic;
using CefNet.CApi;

namespace CefNet
{
	public unsafe partial class CefUrlRequest
	{
		/// <summary>
		 /// Create a new URL request that is not associated with a specific browser or
		 /// frame. Use CefFrame::CreateURLRequest instead if you want the request to
		 /// have this association, in which case it may be handled differently (see
		 /// documentation on that function). Requests may originate from the both browser
		 /// process and the render process.
		 /// For requests originating from the browser process:
		 /// it may be intercepted by the client via CefResourceRequestHandler or
		 /// CefSchemeHandlerFactory;
		 /// POST data may only contain only a single element of type PDE_TYPE_FILE
		 /// or PDE_TYPE_BYTES.
		 /// For requests originating from the render process:
		 /// it cannot be intercepted by the client so only http(s) and blob schemes
		 /// are supported.
		 /// POST data may only contain a single element of type PDE_TYPE_BYTES.
		 /// </summary>
		 /// <param name="request">
		 /// The |request| object will be marked as read-only after calling this function.
		 /// </param>
		 /// <param name="context">
		 /// The |context| parameter must be NULL. If |context| is empty the global request
		 /// context will be used.
		 /// </param>
		public CefUrlRequest(CefRequest request, CefUrlRequestClient client, CefRequestContext context)
			: this(CefNativeApi.cef_urlrequest_create(
				(request ?? throw new ArgumentNullException(nameof(request))).GetNativeInstance(),
				client != null ? client.GetNativeInstance() : null,
				context != null ? context.GetNativeInstance() : null
				))
		{

		}

	}
}

