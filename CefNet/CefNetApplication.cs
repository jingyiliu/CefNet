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
	/// <summary>
	/// Provides methods and properties to manage an application, such as methods to start and stop
	/// an application, to process messages, and properties to get information about an application.
	/// </summary>
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

		/// <summary>
		/// Occurs after WebKit has been initialized.
		/// </summary>
		public event EventHandler WebKitInitialized;

		/// <summary>
		/// Occurs immediately after the CEF context has been initialized.
		/// </summary>
		public event EventHandler CefContextInitialized;

		/// <summary>
		/// Occurs before a child process is launched.
		/// </summary>
		public event EventHandler<EventArgs> BeforeChildProcessLaunch;

		/// <summary>
		/// Occurs after the render process main thread has been created.
		/// </summary>
		public event EventHandler<RenderThreadCreatedEventArgs> RenderThreadCreated;

		private static ProcessType? _ProcessType;
		private int _initThreadId;

		static CefNetApplication()
		{
			using (Process process = Process.GetCurrentProcess())
			{
				AllowDotnetProcess = "dotnet".Equals(process.ProcessName, StringComparison.Ordinal);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CefNetApplication"/> class.
		/// </summary>
		public CefNetApplication()
		{
			AppGlue = new CefAppGlue(this);
		}

		private CefAppGlue AppGlue { get; }

		/// <summary>
		/// Gets the current <see cref="CefNetApplication"/>.
		/// </summary>
		public static CefNetApplication Instance { get; private set; }

		/// <summary>
		/// Gets a value that indicates whether the <see cref="CefNetApplication"/> is initialized.
		/// </summary>
		public static bool IsInitialized
		{
			get { return Instance != null; }
		}

		/// <summary>
		/// Gets and sets a value indicating that the dotnet can be used to run this application,
		/// by specifying an application DLL, such as &apos;dotnet myapp.dll&apos;.
		/// </summary>
		public static bool AllowDotnetProcess { get; set; }

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

		/// <summary>
		/// Shuts down a CEF application.
		/// </summary>
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

		/// <summary>
		/// Returns a handler for functionality specific to the render process.
		/// </summary>
		/// <returns>A handler for functionality specific to the render process.</returns>
		public override CefRenderProcessHandler GetRenderProcessHandler()
		{
			return AppGlue.RenderProcessGlue;
		}

		/// <summary>
		/// Returns the handler for functionality specific to the browser process.<para/>
		/// This function is called on multiple threads in the browser process.
		/// </summary>
		/// <returns>A handler for functionality specific to the browser process.</returns>
		public override CefBrowserProcessHandler GetBrowserProcessHandler()
		{
			return AppGlue.BrowserProcessGlue;
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

		/// <summary>
		/// Raises the <see cref="RenderThreadCreated"/> event.<para/>
		/// Called after the render process main thread has been created. In the browser process called on the IO thread.
		/// </summary>
		/// <param name="e">A <see cref="RenderThreadCreatedEventArgs"/> that contains the event data.</param>
		protected internal virtual void OnRenderThreadCreated(RenderThreadCreatedEventArgs e)
		{
			RenderThreadCreated?.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="WebKitInitialized"/> event.
		/// </summary>
		protected internal virtual void OnWebKitInitialized()
		{
			WebKitInitialized?.Invoke(this, EventArgs.Empty);
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

		/// <summary>
		/// Return a handler for browser load status events.
		/// </summary>
		/// <returns>The handler for browser load status events.</returns>
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
				if (e.Name == XrayResponseKey)
				{
					e.Handled = ProcessXrayMessage(e.Message);
				}
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


		/// <summary>
		/// Raises the <see cref="CefContextInitialized"/> event.<para/>
		/// Called on the browser process UI thread immediately after the CEF context has been initialized.
		/// </summary>
		protected internal virtual void OnContextInitialized()
		{
			CefContextInitialized?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the <see cref="BeforeChildProcessLaunch"/> event.<para/>
		/// Will be called on the browser process UI thread when launching a render process
		/// and on the browser process IO thread when launching a GPU or plugin process.
		/// </summary>
		/// <param name="e">A <see cref="BeforeChildProcessLaunchEventArgs"/> that contains the event data.</param>
		protected internal virtual void OnBeforeChildProcessLaunch(BeforeChildProcessLaunchEventArgs e)
		{
			if (AllowDotnetProcess)
			{
				e.CommandLine.Program = Environment.GetCommandLineArgs()[0];
				e.CommandLine.PrependWrapper("dotnet");
			}
			BeforeChildProcessLaunch?.Invoke(this, e);
		}

		/// <summary>
		/// Return the handler for printing on Linux.<para/>
		/// If a print handler is not provided then printing will not be supported on the Linux platform.
		/// </summary>
		/// <returns>The handler for printing.</returns>
		public virtual CefPrintHandler GetPrintHandler()
		{
			return null;
		}

		/// <summary>
		/// Called from any thread when work has been scheduled for the browser process main (UI) thread.<para/>
		/// This callback should schedule a <see cref="CefApi.DoMessageLoopWork"/> call to happen on the main (UI) thread. 
		/// </summary>
		/// <param name="delayMs">
		/// The requested delay in milliseconds.<para/>If <paramref name="delayMs"/> is &lt;= 0
		/// then the call should happen reasonably soon; otherwise the call should be scheduled
		/// to happen after the specified delay and any currently pending scheduled call should
		/// be cancelled.
		/// </param>
		/// <remarks>
		/// This callback is used in combination with <see cref="CefSettings.ExternalMessagePump"/>
		/// and <see cref="CefApi.DoMessageLoopWork"/> in cases where the CEF message loop must be
		/// integrated into an existing application message loop.
		/// </remarks>
		protected internal virtual void OnScheduleMessagePumpWork(long delayMs)
		{

		}

	}
}
