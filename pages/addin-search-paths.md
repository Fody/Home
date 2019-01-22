# Addin Search Strategies

Every Weaver must publish the location of it's binary at compile time as an MSBuild item, so Fody is able to locate it.
Usually this is achieved by providing a `.props` file with the NuGet package with the following default content:

```xml
﻿<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

  <PropertyGroup>
    <WeaverRuntimeToken
        Condition="$(MSBuildRuntimeType) != 'Core'">netclassicweaver</WeaverRuntimeToken>
    <WeaverRuntimeToken
        Condition="$(MSBuildRuntimeType) == 'Core'">netstandardweaver</WeaverRuntimeToken>
  </PropertyGroup>

  <ItemGroup>
    <WeaverFiles
        Include="$(MsBuildThisFileDirectory)..\$(WeaverRuntimeToken)\$(MSBuildThisFileName).dll" />
  </ItemGroup>

</Project>
```

If you use the `FodyPackaging` NuGet package to create your add in package, this file is automatically added and you don't have to care about it.

However if you want to achieve something special, you may provide your own implementation. The important part here is to provide an item named `WeaverFiles` that points to the location of the weaver assembly somewhere in the build chain.

To e.g. replace the legacy `SolutionDir/Tool` or [InSolutionWeaving](InSolutionWeaving) conventions, just add the `WeaverFiles` item 
to any project that needs to consume it:

```xml
<ItemGroup>
  <WeaverFiles
    Include="$(SolutionDir)SampleWeaver.Fody\bin\$(Configuration)\Net46\SampleWeaver.Fody.dll" />
</ItemGroup>
```

---

#### Legacy strategies

Legacy Weavers that are listed in the `FodyWeavers.xml` file, but don't expose the `WeaverFiles` 
MSBuild item, are located using a simple directory search.

The following directories are searched for legacy Weavers

 * NuGet Package directories
 * SolutionDir/Tools
 * A project in the solution named 'Weavers'. See [InSolutionWeaving](InSolutionWeaving)

Only the newest assembly of every found Weaver (as defined by Assembly.Version) is used.

Since this can result in random results, depending on the actual content of the folders, avoid to use such legacy weavers, but ask the owner to update the weaver.