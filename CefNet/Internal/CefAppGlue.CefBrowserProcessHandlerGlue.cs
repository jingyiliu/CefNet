using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	partial class CefAppGlue
	{
		public void OnContextInitialized()
		{
			_application.OnContextInitialized();
		}

		internal bool AvoidOnBeforeChildProcessLaunch()
		{
			return false;
		}

		public void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
		{
			_application.OnBeforeChildProcessLaunch(new BeforeChildProcessLaunchEventArgs(commandLine));
		}

		internal bool AvoidOnRenderProcessThreadCreated()
		{
			return false;
		}

		public void OnRenderProcessThreadCreated(CefListValue extraInfo)
		{
			_application.OnRenderThreadCreated(new RenderThreadCreatedEventArgs(extraInfo));
		}

		public CefPrintHandler GetPrintHandler()
		{
			return _application.GetPrintHandler();
		}

		internal bool AvoidOnScheduleMessagePumpWork()
		{
			return false;
		}

		public void OnScheduleMessagePumpWork(long delayMs)
		{
			_application.OnScheduleMessagePumpWork(delayMs);
		}
	}
}
