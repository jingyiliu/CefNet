using System;
using System.Collections.Generic;
using System.Text;
using CefNet.CApi;

namespace CefNet
{
	public unsafe partial class CefMediaRouter
	{
		/// <summary>
		/// Gets the <see cref="CefMediaRouter"/> object associated with the global request context.
		/// </summary>
		public static unsafe CefMediaRouter Global
		{
			get
			{
				return CefMediaRouter.Wrap(CefMediaRouter.Create, CefNativeApi.cef_media_router_get_global());
			}
		}

	}
}
