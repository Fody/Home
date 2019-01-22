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


### Implications on your project using Fody

Fody only provides the build integration for the weavers and does not touch your assemblies. So the runtime that your project may target only depends on the weaver(s) being used; see the documentation of the individual weavers you want to use to know what framework you may target.

Many weavers only inject very low-level code, so they could probably even weave assemblies targeting Net2.0, while e.g. weavers dealing with async patterns definitely require at least Net4.5.

However what target frameworks are supported by a weaver is up to the designer of the individual weaver. Since extending the range of supported frameworks might introduce extra overhead, not all possible framework versions might be actively supported.


### Design guidelines for writing weavers

A weaver only mangles with the IL code, so generally it's runtime agnostic. However if the injected code adds references to runtime methods, the weaved code won't work with any runtime that does not provide this method.

A weaver will limit the required target runtime by the version(s) of the build time assembly - the one that is in the lib folder and gets referenced by the target project - that it provides.

It's a common good practice to only demand what is really required. E.g. if you only need to read a file, don't open it in write mode. Same applies to your weaver - if it only uses Net4.0 methods, it should not demand Net4.5.

There are several good reasons to not limit the usage of a weaver to a higher target frameword than really required. Users of your weaver may just not be able to change the target framework of their build output to something higher; one reason would be that the output is used in many other projects maintained by other teams, and changing the target framework would set off an avalanche of compatibility issues. A popular example is e.g. [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/). Even the latest package still supports Net2.0, just because removing it could introduce unknown side effects. Such a library could not make use of Fody at all, if the weaver demands Net4.5 without any need.

To easily verify that your weaver is compatible with the frameworks it claims to work with, you can add a simple smoke-test like in the sample [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin/tree/master/SmokeTest) - just make sure it covers enough of your code to be meaningful.

**Once you have published a weaver, do not increase the supported framework versions without real need.**

If some user has a project that targets framework version X.0 and has already used your weaver, NuGet will mess up the whole project when trying to update a weaver that longer supports version X.0, leaving the users project in an unrecoverable state with mixed netclassic and netstandard references!