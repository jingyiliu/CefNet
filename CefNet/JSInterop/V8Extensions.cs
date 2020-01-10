
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
		public static CefV8Value GetValue(this CefV8Value self, params string[] names)
		{
			if (names is null)
				throw new ArgumentNullException(nameof(names));

			return GetValue(self, new ArraySegment<string>(names));
		}

		private static CefV8Value GetValue(CefV8Value self, ArraySegment<string> names)
		{
			if (names.Count <= 0)
				throw new ArgumentOutOfRangeException(nameof(names));

			CefV8Value value = null;
			foreach (string name in names)
			{
				if (value is null)
				{
					value = self.GetValue(name);
				}
				else if (value.IsNull)
				{
					throw new InvalidOperationException($"Cannot read property '{name}' of null.");
				}
				else if (value.IsUndefined)
				{
					throw new InvalidOperationException($"Cannot read property '{name}' of undefined.");
				}
				else
				{
					self = value;
					value = self.GetValue(name);
					self.Dispose();
				}
			}
			return value;
		}

		public static bool SetValue(this CefV8Value self, string[] names, CefV8Value value, CefV8PropertyAttribute attribute)
		{
			if (names == null)
				throw new ArgumentNullException(nameof(names));

			int last = names.Length - 1;
			if (last < 0)
				throw new ArgumentOutOfRangeException(nameof(names));

			if (last > 0)
			{
				self = GetValue(self, new ArraySegment<string>(names, 0, last));
			}
			return self.SetValueByKey(names[last], value, attribute);
		}


		public static string GetDOMNodeName(this CefV8Value obj)
		{
			if (!obj.IsObject)
				return null;
			string name;
			CefV8Value nodeName = obj.GetValue("nodeName");
			name = nodeName.IsString ? nodeName.GetStringValue() : null;
			nodeName.Dispose();
			return name;
		}

		public static bool ValidateNodeName(this CefV8Value obj, string nodeName)
		{
			string aNodeName = obj.GetDOMNodeName();
			if (aNodeName is null)
				return false;
			return aNodeName.Equals(nodeName, StringComparison.OrdinalIgnoreCase);
		}

		public static bool ValidateNodeName(this CefV8Value obj, string nodeName1, string nodeName2)
		{
			string aNodeName = obj.GetDOMNodeName();
			if (aNodeName == null)
				return false;
			if (aNodeName.Equals(nodeName1, StringComparison.OrdinalIgnoreCase))
				return true;
			return aNodeName.Equals(nodeName2, StringComparison.OrdinalIgnoreCase);
		}

		public static bool ValidateNodeName(this CefV8Value obj, params string[] nodeNames)
		{
			string nodeName = obj.GetDOMNodeName();
			if (nodeName is null)
				return false;
			for (int i = 0; i < nodeNames.Length; i++)
			{
				if (nodeName.Equals(nodeNames[i], StringComparison.OrdinalIgnoreCase))
					return true;
			}
			return false;
		}

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

		public static bool SetValue(this CefV8Value obj, string key, string value, CefV8PropertyAttribute attribute)
		{
			using (CefV8Value str = new CefV8Value(value))
			{
				return obj.SetValueByKey(key, str, attribute);
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
