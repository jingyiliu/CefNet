using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace CefNet
{
	public sealed class PlatformInfo
	{

		public static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		public static bool IsMacOS { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

		public static bool IsLinux { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

		
	}



}
