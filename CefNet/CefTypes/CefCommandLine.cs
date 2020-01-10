using CefNet.CApi;
using System;

namespace CefNet
{
	public unsafe partial class CefCommandLine
	{
		private static CefCommandLineGlobal _GlobalInstance;
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// Returns the singleton global CefCommandLine object. The returned object will be read-only.
		/// </summary>
		public static CefCommandLine Global
		{
			get
			{
				if (_GlobalInstance == null)
				{
					lock (SyncRoot)
					{
						if (_GlobalInstance == null)
						{
							_GlobalInstance = new CefCommandLineGlobal();
						}
					}
				}
				return _GlobalInstance;
			}
		}

		/// <summary>
		/// Create a new CefCommandLine instance.
		/// </summary>
		public CefCommandLine()
			: this(CefNativeApi.cef_command_line_create())
		{

		}

	}
}

