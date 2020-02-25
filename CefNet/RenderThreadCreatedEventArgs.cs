using System;
using System.Collections.Generic;
using System.Text;

namespace CefNet
{
	/// <summary>
	/// Provides data for the <see cref="CefNetApplication.RenderThreadCreated"/> event.
	/// </summary>
	public class RenderThreadCreatedEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RenderThreadCreatedEventArgs"/> class.
		/// </summary>
		/// <param name="extraInfo">The extra information.</param>
		public RenderThreadCreatedEventArgs(CefListValue extraInfo)
		{
			this.ExtraInfo = extraInfo;
		}

		/// <summary>
		/// Provides an opportunity to specify extra information that will be passed
		/// to the <see cref="CefNetApplication.RenderThreadCreated"/> event in
		/// the render process.<para/>
		/// Do not keep a reference to <see cref="ExtraInfo"/>.
		/// </summary>
		public CefListValue ExtraInfo { get; }
	}
}
