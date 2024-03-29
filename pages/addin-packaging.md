<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /pages/mdsource/addin-packaging.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# Addin packaging


## Convention

The convention for Fody addin NuGet packages is as follows:

&#x1F4C1; build<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.props<br>
&#x1F4C1; lib<br>
&nbsp;&nbsp;&nbsp;&#x1F4C1; net452<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; AddinName.dll<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; AddinName.xml<br>
&nbsp;&nbsp;&nbsp;&#x1F4C1; netstandard2.0<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; AddinName.dll<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; AddinName.xml<br>
&#x1F4C1; weaver<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.dll<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.xcf<br>


### Convention Descriptions


 * `build/AddinName.Fody.props`: Facilitates [addin discovery](addin-discovery.md) via an [props file included by NuGet](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package#including-msbuild-props-and-targets-in-a-package).
 * `lib`: Contains the [Lib/Reference project](addin-development.md#Lib/Reference-project) for all [supported target frameworks](https://docs.microsoft.com/en-us/nuget/create-packages/supporting-multiple-target-frameworks).
 * `weaver`: Contains the [Weaver Project](addin-development.md#Weaver-Project) assembly. Also contains the XCF file to [Supporting intellisense for FodyWeavers.xml](addin-development.md#Supporting-intellisense-for-FodyWeavers.xml).


## FodyPackaging NuGet Package

The [FodyPackaging NuGet Package](https://www.nuget.org/packages/FodyPackaging/) simplifies following the above convention.


### MSBuild props and targets

The below files are include as [MSBuild props and targets in a package](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package#including-msbuild-props-and-targets-in-a-package).


#### FodyPackaging.props

<!-- snippet: FodyPackaging.props -->
<a id='snippet-fodypackaging.props'></a>
```props
<Project>
  <PropertyGroup>
    <PackageId Condition="'$(PackageId)' == ''">$(MSBuildProjectName).Fody</PackageId>
    <PackageTags Condition="'$(PackageTags)' == ''">ILWeaving, Fody, Cecil, AOP</PackageTags>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <TargetsForTfmSpecificContentInPackage Condition="'$(SkipPackagingDefaultFiles)' != 'true'">$(TargetsForTfmSpecificContentInPackage);IncludeFodyFiles</TargetsForTfmSpecificContentInPackage>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <NoPackageAnalysis>true</NoPackageAnalysis>
    <NoWarn>$(NoWarn);NU5118</NoWarn>
    <DisableFody>true</DisableFody>
    <WeaverDirPath Condition="'$(WeaverDirPath)' == ''">..\$(PackageId)\bin\$(Configuration)\</WeaverDirPath>
    <WeaverPropsFile Condition="'$(WeaverPropsFile)' == ''">$(MSBuildThisFileDirectory)..\Weaver.props</WeaverPropsFile>
  </PropertyGroup>
</Project>

```
<sup><a href='#snippet-fodypackaging.props' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### FodyPackaging.targets

<!-- snippet: FodyPackaging.targets -->
<a id='snippet-fodypackaging.targets'></a>
```targets
<Project>
  <Target Name="IncludeFodyFiles">
    <PropertyGroup>
      <WeaverFile>$(WeaverDirPath)\netstandard2.0\$(PackageId).dll</WeaverFile>
      <XcfFile>$(WeaverDirPath)\netstandard2.0\$(PackageId).xcf</XcfFile>
    </PropertyGroup>

    <Error Text="FodyPackaging: No weaver found at $(WeaverFile). $(ProjectName) should have a Project Dependency on $(PackageId), and $(PackageId) should target 'netstandard2.0'."
           Condition="!Exists($(WeaverFile))" />

    <ItemGroup>
      <TfmSpecificPackageFile Include="$(XcfFile)"
                              PackagePath="weaver\$(PackageId).xcf"
                              Condition="Exists($(XcfFile))" />
      <TfmSpecificPackageFile Include="$(WeaverFile)"
                              PackagePath="weaver\$(PackageId).dll" />
      <TfmSpecificPackageFile Include="$(WeaverPropsFile)"
                              PackagePath="build\$(PackageId).props" />
    </ItemGroup>
  </Target>
</Project>

```
<sup><a href='#snippet-fodypackaging.targets' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Weaver.props

Included in the consuming package to facilitate [addin discovery](addin-discovery.md).

<!-- snippet: Weaver.props -->
<a id='snippet-weaver.props'></a>
```props
<Project>
  <ItemGroup>
    <WeaverFiles Include="$(MsBuildThisFileDirectory)..\weaver\$(MSBuildThisFileName).dll" />
  </ItemGroup>
</Project>

```
<sup><a href='#snippet-weaver.props' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
