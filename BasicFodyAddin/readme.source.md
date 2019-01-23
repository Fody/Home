[![Chat on Gitter](https://img.shields.io/gitter/room/fody/fody.svg?style=flat&max-age=86400)](https://gitter.im/Fody/Fody)
[![NuGet Status](http://img.shields.io/nuget/v/BasicFodyAddin.Fody.svg?style=flat&max-age=86400)](https://www.nuget.org/packages/BasicFodyAddin.Fody/)

![Icon](https://raw.githubusercontent.com/Fody/BasicFodyAddin/master/package_icon.png)

This is a simple solution built as a starter for writing [Fody](https://github.com/Fody/Fody) addins.


## Usage

See also [Fody usage](https://github.com/Fody/Fody#usage).


### NuGet installation

Install the [BasicFodyAddin.Fody NuGet package](https://nuget.org/packages/BasicFodyAddin.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package BasicFodyAddin.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

Add `<BasicFodyAddin/>` to [FodyWeavers.xml](https://github.com/Fody/Fody#add-fodyweaversxml)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <BasicFodyAddin/>
</Weavers>
```


## The moving parts


### BasicFodyAddin Project

A project that contains all classes used for compile time metadata. Generally any usage and reference to this project is removed at compile time so it is not needed as part of application deployment.

This project is also used to produce the NuGet package. To achieve this the project consumes two NuGets:

 * [Fody](https://www.nuget.org/packages/Fody/) with `PrivateAssets="None"`. This results in producing NuGet package having a dependency on Fody with all ` include="All"` in the nuspec. Note that while this project consumes the Fody NuGet, weaving is not performed on this project. This is due to the FodyPackaging NuGet (see below) including ` <DisableFody>true</DisableFody>` in the MSBuild pipeline.
 * [FodyPackaging](https://www.nuget.org/packages/FodyPackaging/) with `PrivateAssets="All"`. This results in a NuGet package being produced by this project, but no dependency on FodyPackaging in the resulting NuGet package. 

The produced NuGet package will be named with `.Fody` suffix.

This project should also contain all appropriate [NuGet metadata properties](https://docs.microsoft.com/en-us/dotnet/core/tools/csproj#nuget-metadata-properties). Many of these properties have defaults in [FodyPackaging](https://github.com/Fody/Fody/blob/master/FodyPackaging/build/FodyPackaging.props), but can be overridden.

The resultant NuGet package will target the same frameworks that this project targets.

The resultant NuGet package will be created in a directory named `nugets` at the root of the solution.


### BasicFodyAddin.Fody Project

The project that does the weaving.

This project has a NuGet dependency on [FodyHelpers](https://www.nuget.org/packages/FodyHelpers/).


#### Target Frameworks

This project must target `net46` for [msbuild.exe](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild) support, and `netstandard2.0` for [dotnet build](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build) support.


#### Output of the project

It outputs a file named `BasicFodyAddin.Fody`. The '.Fody' suffix is necessary to be picked up by Fody at compile time.


#### ModuleWeaver

ModuleWeaver.cs is where the target assembly is modified. Fody will pick up this type during its processing. Note that the class must be named as `ModuleWeaver`.

`ModuleWeaver` has a base class of `BaseModuleWeaver` which exists in the [FodyHelpers NuGet](https://www.nuget.org/packages/FodyHelpers/).

snippet: ModuleWeaver


##### BaseModuleWeaver.Execute

Called to perform the manipulation of the module. The current module can be accessed and manipulated via `BaseModuleWeaver.ModuleDefinition`.

snippet: Execute


##### BaseModuleWeaver.GetAssembliesForScanning

Called by Fody when it is building up a type cache for lookups. This method should return all possible assemblies that the weaver may require while resolving types. In this case BasicFodyAddin requires `System.Object`, so `GetAssembliesForScanning` returns `netstandard` and `mscorlib`. It is safe to return assembly names that are not used by the current target assembly as these will be ignored.

To use this type cache, a `ModuleWeaver` can call `BaseModuleWeaver.FindType` within `Execute` method.

snippet: GetAssembliesForScanning


##### BaseModuleWeaver.ShouldCleanReference

When `BasicFodyAddin.dll` is referenced by a consuming project, it is only for the purposes configuring the weaving via attributes. As such, it is not required at runtime. With this in mind `BaseModuleWeaver` has an opt in feature to remove the reference, meaning the target weaved application does not need `BasicFodyAddin.dll` at runtime. This feature can be opted in to via the following code in `ModuleWeaver`:

snippet: ShouldCleanReference


##### Other BaseModuleWeaver Members

`BaseModuleWeaver` has a number of other members for logging and extensibility:
https://github.com/Fody/Fody/blob/master/FodyHelpers/BaseModuleWeaver.cs


#### Resultant injected code

In this case a new type is being injected into the target assembly that looks like this.

```csharp
public class Hello
{
    public string World()
    {
        return "Hello World";
    }
}
```

See [ModuleWeaver](https://github.com/Fody/Fody/wiki/ModuleWeaver) for more details.


### AssemblyToProcess Project

A target assembly to process and then validate with unit tests.


### Tests Project

Contains all tests for the weaver.

The project has a NuGet dependency on [FodyHelpers](https://www.nuget.org/packages/FodyHelpers/) .

It has a reference to the `AssemblyToProcess` project, so that `AssemblyToProcess.dll` is copied to the bin directory of the test project.

FodyHelpers contains a utility [WeaverTestHelper](https://github.com/Fody/Fody/blob/master/FodyHelpers/Testing/WeaverTestHelper.cs) for executing test runs on a target assembly using a ModuleWeaver. 

A test can then be run as follows:

snippet: WeaverTests

By default `ExecuteTestRun` will perform a [PeVerify](https://docs.microsoft.com/en-us/dotnet/framework/tools/peverify-exe-peverify-tool) on the resultant assembly


## Icon

<a href="http://thenounproject.com/noun/lego/#icon-No16919" target="_blank">Lego</a> designed by <a href="http://thenounproject.com/timur.zima" target="_blank">Timur Zima</a> from The Noun Project
