using CefNet.CApi;

namespace CefNet
{
	public unsafe partial class CefPostData
	{
		/// <summary>
		/// Create a new CefPostData object.
		/// </summary>
		public CefPostData()
			: this(CefNativeApi.cef_post_data_create())
		{

		}
	}
}
