using CefNet.CApi;

namespace CefNet
{
	public unsafe partial class CefCookieManager
	{
		/// <summary>
		/// Returns the global cookie manager. By default data will be stored at CefSettings.CachePath
		/// if specified or in memory otherwise. Using this function is equivalent to calling
		/// CefRequestContext.GetGlobalContext().GetDefaultCookieManager().
		/// </summary>
		/// <param name="callback">
		/// If |callback| is non-NULL it will be executed asnychronously on the UI thread after the
		/// manager&apos;s storage has been initialized.
		/// </param>
		public static CefCookieManager GetGlobalManager(CefCompletionCallback callback)
		{
			return CefCookieManager.Wrap(CefCookieManager.Create, CefNativeApi.cef_cookie_manager_get_global_manager(callback != null ? callback.GetNativeInstance() : null));
		}
	}
}
