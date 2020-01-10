#if True
using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using CefNet.WinApi;
using CefNet.CApi;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace CefNet
{
	public static unsafe class CefApi
	{
		internal static bool UseUnsafeImplementation = true;

		/// <summary>
		/// Gets compatible CEF API hash.
		/// </summary>
		public static string ApiHash
		{
			get
			{
				return CefNativeApi.ApiHash;
			}
		}

		/// <summary>
		/// Create a new browser window using the window parameters specified by
		/// |windowInfo|. All values will be copied internally and the actual window will
		/// be created on the UI thread. This function can be called on any browser process
		/// thread and will not block.
		/// </summary>
		/// <param name="requestContext">
		/// If |requestContext| is NULL the global request context will be used.
		/// </param>
		/// <param name="extraInfo">
		/// The optional |extraInfo| parameter provides an opportunity to specify extra
		/// information specific to the created browser that will be passed to
		/// CefRenderProcessHandler.OnBrowserCreated() in the render process.
		/// </param>
		public static bool CreateBrowser(CefWindowInfo windowInfo, CefClient client, string url, CefBrowserSettings settings, CefDictionaryValue extraInfo, CefRequestContext requestContext)
		{
			if (windowInfo == null)
				throw new ArgumentNullException(nameof(windowInfo));
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			int rv;
			fixed (char* s = url)
			{
				var aUrl = new cef_string_t() { Str = s, Length = (url != null ? url.Length : 0) };
				rv = CefNativeApi.cef_browser_host_create_browser(
					windowInfo.GetNativeInstance(),
					client != null ? client.GetNativeInstance() : null,
					&aUrl,
					settings.GetNativeInstance(),
					extraInfo != null ? extraInfo.GetNativeInstance() : null,
					requestContext != null ? requestContext.GetNativeInstance() : null
				);
			}
			GC.KeepAlive(windowInfo);
			GC.KeepAlive(settings);
			return rv != 0;
		}

		/// <summary>
		/// Create a new browser window using the window parameters specified by
		/// |windowInfo|. If |request_context| is NULL the global request context will be
		/// used. This function can only be called on the browser process UI thread.
		/// </summary>
		/// <param name="requestContext">
		/// If |request_context| is NULL the global request context will be used.
		/// </param>
		/// <param name="extra_info">
		/// The optional |extra_info| parameter provides an opportunity to specify extra
		/// information specific to the created browser that will be passed to
		/// CefRenderProcessHandler.OnBrowserCreated() in the render process.
		/// </param>
		public static CefBrowser CreateBrowserSync(CefWindowInfo windowInfo, CefClient client, string url, CefBrowserSettings settings, CefDictionaryValue extra_info, CefRequestContext requestContext)
		{
			if (windowInfo == null)
				throw new ArgumentNullException(nameof(windowInfo));
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			cef_browser_t* rv;
			fixed (char* s = url)
			{
				var aUrl = new cef_string_t() { Str = s, Length = (url != null ? url.Length : 0) };
				rv = CefNativeApi.cef_browser_host_create_browser_sync(
					windowInfo.GetNativeInstance(),
					client != null ? client.GetNativeInstance() : null,
					&aUrl,
					settings.GetNativeInstance(),
					extra_info != null ? extra_info.GetNativeInstance() : null,
					requestContext != null ? requestContext.GetNativeInstance() : null
				);
			}
			GC.KeepAlive(settings);
			return CefBrowser.Wrap(CefBrowser.Create, rv);
		}

		/// <summary>
		/// Returns True if called on the specified thread.
		/// </summary>
		public static bool CurrentlyOn(CefThreadId threadId)
		{
			return CefNativeApi.cef_currently_on(threadId) != 0;
		}

		/// <summary>
		/// Post a task for execution on the specified thread. This function may be
		/// called on any thread. It is an error to request a thread from the wrong
		/// process.
		/// </summary>
		public static bool PostTask(CefThreadId threadId, CefTask task)
		{
			if (task == null)
				throw new ArgumentNullException(nameof(task));
			return CefNativeApi.cef_post_task(threadId, task.GetNativeInstance()) != 0;
		}

		/// <summary>
		/// Post a task for execution on the specified thread. This function may be
		/// called on any thread. It is an error to request a thread from the wrong
		/// process.
		/// </summary>
		public static bool PostTask(CefThreadId threadId, CefTask task, long delay)
		{
			if (task == null)
				throw new ArgumentNullException(nameof(task));
			return CefNativeApi.cef_post_delayed_task(threadId, task.GetNativeInstance(), delay) != 0;
		}

		/// <summary>
		/// Register a new V8 extension with the specified JavaScript extension code and
		/// handler. Functions implemented by the handler are prototyped using the
		/// keyword &apos;native&apos;. The calling of a native function is restricted to the scope
		/// in which the prototype of the native function is defined. This function may
		/// only be called on the render process main thread.
		/// </summary>
		public static bool RegisterExtension(string name, string jscode, CefV8Handler handler)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (jscode == null)
				throw new ArgumentNullException(nameof(jscode));

			fixed (char* sName = name)
			fixed (char* sCode = jscode)
			{
				var aName = new cef_string_t { Str = sName, Length = name.Length };
				var aCode = new cef_string_t { Str = sCode, Length = jscode.Length };
				return CefNativeApi.cef_register_extension(&aName, &aCode, handler != null ? handler.GetNativeInstance() : null) != 0;
			}

		}

		/// <summary>
		/// Register a scheme handler factory with the global request context. This
		/// function may be called multiple times to change or remove the factory that
		/// matches the specified |schemeName| and optional |domainName|. Returns False
		/// if an error occurs. This function may be called on any thread in the
		/// browser process.
		/// </summary>
		/// <param name="schemeName">
		/// If |schemeName| is a built-in scheme and no handler is returned by
		/// |factory| then the built-in scheme handler factory will be called. If
		/// |schemeName| is a custom scheme then you must also implement the
		/// CefApp::OnRegisterCustomSchemes() function in all processes.
		/// </param>
		/// <param name="domainName">
		/// An NULL value for a standard scheme will cause the factory to match all
		/// domain names. The domainName value will be ignored for non-standard schemes.
		/// </param>
		public static bool RegisterSchemeHandlerFactory(string schemeName, string domainName, CefSchemeHandlerFactory factory)
		{
			if (string.IsNullOrWhiteSpace(schemeName))
				throw new ArgumentOutOfRangeException(nameof(schemeName));
			if (factory == null)
				throw new ArgumentNullException(nameof(factory));

			fixed (char* s0 = schemeName)
			fixed (char* s1 = domainName)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = schemeName.Length };
				var cstr1 = new cef_string_t { Str = s1, Length = domainName.Length };
				return CefNativeApi.cef_register_scheme_handler_factory(&cstr0, &cstr1, factory.GetNativeInstance()) != 0;
			}
		}

		/// <summary>
		/// Clear all scheme handler factories registered with the global request
		/// context. Returns False on error. This function may be called on any
		/// thread in the browser process.
		/// </summary>
		public static bool ClearSchemeHandlerFactories()
		{
			return CefNativeApi.cef_clear_scheme_handler_factories() != 0;
		}

		/// <summary>
		/// This function should be called from the application entry point function to
		/// execute a secondary process. It can be used to run secondary processes from
		/// the browser client executable (default behavior) or from a separate
		/// executable specified by the CefSettings.BrowserSubprocessPath value. If
		/// called for the browser process (identified by no &quot;type&quot; command-line value)
		/// it will return immediately with a value of -1. If called for a recognized
		/// secondary process it will block until the process should exit and then return
		/// the process exit code.
		/// </summary>
		/// <param name="application">
		/// The |application| parameter may be NULL. 
		/// </param>
		/// <param name="windowsSandboxInfo">
		/// This parameter is only used on Windows and may be NULL (see cef_sandbox_win.h for details).
		/// </param>
		public static int ExecuteProcess(CefMainArgs args, CefApp application, IntPtr windowsSandboxInfo)
		{
			return CefNativeApi.cef_execute_process((cef_main_args_t*)&args, application != null ? application.GetNativeInstance() : null, (void*)windowsSandboxInfo);
		}

		/// <summary>
		/// This function should be called on the main application thread to initialize
		/// the CEF browser process. A return value of True indicates that it succeeded
		/// and False indicates that it failed.
		/// </summary>
		/// <param name="application">
		/// The |application| parameter may be NULL.
		/// </param>
		/// <param name="windowsSandboxInfo">
		/// This parameter is only used on Windows and may be NULL (see cef_sandbox_win.h for details).
		/// </param>
		public static bool Initialize(CefMainArgs args, CefSettings settings, CefApp application, IntPtr windowsSandboxInfo)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));
			bool rv = CefNativeApi.cef_initialize((cef_main_args_t*)&args, settings.GetNativeInstance(), application != null ? application.GetNativeInstance() : null, (void*)windowsSandboxInfo) != 0;
			GC.KeepAlive(settings);
			GC.KeepAlive(application);
			return rv;
		}

		/// <summary>
		/// This function should be called on the main application thread to shut down
		/// the CEF browser process before the application exits.
		/// </summary>
		public static void Shutdown()
		{
			CefNativeApi.cef_shutdown();
		}

		/// <summary>
		/// Perform a single iteration of CEF message loop processing. This function is
		/// provided for cases where the CEF message loop must be integrated into an
		/// existing application message loop. Use of this function is not recommended
		/// for most users; use either the CefRunMessageLoop() function or
		/// CefSettings.MultiThreadedMessageLoop if possible. When using this function
		/// care must be taken to balance performance against excessive CPU usage. It is
		/// recommended to enable the CefSettings.ExternalMessagePump option when using
		/// this function so that CefBrowserProcessHandler::OnScheduleMessagePumpWork()
		/// callbacks can facilitate the scheduling process. This function should only be
		/// called on the main application thread and only if CefInitialize() is called
		/// with a CefSettings.MultiThreadedMessageLoop value of False. This function
		/// will not block.
		/// </summary>
		public static void DoMessageLoopWork()
		{
			CefNativeApi.cef_do_message_loop_work();
		}

		/// <summary>
		/// Run the CEF message loop. Use this function instead of an application-
		/// provided message loop to get the best balance between performance and CPU
		/// usage. This function should only be called on the main application thread and
		/// only if CefInitialize() is called with a CefSettings.MultiThreadedMessageLoop
		/// value of False. This function will block until a quit message is received by
		/// the system.
		/// </summary>
		public static void RunMessageLoop()
		{
			CefNativeApi.cef_run_message_loop();
		}

		/// <summary>
		/// Quit the CEF message loop that was started by calling CefRunMessageLoop().
		/// This function should only be called on the main application thread and only
		/// if CefRunMessageLoop() was used.
		/// </summary>
		public static void QuitMessageLoop()
		{
			CefNativeApi.cef_quit_message_loop();
		}

		/// <summary>
		/// Set to True before calling Windows APIs like TrackPopupMenu that enter a
		/// modal message loop. Set to False after exiting the modal message loop.
		/// </summary>
		public static void SetOSModalLoop(bool osModalLoop)
		{
			CefNativeApi.cef_set_osmodal_loop(osModalLoop ? 1 : 0);
		}

		/// <summary>
		/// Call during process startup to enable High-DPI support on Windows 7 or newer.
		/// Older versions of Windows should be left DPI-unaware because they do not
		/// support DirectWrite and GDI fonts are kerned very badly.
		/// </summary>
		public static void EnableHighDPISupport()
		{
			CefNativeApi.cef_enable_highdpi_support();
		}

		/// <summary>
		/// Returns True if the certificate status has any error, major or minor.
		/// </summary>
		public static bool IsCertStatusError(CefCertStatus status)
		{
			return CefNativeApi.cef_is_cert_status_error(status) != 0;
		}

		/// <summary>
		/// Returns True if the certificate status represents only minor errors (e.g.
		/// failure to verify certificate revocation).
		/// </summary>
		public static bool IsCertStatusMinorError(CefCertStatus status)
		{
			return CefNativeApi.cef_is_cert_status_minor_error(status) != 0;
		}

		/// <summary>
		/// Crash reporting is configured using an INI-style config file named
		/// &quot;crash_reporter.cfg&quot;. On Windows and Linux this file must be placed next to
		/// the main application executable. On macOS this file must be placed in the
		/// top-level app bundle Resources directory
		/// (e.g. &quot;&lt;appname&gt;.app/Contents/Resources&quot;).
		/// </summary>
		public static bool CrashReportingEnabled
		{
			get { return CefNativeApi.cef_crash_reporting_enabled() != 0; }
		}

		/// <summary>
		/// Sets or clears a specific key-value pair from the crash metadata.
		/// </summary>
		public static void SetCrashKeyValue(string key, string value)
		{
			if (string.IsNullOrWhiteSpace(key))
				throw new ArgumentOutOfRangeException(nameof(key));

			fixed (char* s0 = key)
			fixed (char* s1 = value)
			{
				var aKey = new cef_string_t { Str = s0, Length = value.Length };
				var aValue = new cef_string_t { Str = s1, Length = (value != null ? value.Length : 0) };
				CefNativeApi.cef_set_crash_key_value(&aKey, &aValue);
			}
		}

		/// <summary>
		/// Creates a directory and all parent directories if they don&apos;t already exist.
		/// Returns True on successful creation or if the directory already exists.
		/// The directory is only readable by the current user. Calling this function on
		/// the browser process UI or IO threads is not allowed.
		/// </summary>
		public static bool CreateDirectory(string fullPath)
		{
			if (string.IsNullOrWhiteSpace(fullPath))
				throw new ArgumentOutOfRangeException(nameof(fullPath));

			fixed (char* s0 = fullPath)
			{
				var path = new cef_string_t { Str = s0, Length = fullPath.Length };
				return CefNativeApi.cef_create_directory(&path) != 0;
			}
		}

		/// <summary>
		/// Get the temporary directory provided by the system.
		/// WARNING: In general, you should use the temp directory variants below instead
		/// of this function. Those variants will ensure that the proper permissions are
		/// set so that other users on the system can&apos;t edit them while they&apos;re open
		/// (which could lead to security issues). Returns null if an error occurs.
		/// </summary>
		public static string GetTempDirectory()
		{
			var path = new cef_string_t();
			if (CefNativeApi.cef_get_temp_directory(&path) != 0)
			{
				return CefString.ReadAndFree(&path);
			}
			return null;
		}

		/// <summary>
		/// Creates a new directory. On Windows if |prefix| is provided the new directory
		/// name is in the format of &quot;prefixyyyy&quot;. Returns the full path of the
		/// directory that was created or null if an error occurs. The directory is only
		/// readable by the current user. Calling this function on the browser process UI
		/// or IO threads is not allowed.
		/// </summary>
		public static string CreateNewTempDirectory(string prefix)
		{
			fixed (char* s0 = prefix)
			{
				var path = new cef_string_t();
				var cstr = new cef_string_t { Str = s0, Length = (prefix != null ? prefix.Length : 0) };
				if (CefNativeApi.cef_create_new_temp_directory(&cstr, &path) != 0)
				{
					return CefString.ReadAndFree(&path);
				}
			}
			return null;
		}

		/// <summary>
		/// Creates a directory within another directory. Extra characters will be
		/// appended to |prefix| to ensure that the new directory does not have the same
		/// name as an existing directory. Returns the full path of the directory that
		/// was created or null if an error occurs. The directory is only readable by the
		/// current user. Calling this function on the browser process UI or IO threads
		/// is not allowed.
		/// </summary>
		public static string CreateTempDirectoryInDirectory(string basePath, string prefix)
		{
			if (string.IsNullOrWhiteSpace(basePath))
				throw new ArgumentOutOfRangeException(nameof(basePath));

			fixed (char* s0 = basePath)
			fixed (char* s1 = prefix)
			{
				var path = new cef_string_t();
				var cstr0 = new cef_string_t { Str = s0, Length = basePath.Length };
				var cstr1 = new cef_string_t { Str = s1, Length = (prefix != null ? prefix.Length : 0) };
				if (CefNativeApi.cef_create_temp_directory_in_directory(&cstr0, &cstr1, &path) != 0)
				{
					return CefString.ReadAndFree(&path);
				}
			}
			return null;
		}

		/// <summary>
		/// Returns True if the given path exists and is a directory. Calling this
		/// function on the browser process UI or IO threads is not allowed.
		/// </summary>
		public static bool DirectoryExists(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentOutOfRangeException(nameof(path));
			fixed (char* s0 = path)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = path.Length };
				return CefNativeApi.cef_directory_exists(&cstr0) != 0;
			}
		}

		/// <summary>
		/// Deletes the given path whether it&apos;s a file or a directory. Returns True on successful
		/// deletion or if |path| does not exist. Calling this function on the browser process UI or
		/// IO threads is not allowed.
		/// </summary>
		/// <param name="path">
		/// If |path| is a directory all contents will be deleted.
		/// </param>
		/// <param name="recursive">
		/// If True any sub directories and their contents will also be deleted (equivalent to executing
		/// &quot;rm -rf&quot;, so use with caution). On POSIX environments if |path| is a symbolic
		/// link then only the symlink will be deleted.
		/// </param>
		public static bool DeleteFile(string path, bool recursive)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			fixed (char* s0 = path)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = path.Length };
				return CefNativeApi.cef_delete_file(&cstr0, recursive ? 1 : 0) != 0;
			}
		}

		/// <summary>
		/// Writes the contents of |sourceDirectory| into a zip archive at |destinationFile|.
		/// Returns True on success. Calling this function on the browser process UI
		/// or IO threads is not allowed.
		/// </summary>
		/// <param name="includeHiddenFiles">
		/// If True then files starting with &quot;.&quot; will be included.
		/// </param>
		public static bool ZipDirectory(string sourceDirectory, string destinationFile, bool includeHiddenFiles)
		{
			if (string.IsNullOrWhiteSpace(sourceDirectory))
				throw new ArgumentOutOfRangeException(nameof(sourceDirectory));
			if (string.IsNullOrWhiteSpace(destinationFile))
				throw new ArgumentOutOfRangeException(nameof(destinationFile));

			fixed (char* s0 = sourceDirectory)
			fixed (char* s1 = destinationFile)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = sourceDirectory.Length };
				var cstr1 = new cef_string_t { Str = s1, Length = destinationFile.Length };
				return CefNativeApi.cef_zip_directory(&cstr0, &cstr1, includeHiddenFiles ? 1 : 0) != 0;
			}
		}

		/// <summary>
		/// Loads the existing &quot;Certificate Revocation Lists&quot; file that is managed
		/// by Google Chrome. This file can generally be found in Chrome&apos;s User Data
		/// directory (e.g. &quot;%LOCALAPPDATA%\Google\Chrome\User Data&quot; on Windows)
		/// and is updated periodically by Chrome&apos;s component updater service.
		/// Must be called in the browser process after the context has been initialized.
		/// See https://dev.chromium.org/Home/chromium-security/crlsets for background.
		/// </summary>
		public static void LoadClrlSetsFile(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException(null, path);

			fixed (char* s0 = path)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = path.Length };
				CefNativeApi.cef_load_crlsets_file(&cstr0);
			}
		}

		/// <summary>
		/// Add an entry to the cross-origin access whitelist.
		/// The same-origin policy restricts how scripts hosted from different origins
		/// (scheme + domain + port) can communicate. By default, scripts can only access
		/// resources with the same origin. Scripts hosted on the HTTP and HTTPS schemes
		/// (but no other schemes) can use the &quot;Access-Control-Allow-Origin&quot; header to
		/// allow cross-origin requests. For example, https://source.example.com can make
		/// XMLHttpRequest requests on http://target.example.com if the
		/// http://target.example.com request returns an &quot;Access-Control-Allow-Origin:
		/// https://source.example.com&quot; response header.
		/// Scripts in separate frames or iframes and hosted from the same protocol and
		/// domain suffix can execute cross-origin JavaScript if both pages set the
		/// document.domain value to the same domain suffix. For example,
		/// scheme://foo.example.com and scheme://bar.example.com can communicate using
		/// JavaScript if both domains set document.domain=&quot;example.com&quot;.
		/// This function is used to allow access to origins that would otherwise violate
		/// the same-origin policy. Scripts hosted underneath the fully qualified
		/// |sourceOrigin| URL (like http://www.example.com) will be allowed access to
		/// all resources hosted on the specified |targetProtocol| and |targetDomain|.
		/// This function cannot be used to bypass the restrictions on local or display
		/// isolated schemes. See the comments on CefRegisterCustomScheme for more
		/// information.
		/// This function may be called on any thread. Returns False if
		/// |sourceOrigin| is invalid or the whitelist cannot be accessed.
		/// </summary>
		/// <param name="targetDomain">
		/// If |targetDomain| is non-NULL and |allowTargetSubdomains| if False
		/// only exact domain matches will be allowed. If |targetDomain| contains a top-
		/// level domain component (like &quot;example.com&quot;) and |allowTargetSubdomains| is
		/// True sub-domain matches will be allowed. If |targetDomain| is NULL and
		/// |allowTargetSubdomains| if True all domains and IP addresses will be
		/// allowed.
		/// </param>
		public static bool AddCrossOriginWhitelistEntry(string sourceOrigin, string targetProtocol, string targetDomain, bool allowTargetSubdomains)
		{
			if (sourceOrigin == null)
				throw new ArgumentNullException(nameof(sourceOrigin));
			if (targetProtocol == null)
				throw new ArgumentNullException(nameof(targetProtocol));

			fixed (char* s0 = sourceOrigin)
			fixed (char* s1 = targetProtocol)
			fixed (char* s2 = targetDomain)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = sourceOrigin.Length };
				var cstr1 = new cef_string_t { Str = s1, Length = targetProtocol.Length };
				var cstr2 = new cef_string_t { Str = s2, Length = (targetDomain != null ? targetDomain.Length : 0) };
				return CefNativeApi.cef_add_cross_origin_whitelist_entry(&cstr0, &cstr1, &cstr2, allowTargetSubdomains ? 1 : 0) != 0;
			}
		}

		/// <summary>
		/// Remove an entry from the cross-origin access whitelist. Returns False if
		/// |source_origin| is invalid or the whitelist cannot be accessed.
		/// </summary>
		public static bool RemoveCrossOriginWhitelistEntry(string sourceOrigin, string targetProtocol, string targetDomain, bool allowTargetSubdomains)
		{
			if (sourceOrigin == null)
				throw new ArgumentNullException(nameof(sourceOrigin));
			if (targetProtocol == null)
				throw new ArgumentNullException(nameof(targetProtocol));

			fixed (char* s0 = sourceOrigin)
			fixed (char* s1 = targetProtocol)
			fixed (char* s2 = targetDomain)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = sourceOrigin.Length };
				var cstr1 = new cef_string_t { Str = s1, Length = targetProtocol.Length };
				var cstr2 = new cef_string_t { Str = s2, Length = (targetDomain != null ? targetDomain.Length : 0) };
				return CefNativeApi.cef_remove_cross_origin_whitelist_entry(&cstr0, &cstr1, &cstr2, allowTargetSubdomains ? 1 : 0) != 0;
			}
		}

		/// <summary>
		/// Remove all entries from the cross-origin access whitelist. Returns False
		/// if the whitelist cannot be accessed.
		/// </summary>
		public static bool ClearCrossOriginWhitelist()
		{
			return CefNativeApi.cef_clear_cross_origin_whitelist() != 0;
		}

		/// <summary>
		/// Parse the specified |url| into its component parts.
		/// </summary>
		public static CefUrlParts ParseUrl(string url)
		{
			if (url == null)
				throw new ArgumentNullException(nameof(url));

			var url_parts = new CefUrlParts();
			fixed (char* s0 = url)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = url.Length };
				if (CefNativeApi.cef_parse_url(&cstr0, (cef_urlparts_t*)&url_parts) != 0)
				{
					return url_parts;
				}
			}
			throw new UriFormatException();
		}

		/// <summary>
		/// Creates a URL from the specified |parts|, which must contain a non-NULL spec
		/// or a non-NULL host and path (at a minimum), but not both. Returns NULL if
		/// |parts| isn&apos;t initialized as described.
		/// </summary>
		public static string CreateUrl(CefUrlParts parts)
		{
			var s = new cef_string_t();
			CefNativeApi.cef_create_url((cef_urlparts_t*)&parts, &s);
			return CefString.ReadAndFree(&s);
		}

		/// <summary>
		/// This is a convenience function for formatting a URL in a concise and human-
		/// friendly way to help users make security-related decisions (or in other
		/// circumstances when people need to distinguish sites, origins, or otherwise-
		/// simplified URLs from each other). Internationalized domain names (IDN) may be
		/// presented in Unicode if the conversion is considered safe. The returned value
		/// will (a) omit the path for standard schemes, excepting file and filesystem,
		/// and (b) omit the port if it is the default for the scheme. Do not use this
		/// for URLs which will be parsed or sent to other applications.
		/// </summary>
		public static string FormatUrlForSecurityDisplay(string originUrl)
		{
			fixed (char* s0 = originUrl)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = (originUrl != null ? originUrl.Length : 0) };
				return CefString.ReadAndFree(CefNativeApi.cef_format_url_for_security_display(&cstr0));
			}
		}

		/// <summary>
		/// Returns the mime type for the specified file extension or an NULL string if unknown.
		/// </summary>
		public static string GetMimeType(string extension)
		{
			fixed (char* s0 = extension)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = (extension != null ? extension.Length : 0) };
				return CefString.ReadAndFree(CefNativeApi.cef_get_mime_type(&cstr0));
			}
		}

		/// <summary>
		/// Get the extensions associated with the given mime type. This should be passed
		/// in lower case. There could be multiple extensions for a given mime type, like
		/// &quot;html,htm&quot; for &quot;text/html&quot;, or &quot;txt,text,html,...&quot;
		/// for &quot;text/*&quot;. Any existing elements in the provided vector will not
		/// be erased.
		/// </summary>
		public static string[] GetExtensionsForMimeType(string mimeType)
		{
			using (var list = new CefStringList())
			{
				fixed (char* s0 = mimeType)
				{
					var cstr0 = new cef_string_t { Str = s0, Length = (mimeType != null ? mimeType.Length : 0) };
					CefNativeApi.cef_get_extensions_for_mime_type(&cstr0, list.GetNativeInstance());
				}
				return list.ToArray();
			}
		}

		/// <summary>
		/// Escapes characters in |text| which are unsuitable for use as a query
		/// parameter value. Everything except alphanumerics and -_.!~*&apos;()
		/// will be converted to &quot;%XX&quot;. The result is basically the same
		/// as encodeURIComponent in Javacript.
		/// </summary>
		/// <param name="usePlus">
		/// If |use_plus| is True spaces will change to &quot;+&quot;.
		/// </param>
		public static string UriEncode(string text, bool usePlus)
		{
			if (text == null)
				return null;

			fixed (char* s0 = text)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = text.Length };
				return CefString.ReadAndFree(CefNativeApi.cef_uriencode(&cstr0, usePlus ? 1 : 0));
			}
		}

		/// <summary>
		/// Unescapes |text| and returns the result. Unescaping consists of looking for
		/// the exact pattern &quot;%XX&quot; where each X is a hex digit and converting to the
		/// character with the numerical value of those digits (e.g. &quot;i%20=%203%3b&quot;
		/// unescapes to &quot;i = 3;&quot;).
		/// </summary>
		/// <param name="convertToUtf8">
		/// If |convertToUtf8| is True this function will attempt to interpret the initial
		/// decoded result as UTF-8. If the result is convertable into UTF-8 it will be
		/// returned as converted. Otherwise the initial decoded result will be returned.
		/// </param>
		/// <param name="unescapeRule">
		/// The |rule| parameter supports further customization the decoding process.
		/// </param>
		public static string UriDecode(string text, bool convertToUtf8, CefUriUnescapeRule rule)
		{
			if (text == null)
				return null;

			fixed (char* s0 = text)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = text.Length };
				return CefString.ReadAndFree(CefNativeApi.cef_uridecode(&cstr0, convertToUtf8 ? 1 : 0, rule));
			}
		}

		/// <summary>
		/// Parses the specified |json| string and returns a dictionary or list
		/// representation. If JSON parsing fails this function returns NULL.
		/// </summary>
		public static CefValue CefParseJSON(string json, CefJsonParserOptions options)
		{
			if (json == null)
				return null;

			fixed (char* s0 = json)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = json.Length };
				return CefValue.Wrap(CefValue.Create, CefNativeApi.cef_parse_json(&cstr0, options));
			}
		}

		/// <summary>
		/// Parses the specified |json| string and returns a dictionary or list
		/// representation. If JSON parsing fails this function returns NULL and
		/// populates |errorCode| and |errorMessage| with an error code and a
		/// formatted error message respectively.
		/// </summary>
		public static CefValue CefParseJSON(string json, CefJsonParserOptions options, out CefJsonParserError errorCode, out string errorMessage)
		{
			if (json == null)
			{
				errorCode = CefJsonParserError.UnexpectedToken;
				errorMessage = "Line: 1, column: 1, Unexpected token.";
				return null;
			}

			fixed (char* s0 = json)
			fixed (CefJsonParserError* p1 = &errorCode)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = json.Length };
				var cstr1 = new cef_string_t();
				CefValue rv = CefValue.Wrap(CefValue.Create, CefNativeApi.cef_parse_jsonand_return_error(&cstr0, options, p1, &cstr1));
				errorMessage = CefString.ReadAndFree(&cstr1);
				return rv;
			}
		}

		/// <summary>
		/// Generates a JSON string from the specified root |node| which should be a
		/// dictionary or list value. Returns an NULL string on failure. This function
		/// requires exclusive access to |node| including any underlying data.
		/// </summary>
		public static string CefWriteJSON(CefValue node, CefJsonWriterOptions options)
		{
			if (node == null)
				return null;

			return CefString.ReadAndFree(CefNativeApi.cef_write_json(node.GetNativeInstance(), options));
		}

		/// <summary>
		/// Retrieve the path associated with the specified |key|. Returns an NULL string
		/// on failure. Can be called on any thread in the browser process.
		/// </summary>
		public static string GetPath(CefPathKey key)
		{
			var path = new cef_string_t();
			if (CefNativeApi.cef_get_path(key, &path) != 0)
			{
				return CefString.ReadAndFree(&path);
			}
			return null;
		}

		/// <summary>
		/// Launches the process specified via |command_line|. Returns True upon
		/// success. Must be called on the browser process TID_PROCESS_LAUNCHER thread.
		/// Unix-specific notes: - All file descriptors open in the parent process will
		/// be closed in the
		/// child process except for stdin, stdout, and stderr.
		/// - If the first argument on the command line does not contain a slash,
		/// PATH will be searched. (See man execvp.)
		/// </summary>
		public static bool CefLaunchProcess(CefCommandLine commandLine)
		{
			if (commandLine == null)
				throw new ArgumentNullException(nameof(commandLine));

			return CefNativeApi.cef_launch_process(commandLine.GetNativeInstance()) != 0;
		}

		/// <summary>
		/// Visit web plugin information. Can be called on any thread in the browser
		/// process.
		/// </summary>
		public static void VisitWebPluginInfo(CefWebPluginInfoVisitor visitor)
		{
			if (visitor == null)
				throw new ArgumentNullException(nameof(visitor));

			CefNativeApi.cef_visit_web_plugin_info(visitor.GetNativeInstance());
		}

		/// <summary>
		/// Cause the plugin list to refresh the next time it is accessed regardless of
		/// whether it has already been loaded. Can be called on any thread in the
		/// browser process.
		/// </summary>
		public static void RefreshWebPlugins()
		{
			CefNativeApi.cef_refresh_web_plugins();
		}

		/// <summary>
		/// Unregister an internal plugin. This may be undone the next time
		/// RefreshWebPlugins() is called. Can be called on any thread in the
		/// browser process.
		/// </summary>
		public static void UnregisterInternalWebPlugin(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));

			fixed (char* s0 = path)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = path.Length };
				CefNativeApi.cef_unregister_internal_web_plugin(&cstr0);
			}
		}

		/// <summary>
		/// Register a plugin crash. Can be called on any thread in the browser process
		/// but will be executed on the IO thread.
		/// </summary>
		public static void RegisterWebPluginCrash(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));

			fixed (char* s0 = path)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = path.Length };
				CefNativeApi.cef_register_web_plugin_crash(&cstr0);
			}
		}

		/// <summary>
		/// Query if a plugin is unstable. Can be called on any thread in the browser
		/// process.
		/// </summary>
		public static void IsWebPluginUnstable(string path, CefWebPluginUnstableCallback callback)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			fixed (char* s0 = path)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = path.Length };
				CefNativeApi.cef_is_web_plugin_unstable(&cstr0, callback.GetNativeInstance());
			}
		}

		/// <summary>
		/// Register the Widevine CDM plugin.
		/// The client application is responsible for downloading an appropriate
		/// platform-specific CDM binary distribution from Google, extracting the
		/// contents, and building the required directory structure on the local machine.
		/// The CefBrowserHost::StartDownload function and CefZipArchive structure
		/// can be used to implement this functionality in CEF. Contact Google via
		/// https://www.widevine.com/contact.html for details on CDM download.
		/// </summary>
		/// <param name="path">
		/// Is a directory that must contain the following files:
		/// 1. manifest.json file from the CDM binary distribution.
		/// 2. widevinecdm file from the CDM binary distribution (e.g.
		/// widevinecdm.dll on on Windows, libwidevinecdm.dylib on OS X,
		/// libwidevinecdm.so on Linux).
		/// If any of these files are missing or if the manifest file has incorrect
		/// contents the registration will fail and |callback| will receive a |result|
		/// value of CEF_CDM_REGISTRATION_ERROR_INCORRECT_CONTENTS.
		/// </param>
		/// <param name="callback">
		/// |callback| will be executed asynchronously once registration is complete.
		/// On Linux this function must be called before CefInitialize() and the
		/// registration cannot be changed during runtime. If registration is not
		/// supported at the time that RegisterWidevineCDM() is called then
		/// |callback| will receive a |result| value of
		/// CEF_CDM_REGISTRATION_ERROR_NOT_SUPPORTED.
		/// </param>
		public static void RegisterWidevineCDM(string path, CefRegisterCDMCallback callback)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));

			fixed (char* s0 = path)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = path.Length };
				CefNativeApi.cef_register_widevine_cdm(&cstr0, callback.GetNativeInstance());
			}
		}

		/// <summary>
		/// Returns the current platform thread ID.
		/// </summary>
		public static uint GetCurrentPlatformThreadId()
		{
			return CefNativeApi.cef_get_current_platform_thread_id();
		}

		/// <summary>
		/// Returns the current platform thread handle.
		/// </summary>
		public static IntPtr GetCurrentPlatformThreadHandle()
		{
			if (PlatformInfo.IsWindows)
				return new IntPtr((int)CefNativeApi.cef_get_current_platform_thread_handle_windows());
			if (PlatformInfo.IsLinux)
				return CefNativeApi.cef_get_current_platform_thread_handle_linux();
			throw new NotImplementedException();
		}

		/// <summary>
		/// Start tracing events on all processes.
		/// If CefBeginTracing was called previously, or if a CefEndTracingAsync call is
		/// pending, CefBeginTracing will fail and return False.
		/// This function must be called on the browser process UI thread.
		/// </summary>
		/// <param name="categories">
		/// A comma-delimited list of category wildcards. A category can
		/// have an optional &apos;-&apos; prefix to make it an excluded category. Having both
		/// included and excluded categories in the same list is not supported.
		/// Example: &quot;test_MyTest*&quot; Example: &quot;test_MyTest*,test_OtherStuff&quot;
		/// Example: &quot;-excluded_category1,-excluded_category2&quot;
		/// </param>
		/// <param name="callback">
		/// Tracing is initialized asynchronously and |callback| will be executed on the
		/// UI thread after initialization is complete.
		/// </param>
		public static bool CefBeginTracing(string categories, CefCompletionCallback callback)
		{
			if (categories == null)
				throw new ArgumentOutOfRangeException(nameof(categories));
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			fixed (char* s0 = categories)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = categories.Length };
				return CefNativeApi.cef_begin_tracing(&cstr0, callback.GetNativeInstance()) != 0;
			}
		}

		/// <summary>
		/// Stop tracing events on all processes.
		/// This function will fail and return False if a previous call to
		/// CefEndTracingAsync is already pending or if CefBeginTracing was not called.
		/// This function must be called on the browser process UI thread.
		/// </summary>
		/// <param name="path">
		/// The path at which tracing data will be written. If |path| is NULL
		/// a new temporary file path will be used.
		/// </param>
		/// <param name="callback">
		/// The callback that will be executed once all processes have sent their trace data.
		/// If |callback| is NULL no trace data will be written.
		/// </param>
		public static bool CefEndTracing(string path, CefEndTracingCallback callback)
		{
			if (callback == null)
				throw new ArgumentNullException(nameof(callback));

			fixed (char* s0 = path)
			{
				var cstr0 = new cef_string_t { Str = s0, Length = path != null ? path.Length : 0 };
				return CefNativeApi.cef_end_tracing(&cstr0, callback.GetNativeInstance()) != 0;
			}
		}

		/// <summary>
		/// Returns the current system trace time or, if none is defined, the current
		/// high-res time. Can be used by clients to synchronize with the time
		/// information in trace events.
		/// </summary>
		public static long CefNowFromSystemTraceTime()
		{
			return CefNativeApi.cef_now_from_system_trace_time();
		}

		/// <summary>
		/// Returns CEF version information for the libcef library.
		/// </summary>
		/// <param name="component">
		/// The |component| parameter describes which version component will be returned.
		/// </param>
		public static int CefVersionInfo(CefVersionComponent component)
		{
			return CefNativeApi.cef_version_info((int)component);
		}

		/// <summary>
		/// Returns CEF API hashes for the libcef library.
		/// </summary>
		/// <param name="type">
		/// The |type| parameter describes which hash value will be returned.
		/// </param>
		public static string CefApiHash(CefApiHashType type)
		{
			return Marshal.PtrToStringAnsi(CefNativeApi.cef_api_hash((int)type));
		}

	}
}

#endif