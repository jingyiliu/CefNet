# CefNet

CefNet is a .NET CLR binding for the Chromium Embedded Framework (CEF).

## Getting Started

...

## Features

* Cross-platform
* Full managed code

## Warning
The API of this project is not frozen and is subject to change.

## Develop

1. Install [.NET Core SDK](https://www.microsoft.com/net/download)
2. Install the [DotAsm](https://www.nuget.org/packages/DotAsm/) tool: `dotnet tool install -g DotAsm`
3. Run `git clone https://github.com/CefNet/CefNet.git`
4. Download a [CEF package](http://opensource.spotify.com/cefbuilds/index.html). See [Directory.Build.props](https://github.com/CefNet/CefNet/blob/master/Directory.Build.props) for the required CEF version.
5. Extract all files into the cef/ directory.
6. Copy cef/Resources/icudtl.dat into cef/Release/.

## Migration to other CEF build
1. Download a [CEF package](http://opensource.spotify.com/cefbuilds/index.html).
2. Extract all header files into the cef/include directory.
3. Build and run CefGen.sln in debug mode to generate the generated files. Watch the output for errors.
4. Build CefNet.sln
5. If the build fails, make the necessary changes.

## Similar projects and links

* [CefGlue](https://gitlab.com/xiliumhq/chromiumembedded/cefglue): An alternative .NET CEF wrapper built using P/Invoke.
* [CefSharp](https://github.com/cefsharp/CefSharp): Another .NET CEF wrapper built using C++/CLI.
* [ChromiumFx](https://bitbucket.org/chromiumfx/chromiumfx): Another .Net CEF wrapper built using C++ and P/Invoke.
* [CEF Bitbucket Project](https://bitbucket.org/chromiumembedded/cef/overview): The official CEF issue tracker
* The official CEF Forum: http://magpcss.org/ceforum/
* CEF API Docs: http://magpcss.org/ceforum/apidocs3/index-all.html
