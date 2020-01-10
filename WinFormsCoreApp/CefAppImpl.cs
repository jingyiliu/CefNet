using CefNet;
using System;
using System.IO;

namespace WinFormsCoreApp
{
	class CefAppImpl : CefNetApplication
	{
		public override void OnBeforeCommandLineProcessing(string processType, CefCommandLine commandLine)
		{
			base.OnBeforeCommandLineProcessing(processType, commandLine);

			Console.WriteLine("ChromiumWebBrowser_OnBeforeCommandLineProcessing");
			Console.WriteLine(commandLine.CommandLineString);

			//commandLine.AppendSwitchWithValue("proxy-server", "127.0.0.1:8888");


			commandLine.AppendSwitchWithValue("remote-debugging-port", "9222");
			commandLine.AppendSwitch("off-screen-rendering-enabled");
			commandLine.AppendSwitchWithValue("off-screen-frame-rate", "30");
			
			//enable-devtools-experiments
			commandLine.AppendSwitch("enable-devtools-experiments");

			//e.CommandLine.AppendSwitchWithValue("user-agent", "Mozilla/5.0 (Windows 10.0) WebKa/" + DateTime.UtcNow.Ticks);

			//("force-device-scale-factor", "1");

			//commandLine.AppendSwitch("disable-gpu");
			//commandLine.AppendSwitch("disable-gpu-compositing");
			//commandLine.AppendSwitch("disable-gpu-vsync");

			commandLine.AppendSwitch("enable-begin-frame-scheduling");
			commandLine.AppendSwitch("enable-media-stream");
		}

	}
}
