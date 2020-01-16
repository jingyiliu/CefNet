using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CefNet;
using CefNet.Avalonia;
using System;

namespace AvaloniaApp
{
	public class MainWindow : Window
	{
		bool isFirstLoad = true;

		private TextBox txtAddress = null;
		private TabControl tabs = null;
		private Menu menu = null;
		private DockPanel controlsPanel = null;

		public MainWindow()
		{
			InitializeComponent();

			this.Opened += MainWindow_Opened;
			CustomWebView.FullscreenEvent.AddClassHandler(typeof(WebView), HandleFullscreenEvent);

#if DEBUG
			this.AttachDevTools();
#endif
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);

			txtAddress = this.FindControl<TextBox>(nameof(txtAddress));
			tabs = this.FindControl<TabControl>(nameof(tabs));
			menu = this.FindControl<Menu>(nameof(menu));
			controlsPanel = this.FindControl<DockPanel>(nameof(controlsPanel));
		}


		private void HandleFullscreenEvent(object sender, RoutedEventArgs e)
		{
			WrapPanel tabHeaders = tabs.FindChild<WrapPanel>(null);
			if (((FullscreenModeChangeEventArgs)e).Fullscreen)
			{
				menu.IsVisible = false;
				controlsPanel.IsVisible = false;
				tabHeaders.IsVisible = false;
				this.HasSystemDecorations = false;
				WindowState = WindowState.Maximized;
				Topmost = true;
			}
			else
			{
				menu.IsVisible = true;
				controlsPanel.IsVisible = true;
				tabHeaders.IsVisible = true;
				this.HasSystemDecorations = true;
				WindowState = WindowState.Normal;
				Topmost = false;
			}
		}

		private void MainWindow_Opened(object sender, EventArgs e)
		{
			if (!isFirstLoad)
				return;
			isFirstLoad = false;

			AddTab(true);
		}

		private void AddTab(bool useGlobalContext)
		{
			WebViewTab viewTab;
			if (useGlobalContext)
			{
				viewTab = new WebViewTab();
				viewTab.WebView.Navigated += WebView_Navigated;
				((AvaloniaList<object>)tabs.Items).Add(viewTab);
				viewTab.Title = "about:blank";

				tabs.SelectedItem = viewTab;
			}
			else
			{
				//var cx = new CefRequestContext(new CefRequestContextSettings());
				//tabs.Controls.Add(new WebViewTab(new CefBrowserSettings(), cx));
			}
		}

		private void WebView_Navigated(object sender, NavigatedEventArgs e)
		{
			txtAddress.Text = e.Url.ToString();
		}

		private IChromiumWebView SelectedView
		{
			get
			{
				return (tabs.SelectedItem as WebViewTab)?.WebView;
			}
		}



		private void AddTab_Click(object sender, RoutedEventArgs e)
		{
			AddTab(true);
		}

		private void NavigateButton_Click(object sender, RoutedEventArgs e)
		{
			//SelectedView?.Navigate("http://yandex.ru");
			SelectedView?.Navigate("http://example.com");
		}

		private void txtAddress_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if (Uri.TryCreate(txtAddress.Text, UriKind.Absolute, out Uri url))
				{
					SelectedView?.Navigate(url.AbsoluteUri);
				}
			}
		}

	}
}
