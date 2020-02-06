
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefNet.JSInterop
{
	public static class V8Extensions
	{
		public static CefV8Value CreateObject(this CefV8Context context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			using (CefV8Value global = context.GetGlobal())
			using (CefV8Value ctor = global.GetValue("Object"))
			{
				return ctor.ExecuteFunction(global, new CefV8Value[0]);
			}
		}

		public static Task<ScriptableObject> GetScriptableObjectAsync(this CefFrame self, CancellationToken cancellationToken)
		{
			if (self is null)
				throw new ArgumentNullException(nameof(self));

			if (CefApi.CurrentlyOn(CefThreadId.Renderer))
				return Task.FromResult(GetScriptableObject(self));

			return Task.Run(() => GetScriptableObject(self));
		}

		private static ScriptableObject GetScriptableObject(CefFrame frame)
		{
			var provider = new ScriptableObjectProvider(frame);
			return new ScriptableObject(provider.GetGlobal(), provider);
		}
	}
}
