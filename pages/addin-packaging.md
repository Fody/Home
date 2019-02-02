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
&#x1F4C1; netclassicweaver<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.dll<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.pdb<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.xcf<br>
&#x1F4C1; netstandardweaver<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.dll<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.pdb<br>
&nbsp;&nbsp;&nbsp; AddinName.Fody.xcf<br>


### Convention Descriptions


 * `build/AddinName.Fody.props`: Facilitates [addin discovery](addin-discovery.md) via an [props file included by NuGet](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package#including-msbuild-props-and-targets-in-a-package).
 * `lib`: Contains the [Lib/Reference project](addin-development.md#Lib/Reference-project) for all [supported target frameworks](https://docs.microsoft.com/en-us/nuget/create-packages/supporting-multiple-target-frameworks).
 * `netclassicweaver`/`netstandardweaver1: Contains the [Weaver Project](addin-development.md#Weaver-Project) assemblies. Also contains the XCF file to [Supporting intellisense for FodyWeavers.xml](addin-development.md#Supporting-intellisense-for-FodyWeavers.xml).


## FodyPackaging NuGet Package

The [FodyPackaging NuGet Package](https://www.nuget.org/packages/FodyPackaging/) simplifies following the above convention.


### MSBuild props and targets

The below files are include as [MSBuild props and targets in a package](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package#including-msbuild-props-and-targets-in-a-package).


#### FodyPackaging.props

<!-- snippet: FodyPackaging.props -->
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
  <ItemGroup>
    <!-- fake reference to the weaver project to work around https://github.com/Microsoft/msbuild/issues/2661 -->
    <ProjectReference Include="..\$(PackageId)\$(PackageId).csproj"
                      PrivateAssets="All"
                      Condition="$(TargetFramework)=='fake'" />
  </ItemGroup>
</Project>
```
<!-- endsnippet -->


#### FodyPackaging.targets

<!-- snippet: FodyPackaging.targets -->
```targets
<Project>
  <Target Name="IncludeFodyFiles">
    <ItemGroup>
      <NetClassicFilesToInclude Include="$(WeaverDirPath)\net4*\$(PackageId).dll" />
      <NetStandardFilesToInclude Include="$(WeaverDirPath)\netstandard2*\$(PackageId).dll" />
    </ItemGroup>

    <Error Text="FodyPackaging: No NetClassic weavers found to include in package. Maybe the build order is wrong?"
           Condition="'@(NetClassicFilesToInclude)'==''" />
    <Error Text="FodyPackaging: No NetStandard weavers found to include in package. Maybe the build order is wrong?"
           Condition="'@(NetStandardFilesToInclude)'==''" />

    <ItemGroup>
      <NetClassicFilesToInclude Include="$(WeaverDirPath)\net4*\$(PackageId).xcf" />
      <NetStandardFilesToInclude Include="$(WeaverDirPath)\netstandard2*\$(PackageId).xcf" />
      <NetClassicFilesToInclude Include="$(WeaverDirPath)\net4*\$(PackageId).pdb" />
      <NetStandardFilesToInclude Include="$(WeaverDirPath)\netstandard2*\$(PackageId).pdb" />

      <TfmSpecificPackageFile Include="@(NetClassicFilesToInclude)"
                              PackagePath="netclassicweaver\%(Filename)%(Extension)" />
      <TfmSpecificPackageFile Include="@(NetStandardFilesToInclude)"
                              PackagePath="netstandardweaver\%(Filename)%(Extension)" />
      <TfmSpecificPackageFile Include="$(WeaverPropsFile)"
                              PackagePath="build\$(PackageId).props" />
    </ItemGroup>
  </Target>
</Project>
```
<!-- endsnippet -->


### Weaver.props

Included in the consuming package to facilitate [addin discovery](addin-discovery.md).

<!-- snippet: Weaver.props -->
```props
<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

  <PropertyGroup>
    <WeaverRuntimeToken Condition="$(MSBuildRuntimeType) != 'Core'">netclassicweaver</WeaverRuntimeToken>
    <WeaverRuntimeToken Condition="$(MSBuildRuntimeType) == 'Core'">netstandardweaver</WeaverRuntimeToken>
  </PropertyGroup>

  <ItemGroup>
    <WeaverFiles Include="$(MsBuildThisFileDirectory)..\$(WeaverRuntimeToken)\$(MSBuildThisFileName).dll" />
  </ItemGroup>

</Project>
```
<!-- endsnippet -->

