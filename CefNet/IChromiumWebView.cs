//using CefNet.DOM;

using CefNet.WinApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace CefNet
{
	public interface IChromiumWebView
	{
		/// <summary>
		/// Occurs before a new browser window is opened.
		/// </summary>
		event EventHandler<CreateWindowEventArgs> CreateWindow;
		event EventHandler<BeforeBrowseEventArgs> Navigating;
		event EventHandler<NavigatedEventArgs> Navigated;
		event EventHandler<LoadingStateChangeEventArgs> LoadingStateChange;

		event EventHandler<CancelEventArgs> Closing;
		event EventHandler Closed;
		event EventHandler<CefPaintEventArgs> CefPaint;
		event EventHandler BrowserCreated;
		event EventHandler<DocumentTitleChangedEventArgs> DocumentTitleChanged;



		bool CanGoBack { get; }
		bool CanGoForward { get; }
		bool IsBusy { get; }
		int Identifier { get; }
		bool HasDocument { get; }

		bool GoBack();
		bool GoForward();
		void Reload();
		void ReloadIgnoreCache();
		void Stop();
		CefFrame GetMainFrame();
		CefFrame GetFocusedFrame();
		CefFrame GetFrame(long identifier);
		CefFrame GetFrame(string name);
		int GetFrameCount();
		long[] GetFrameIdentifiers();
		string[] GetFrameNames();


		void ShowDevTools();
		void ShowDevTools(CefPoint inspectElementAt);
		void NotifyRootMovedOrResized();
		void Close();
		void Navigate(string url);
#if NAVIGATEWITHPARAMS
		void Navigate(string url, string referrer);
		void NavigateWithParams(string url, string referrer, PageTransition navigationType, PostData postData, string extraHeaders);
		void LoadContent(string url, string referrer, string contentType, string content, Encoding encoding);
#endif //NAVIGATEWITHPARAMS
		//Window GetWindow();

		CefBrowser BrowserObject { get; }

		/// <summary>
		/// Sends the KeyDown event to the browser.
		/// </summary>
		/// <param name="c">The character associated with the key.</param>
		/// <param name="ctrlKey">The Control key flag.</param>
		/// <param name="altKey">The Alt key flag.</param>
		/// <param name="shiftKey">The Shift key flag.</param>
		/// <param name="metaKey">The Meta key flag.</param>
		/// <param name="repeatCount">The repeat count.</param>
		/// <param name="extendedKey">The extended key flag.</param>
		void SendKeyDown(char c, bool ctrlKey = false, bool altKey = false, bool shiftKey = false, bool metaKey = false, int repeatCount = 0, bool extendedKey = false);

		/// <summary>
		/// Sends the KeyDown event to the browser.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="ctrlKey">The Control key flag.</param>
		/// <param name="altKey">The Alt key flag.</param>
		/// <param name="shiftKey">The Shift key flag.</param>
		/// <param name="metaKey">The Meta key flag.</param>
		/// <param name="repeatCount">The repeat count.</param>
		/// <param name="extendedKey">The extended key flag.</param>
		void SendKeyDown(VirtualKeys key, bool ctrlKey = false, bool altKey = false, bool shiftKey = false, bool metaKey = false, int repeatCount = 0, bool extendedKey = false);

		/// <summary>
		/// Sends the KeyUp event to the browser.
		/// </summary>
		/// <param name="c">The character associated with the key.</param>
		/// <param name="ctrlKey">The Control key flag.</param>
		/// <param name="altKey">The Alt key flag.</param>
		/// <param name="shiftKey">The Shift key flag.</param>
		/// <param name="metaKey">The Meta key flag.</param>
		/// <param name="extendedKey">The extended key flag.</param>
		void SendKeyUp(char c, bool ctrlKey = false, bool altKey = false, bool shiftKey = false, bool metaKey = false, bool extendedKey = false);


		/// <summary>
		/// Sends the KeyUp event to the browser.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="ctrlKey">The Control key flag.</param>
		/// <param name="altKey">The Alt key flag.</param>
		/// <param name="shiftKey">The Shift key flag.</param>
		/// <param name="metaKey">The Meta key flag.</param>
		/// <param name="extendedKey">The extended key flag.</param>
		void SendKeyUp(VirtualKeys key, bool ctrlKey = false, bool altKey = false, bool shiftKey = false, bool metaKey = false, bool extendedKey = false);

		/// <summary>
		/// Sends the KeyPress event to the browser.
		/// </summary>
		/// <param name="c">The character associated with the key.</param>
		/// <param name="ctrlKey">The Control key flag.</param>
		/// <param name="altKey">The Alt key flag.</param>
		/// <param name="shiftKey">The Shift key flag.</param>
		/// <param name="metaKey">The Meta key flag.</param>
		/// <param name="extendedKey">The extended key flag.</param>
		void SendKeyPress(char c, bool ctrlKey = false, bool altKey = false, bool shiftKey = false, bool metaKey = false, bool extendedKey = false);

#if USERAGENTOVERRIDE
		void SetUserAgentOverride(string useragent);
		string GetUserAgentOverride();
#endif //USERAGENTOVERRIDE
	}
}
