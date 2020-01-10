using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet.Internal
{
	partial class CefAppGlue
	{
		public virtual void OnContextInitialized()
		{

		}

		internal bool AvoidOnBeforeChildProcessLaunch()
		{
			return false;
		}

		public virtual void OnBeforeChildProcessLaunch(CefCommandLine commandLine)
		{

		}

		internal bool AvoidOnRenderProcessThreadCreated()
		{
			return false;
		}

		public virtual void OnRenderProcessThreadCreated(CefListValue extraInfo)
		{

		}

		public virtual CefPrintHandler GetPrintHandler()
		{
			return null;
		}

		internal bool AvoidOnScheduleMessagePumpWork()
		{
			return false;
		}

		public virtual void OnScheduleMessagePumpWork(long delayMs)
		{

		}
	}
}
