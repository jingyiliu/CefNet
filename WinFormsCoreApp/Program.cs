using CefNet;
using CefNet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsCoreApp
{
	static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.SetHighDpiMode(HighDpiMode.SystemAware);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);


			string cefPath = Path.Combine(Path.GetDirectoryName(GetProjectPath()), "cef");

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;

			var settings = new CefSettings();
			settings.MultiThreadedMessageLoop = true;
			settings.NoSandbox = true;
			settings.WindowlessRenderingEnabled = true;
			settings.LocalesDirPath = Path.Combine(cefPath, "Resources", "locales");
			settings.ResourcesDirPath = Path.Combine(cefPath, "Resources");
			settings.LogSeverity = CefLogSeverity.Warning;
			settings.IgnoreCertificateErrors = true;
			settings.UncaughtExceptionStackSize = 8;
			
			var app = new CefAppImpl();
			app.CefProcessMessageReceived += ScriptableObjectTests.HandleScriptableObjectTestMessage;
			try
			{
				app.Initialize(Path.Combine(cefPath, "Release"), settings);
				Application.Run(new MainForm());
			}
			finally
			{
				app.Shutdown();
				app.Dispose();
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			ShowUnhandledException(e.ExceptionObject as Exception, "AppDomain::UnhandledException");
		}

		private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			ShowUnhandledException(e.Exception, "Application::ThreadException");
		}

		private static void ShowUnhandledException(Exception exception, string from)
		{
			if (exception == null)
				return;
			MessageBox.Show(string.Format("{0}: {1}\r\n{2}", exception.GetType().Name, exception.Message, exception.StackTrace), from);
		}

		private static string GetProjectPath()
		{
			string projectPath = Path.GetDirectoryName(typeof(Program).Assembly.Location);
			string rootPath = Path.GetPathRoot(projectPath);
			while (Path.GetFileName(projectPath) != "WinFormsCoreApp")
			{
				if (projectPath == rootPath)
					throw new DirectoryNotFoundException("Could not find the project directory.");
				projectPath = Path.GetDirectoryName(projectPath);
			}
			return projectPath;
		}

	}
}
