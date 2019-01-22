# Supported .net runtimes and IDE


## Visual Studio support

Visual Studio 2017 and above are supported. Older versions of Visual Studio may still work, but are not actively supported.

Compatibility test builds:

 * Visual Studio 2015: ![Build status](https://tom-englert.visualstudio.com/Open%20Source/_apis/build/status/FodyIntegration2015)
 * Visual Studio 2013: ![Build status](https://tom-englert.visualstudio.com/Open%20Source/_apis/build/status/FodyIntegration2013)
 * Visual Studio 2012: ![Build status](https://tom-englert.visualstudio.com/Open%20Source/_apis/build/status/FodyIntegration2012)


## Target framework

Fody targets (and requires) the following at build time

 * [.net 4.6](http://blogs.msdn.com/b/dotnet/archive/2015/07/20/announcing-net-framework-4-6.aspx), or higher, to work with [MSBuild 15](https://docs.microsoft.com/en-us/visualstudio/msbuild/what-s-new-in-msbuild-15-0).
 * [dotnet core SDK](https://dotnet.microsoft.com/download) to work with [dotnet build](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build).


### Implications if using using Fody on project

Fody provides the build integration for the weavers. The runtime that the project may target only depends on the weaver(s) being used; see the documentation of the individual weavers to know what frameworks are supported.

The target framework(s) supported by a weaver is up to the designer of the individual weaver. Since extending the range of supported frameworks might introduce extra overhead, not all possible framework versions might be actively supported.
