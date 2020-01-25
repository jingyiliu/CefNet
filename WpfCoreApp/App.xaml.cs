using CefNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WinFormsCoreApp;

namespace WpfCoreApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private CefAppImpl app;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			string cefPath = Path.Combine(Path.GetDirectoryName(GetProjectPath()), "cef");


			var settings = new CefSettings();
			settings.MultiThreadedMessageLoop = true;
			settings.NoSandbox = true;
			settings.WindowlessRenderingEnabled = true;
			settings.LocalesDirPath = Path.Combine(cefPath, "Resources", "locales");
			settings.ResourcesDirPath = Path.Combine(cefPath, "Resources");
			settings.LogSeverity = CefLogSeverity.Warning;
			settings.IgnoreCertificateErrors = true;
			settings.UncaughtExceptionStackSize = 8;

			app = new CefAppImpl();
			app.Initialize(Path.Combine(cefPath, "Release"), settings);
		}

		protected override void OnExit(ExitEventArgs e)
		{
			//Thread.Sleep(1000);
			GC.Collect();
			GC.WaitForPendingFinalizers();

			app?.Shutdown();
			base.OnExit(e);

		}

		private static string GetProjectPath()
		{
			string projectPath = Path.GetDirectoryName(typeof(App).Assembly.Location);
			string rootPath = Path.GetPathRoot(projectPath);
			while (Path.GetFileName(projectPath) != "WpfCoreApp")
			{
				if (projectPath == rootPath)
					throw new DirectoryNotFoundException("Could not find the project directory.");
				projectPath = Path.GetDirectoryName(projectPath);
			}
			return projectPath;
		}
	}
}
