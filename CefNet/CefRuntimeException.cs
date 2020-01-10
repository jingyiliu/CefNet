using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	public class CefRuntimeException : Exception
	{
		public CefRuntimeException(string message)
			: base(message)
		{
		}
	}

}
