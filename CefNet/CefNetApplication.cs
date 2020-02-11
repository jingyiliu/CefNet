using CefNet.Internal;
using CefNet.JSInterop;
using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace CefNet
{
	public class CefNetApplication : CefApp
	{
		internal const string XrayRequestKey = "xray-request";
		internal const string XrayResponseKey = "xray-response";
		internal const string XrayReleaseKey = "xray-release";

		/// <summary>
		/// Occurs when a new message is received from a different process. Do not keep a
		/// reference to or attempt to access the message outside of an event handler.
		/// </summary>
		public event EventHandler<CefProcessMessageReceivedEventArgs> CefProcessMessageReceived;

		/// <summary>
		/// Occurs when an exception in a frame is not caught. This event is disabled by default.
		/// To enable set CefSettings.UncaughtExceptionStackSize &gt; 0.
		/// </summary>
		public event EventHandler<CefUncaughtExceptionEventArgs> CefUncaughtException;

		private static ProcessType? _ProcessType;
		private int _initThreadId;

		public CefNetApplication()
		{
			AppGlue = new CefAppGlue(this);
		}

		private CefAppGlue AppGlue { get; }

		public static CefNetApplication Instance { get; private set; }

		public static bool IsInitialized
		{
			get { return Instance != null; }
		}

		private static void AssertApiVersion()
		{
			string hash = CefApi.CefApiHash(CefApiHashType.Universal);
			if (CefApi.ApiHash.Equals(hash, StringComparison.OrdinalIgnoreCase))
				return;

			throw new CefVersionMismatchException(string.Format(
				"CEF runtime version mismatch. Loaded version API hash: '{0}', expected: '{1}' (CEF {2}).",
				hash,
				CefApi.ApiHash,
				typeof(CefApi).Assembly.GetName().Version.Major
			));
		}

		private static string InitializeDllPath(string cefPath)
		{
			if (!string.IsNullOrWhiteSpace(cefPath))
			{
				cefPath = cefPath.Trim();
				if (!Directory.Exists(cefPath))
					throw new DirectoryNotFoundException(string.Format("The CEF runtime can't be initialized from '{0}'.", cefPath));

				string path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
				if (PlatformInfo.IsWindows)
				{
					if (!path.StartsWith(cefPath, StringComparison.CurrentCultureIgnoreCase)
						|| (path.Length > cefPath.Length && path[cefPath.Length] != ';'))
					{
						Environment.SetEnvironmentVariable("PATH", cefPath + ";" + path);
					}
					return Path.Combine(cefPath, "libcef.dll");
				}
				else if (PlatformInfo.IsLinux)
				{
					if (!path.StartsWith(cefPath, StringComparison.CurrentCulture)
						|| (path.Length > cefPath.Length && path[cefPath.Length] != ':'))
					{
						Environment.SetEnvironmentVariable("PATH", cefPath + ":" + path);
					}
					path = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? string.Empty;
					if (!path.StartsWith(cefPath, StringComparison.CurrentCulture)
						|| (path.Length > cefPath.Length && path[cefPath.Length] != ';'))
					{
						Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", cefPath + ":" + path);
					}
					return Path.Combine(cefPath, "libcef.so");
				}
				else if (PlatformInfo.IsMacOS)
				{
					if (!path.StartsWith(cefPath, StringComparison.CurrentCulture)
						|| (path.Length > cefPath.Length && path[cefPath.Length] != ':'))
					{
						Environment.SetEnvironmentVariable("PATH", cefPath + ":" + path);
					}
					path = Environment.GetEnvironmentVariable("DYLD_LIBRARY_PATH") ?? string.Empty;
					if (!path.StartsWith(cefPath, StringComparison.CurrentCulture)
						|| (path.Length > cefPath.Length && path[cefPath.Length] != ';'))
					{
						Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", cefPath + ":" + path);
					}
					return Path.Combine(cefPath, "libcef.so");
				}
			}
			return PlatformInfo.IsWindows ? "libcef.dll" : "libcef.so";
		}

		/// <summary>
		/// Initializes CEF from specified path with user-provided settings. This
		/// function should be called on the main application thread to initialize
		/// CEF processes.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="settings"></param>
		/// <exception cref="DllNotFoundException"></exception>
		/// <exception cref="DirectoryNotFoundException"></exception>
		/// <exception cref="CefVersionMismatchException"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		public void Initialize(string path, CefSettings settings)
		{
			if (PlatformInfo.IsWindows)
			{
				if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
					throw new InvalidOperationException("The calling thread must be STA");
			}

			if (IsInitialized)
				throw new InvalidOperationException("CEF already initialized. You must call Initialize once per application process.");

			path = InitializeDllPath(path);

			if (PlatformInfo.IsWindows)
			{
				const int LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;
				if (IntPtr.Zero == NativeMethods.LoadLibraryEx(path, IntPtr.Zero, LOAD_WITH_ALTERED_SEARCH_PATH))
					throw new DllNotFoundException(string.Format("Can't load '{0}' (error: {1}).", path, Marshal.GetLastWin32Error()));
			}
			else if (PlatformInfo.IsLinux || PlatformInfo.IsMacOS)
			{
				const int RTLD_NOW = 2;
				if (IntPtr.Zero == NativeMethods.dlopen(path, RTLD_NOW))
					throw new DllNotFoundException(string.Format("Can't load '{0}'.", path));
			}
			else
			{
				throw new PlatformNotSupportedException();
			}

			AssertApiVersion();

			Interlocked.Exchange(ref _initThreadId, Thread.CurrentThread.ManagedThreadId);
			Instance = this;

			// Main args
			CefMainArgs main_args = CefMainArgs.CreateDefault();
			int retval = CefApi.ExecuteProcess(main_args, this, IntPtr.Zero);
			if (retval != -1)
				Environment.Exit(retval);

			if (!CefApi.Initialize(main_args, settings, this, IntPtr.Zero))
				throw new CefRuntimeException("Failed to initialize the CEF browser process.");

			GC.KeepAlive(settings);
		}

		public void Shutdown()
		{
			if (_initThreadId != Thread.CurrentThread.ManagedThreadId)
				throw new InvalidOperationException();

			CefApi.Shutdown();
		}

		/// <summary>
		/// Gets the type of the current process.
		/// </summary>
		public static ProcessType ProcessType
		{
			get
			{
				if (_ProcessType != null)
					return _ProcessType.Value;

				string type = Environment.GetCommandLineArgs().FirstOrDefault(arg => arg.StartsWith("--type="));
				if (type is null)
				{
					_ProcessType = ProcessType.Main;
					return ProcessType.Main;
				}

				switch (type.Substring(7))
				{
					case "renderer":
						_ProcessType = ProcessType.Renderer;
						break;
					case "zygote":
						_ProcessType = ProcessType.Zygote;
						break;
					case "gpu-process":
						_ProcessType = ProcessType.Gpu;
						break;
					case "utility":
						_ProcessType = ProcessType.Utility;
						break;
					case "ppapi":
						_ProcessType = ProcessType.PPApi;
						break;
					case "ppapi-broker":
						_ProcessType = ProcessType.PPApiBroker;
						break;
					case "nacl-loader":
						_ProcessType = ProcessType.NaClLoader;
						break;
					default:
						_ProcessType = ProcessType.Other;
						break;
				}
				return _ProcessType.Value;
			}
		}

		public override CefRenderProcessHandler GetRenderProcessHandler()
		{
			return AppGlue.RenderProcessGlue;
		}

		public static long GetCorsQueryFrame(long id)
		{
			long frameid;
			ManualResetEventSlim corsEvent = null;
			while (true)
			{
				lock (corsData)
				{
					if (corsData.TryGetValue(id, out frameid))
						break;
					corsEvent = new ManualResetEventSlim(false);
					corsQuery.Add(id, corsEvent);
				}
				corsEvent.Wait();
			}
			corsEvent?.Dispose();
			return frameid;
		}

		private static bool ProcessOnBrowserMessage(CefProcessMessageReceivedEventArgs e)
		{
			CefProcessMessage msg = e.Message;
			if (msg.Name == CefNetApplication.XrayResponseKey)
			{
				return ProcessXrayMessage(msg);
			}
			else if (msg.Name == "frameid-query")
			{
				return ProcessXrayCorsMessage(msg);
			}
			return false;
		}

		private static Dictionary<long, ManualResetEventSlim> corsQuery = new Dictionary<long, ManualResetEventSlim>();
		private static Dictionary<long, long> corsData = new Dictionary<long, long>();

		private static bool ProcessXrayCorsMessage(CefProcessMessage msg)
		{
			CefListValue args = msg.ArgumentList;
			long corsid = args.GetInt64(0);
			long frameid = args.GetInt64(1);
			lock (corsData)
			{
				corsData.Add(corsid, frameid);
				if (corsQuery.TryGetValue(corsid, out ManualResetEventSlim corsEvent))
				{
					corsEvent.Set();
				}
			}
			return true;
		}

		private static bool ProcessXrayMessage(CefProcessMessage msg)
		{
			CefListValue args = msg.ArgumentList;
			var sqi = ScriptableRequestInfo.Get(args.GetInt(0));
			if (sqi is null)
				return true;

			if (args.GetSize() == 2)
			{
				sqi.Complete(args.GetValue(1));
			}
			else
			{
				sqi.Complete(new CefNetRemoteException(args.GetString(1), args.GetString(2), args.GetString(3)));
			}
			return true;
		}

		private static void ProcessXrayRelease(CefProcessMessageReceivedEventArgs e)
		{
			using (var args = e.Message.ArgumentList)
			{
				if (args.GetType(0) != CefValueType.Binary)
					return;
				XrayHandle handle = XrayHandle.FromCfxBinaryValue(args.GetBinary(0));
				if ((int)(handle.frame >> 32) != (int)(e.Frame.Identifier >> 32))
					return; // Mismatch process IDs
				handle.Release();
			}
		}

		private static void ProcessXrayRequest(CefProcessMessageReceivedEventArgs e)
		{
			CefListValue args = e.Message.ArgumentList;

			CefProcessMessage message = new CefProcessMessage(XrayResponseKey);
			CefListValue retArgs = message.ArgumentList;
			retArgs.SetSize(2);
			retArgs.SetValue(0, args.GetValue(0));

			CefValue retval = null;
			XrayAction queryAction = (XrayAction)args.GetInt(1);

			try
			{
				CefV8Context v8Context;
				XrayObject target = null;

				if (queryAction == XrayAction.GetGlobal)
				{
					v8Context = e.Frame.V8Context;
				}
				else
				{
					target = XrayHandle.FromCfxBinaryValue(args.GetBinary(2)).GetTarget(e.Frame);
					v8Context = target?.Context ?? e.Frame.V8Context;
				}

				if (!v8Context.IsValid)
					throw new ObjectDeadException();
				if (!v8Context.Enter())
					throw new ObjectDeadException();
				try
				{
					CefV8Value rv = null;
					switch (queryAction)
					{
						case XrayAction.Get:
							long corsRid = ScriptableObjectProvider.Get(v8Context, target, args, out rv);
							if (corsRid != 0)
							{
								var xray = new XrayHandle();
								xray.dataType = XrayDataType.CorsRedirect;
								xray.iRaw = corsRid;
								retval = new CefValue();
								retval.SetBinary(xray.ToCfxBinaryValue());
								retArgs.SetValue(1, retval);
							}
							break;
						case XrayAction.Set:
							ScriptableObjectProvider.Set(v8Context, target, args);
							break;
						case XrayAction.InvokeMember:
							rv = ScriptableObjectProvider.InvokeMember(v8Context, target, args);
							break;
						case XrayAction.Invoke:
							rv = ScriptableObjectProvider.Invoke(v8Context, target, args);
							break;
						case XrayAction.GetGlobal:
							rv = v8Context.GetGlobal();
							break;
						default:
							throw new NotSupportedException();
					}
					if (rv != null)
					{
						retval = ScriptableObjectProvider.CastCefV8ValueToCefValue(v8Context, rv, out bool isXray);
						if (!isXray) rv.Dispose();
						retArgs.SetValue(1, retval);
					}
				}
				finally
				{
					v8Context.Exit();
				}
			}
			catch (AccessViolationException) { throw; }
			catch (OutOfMemoryException) { throw; }
			catch (Exception ex)
			{
				//File.AppendAllText("G:\\outlog.txt", ex.GetType().Name + ": " + ex.Message + "\r\n" + ex.StackTrace + "\r\n");
				retArgs.SetSize(4);
				retArgs.SetString(1, ex.Message);
				retArgs.SetString(2, ex.GetType().FullName);
				retArgs.SetString(3, ex.StackTrace);
			}



			//CfxValueType t = e.Message.ArgumentList.GetType(0);



			e.Frame.SendProcessMessage(CefProcessId.Browser, message);
			message.Dispose();
			retval?.Dispose();

			e.Handled = true;
		}


		#region CefRenderProcessHandler 

		protected internal virtual void OnRenderThreadCreated(CefListValue extraInfo)
		{

		}

		protected internal virtual void OnWebKitInitialized()
		{

		}

		/// <summary>
		/// Called after a browser has been created.
		/// </summary>
		/// <param name="browser">The browser instance.</param>
		/// <param name="extraInfo">A read-only value originating from the browser creator or null.</param>
		protected internal virtual void OnBrowserCreated(CefBrowser browser, CefDictionaryValue extraInfo)
		{

		}

		/// <summary>
		/// Called before a browser is destroyed. Release all references to the browser object
		/// and do not attempt to execute any methods on the browser object after this method returns.
		/// </summary>
		/// <param name="browser">The browser instance.</param>
		protected internal virtual void OnBrowserDestroyed(CefBrowser browser)
		{

		}

		protected internal virtual CefLoadHandler GetLoadHandler()
		{
			return null;
		}

		protected internal virtual void OnContextCreated(CefBrowser browser, CefFrame frame, CefV8Context context)
		{
			XrayObject.OnContextCreated(context);
		}

		protected internal virtual void OnContextReleased(CefBrowser browser, CefFrame frame, CefV8Context context)
		{
			XrayObject.OnContextReleased(context);
		}

		/// <summary>
		/// Called for global uncaught exceptions in a frame. Execution of this callback is disabled by default.
		/// To enable set CefSettings.UncaughtExceptionStackSize &gt; 0.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnUncaughtException(CefUncaughtExceptionEventArgs e)
		{
			CefUncaughtException?.Invoke(this, e);
		}

		protected internal virtual void OnFocusedNodeChanged(CefBrowser browser, CefFrame frame, CefDOMNode node)
		{

		}

		/// <summary>
		/// Called when a new message is received from a different process. Do not keep a
		/// reference to or attempt to access the message outside of this callback.
		/// </summary>
		protected internal virtual void OnCefProcessMessageReceived(CefProcessMessageReceivedEventArgs e)
		{
			if (e.SourceProcess == CefProcessId.Renderer)
			{
				e.Handled = ProcessOnBrowserMessage(e);
			}
			else if (e.SourceProcess == CefProcessId.Browser)
			{
				switch (e.Name)
				{
					case XrayReleaseKey:
						ProcessXrayRelease(e);
						return;
					case XrayRequestKey:
						ProcessXrayRequest(e);
						return;
				}
			}
			if (!e.Handled)
			{
				CefProcessMessageReceived?.Invoke(this, e);
			}
		}

		#endregion
	}
}
