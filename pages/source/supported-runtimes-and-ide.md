# Supported .net runtimes and IDE


## Requirements at build time

To build a project using Fody you will need:

 * Visual Studio 2017
 * .Net >= 4.6

older versions of Visual Studio may still work, but are not actively supported.

Compatibility test builds:

 * Visual Studio 2015: ![Build status](https://tom-englert.visualstudio.com/Open%20Source/_apis/build/status/FodyIntegration2015)
 * Visual Studio 2013: ![Build status](https://tom-englert.visualstudio.com/Open%20Source/_apis/build/status/FodyIntegration2013)
 * Visual Studio 2012: ![Build status](https://tom-englert.visualstudio.com/Open%20Source/_apis/build/status/FodyIntegration2012)


## Target framework


### General considerations

The target framework that you build your project against only limits the types and methods you can use to the ones provided by this framework - no less and no more. It does not limit you to run on any newer framework. It even does not provide any means to prohibit running on an older framework!

So you have to choose your target framework settings based on the framework that you know your host (finally the .exe starting the process) guarantees. Depending on how your host loads your assembly, this could introduce some pitfalls:

For example MSBuild 15 guarantees to run on .Net4.6

```xml
<startup useLegacyV2RuntimeActivationPolicy="true">
  <supportedRuntime version="v4.0"
                    sku=".NETFramework,Version=v4.6" />
</startup>
```

Now when writing a build task, you could set the target framework of your assembly to 4.7, with the following impact:

 * as long as you don't use any type or method introduced after 4.6, that will work great always.
 * if you use anything introduced in 4.7, and the target machine has installed 4.7, that will work fine, too.
 * however if you use anything introduced in 4.7, and the target machine has installed only 4.6, MSBuild will start, but crash at runtime in your code with a MemberAccessException.

So it's a good decision for a library to choose the target framework only as high as really needed, to be on the safe side.

This is why Fody itself targets Net4.6, and any weaver should do the same.


### Implications if using using Fody on project

Fody only provides the build integration for the weavers and does not touch your assemblies. So the runtime that your project may target only depends on the weaver(s) being used; see the documentation of the individual weavers you want to use to know what framework you may target.

Many weavers only inject very low-level code, so they could probably even weave assemblies targeting Net2.0, while e.g. weavers dealing with async patterns definitely require at least Net4.5.

However what target frameworks are supported by a weaver is up to the designer of the individual weaver. Since extending the range of supported frameworks might introduce extra overhead, not all possible framework versions might be actively supported.

