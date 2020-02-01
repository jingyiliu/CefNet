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
		/// <summary>
		/// Occurs to report find results returned by <see cref="Find"/>.
		/// </summary>
		event EventHandler<ITextFoundEventArgs> TextFound;


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

		/// <summary>
		/// Open developer tools.
		/// </summary>
		void ShowDevTools();

		/// <summary>
		/// Open developer tools (DevTools). If the DevTools is already open then it will be focused.
		/// </summary>
		/// <param name="inspectElementAt">
		/// If <paramref name="inspectElementAt"/> is non-empty then the element at the specified (x,y) location will be inspected.
		/// </param>
		void ShowDevTools(CefPoint inspectElementAt);

		/// <summary>
		/// Explicitly close the associated developer tools, if any.
		/// </summary>
		void CloseDevTools();

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

		/// <summary>
		/// Search for <paramref name="searchText"/>. The <see cref="TextFound"/> event
		/// will be occurred to report find results.
		/// </summary>
		/// <param name="identifier">
		/// An unique ID and these IDs must strictly increase so that newer requests always
		/// have greater IDs than older requests. If <paramref name="identifier"/> is zero or less than the
		/// previous ID value then it will be automatically assigned a new valid ID.
		/// </param>
		/// <param name="searchText">The string to seek.</param>
		/// <param name="forward">A value which indicates whether to search forward or backward within the page.</param>
		/// <param name="matchCase">The true value indicates that the search should be case-sensitive.</param>
		/// <param name="findNext">A value which indicates whether this is the first request or a follow-up.</param>
		void Find(int identifier, string searchText, bool forward, bool matchCase, bool findNext);

		/// <summary>
		/// Cancel all searches that are currently going on.
		/// </summary>
		void StopFinding(bool clearSelection);

#if USERAGENTOVERRIDE
		void SetUserAgentOverride(string useragent);
		string GetUserAgentOverride();
#endif //USERAGENTOVERRIDE
	}
}
