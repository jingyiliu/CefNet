using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CefNet;
using System.IO;
using WinFormsCoreApp;

namespace AvaloniaApp
{
	public class App : Application
	{
		private CefAppImpl app;

		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}

		public override void OnFrameworkInitializationCompleted()
		{
			if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			{
				desktop.MainWindow = new MainWindow();
				desktop.Startup += Startup;
				desktop.Exit += Exit;
			}

			base.OnFrameworkInitializationCompleted();
		}

		private void Startup(object sender, ControlledApplicationLifetimeStartupEventArgs e)
		{
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

		private void Exit(object sender, ControlledApplicationLifetimeExitEventArgs e)
		{
			app?.Shutdown();
		}

		private static string GetProjectPath()
		{
			string projectPath = Path.GetDirectoryName(typeof(App).Assembly.Location);
			string rootPath = Path.GetPathRoot(projectPath);
			while (Path.GetFileName(projectPath) != "AvaloniaApp")
			{
				if (projectPath == rootPath)
					throw new DirectoryNotFoundException("Could not find the project directory.");
				projectPath = Path.GetDirectoryName(projectPath);
			}
			return projectPath;
		}
	}
}
