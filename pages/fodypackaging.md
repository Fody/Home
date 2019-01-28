# FodyPackaging NuGet Package

NuGet Package: [FodyPackaging](https://www.nuget.org/packages/FodyPackaging/)


## MSBuild props and targets

The below files are include as [MSBuild props and targets in a package](https://docs.microsoft.com/en-us/nuget/create-packages/creating-a-package#including-msbuild-props-and-targets-in-a-package).


### FodyPackaging.props

<!-- snippet: FodyPackaging.props -->
```props
<Project>
  <PropertyGroup>
    <PackageId Condition="'$(PackageId)' == ''">$(MSBuildProjectName).Fody</PackageId>
    <PackageTags Condition="'$(PackageTags)' == ''">ILWeaving, Fody, Cecil, AOP</PackageTags>
    <FodySolutionDir Condition="'$(FodySolutionDir)' == '' AND '$(SolutionDir)' != '*Undefined*'">$(SolutionDir)</FodySolutionDir>
    <FodySolutionDir Condition="'$(FodySolutionDir)' == ''">$(MSBuildProjectDirectory)</FodySolutionDir>
    <FodySolutionDir Condition="!HasTrailingSlash('$(FodySolutionDir)')">$(FodySolutionDir)\</FodySolutionDir>
    <PackageOutputPath Condition="'$(PackageOutputPath)' == ''">$(FodySolutionDir)nugets</PackageOutputPath>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <TargetsForTfmSpecificContentInPackage Condition="'$(SkipPackagingDefaultFiles)' != 'true'">$(TargetsForTfmSpecificContentInPackage);IncludeFodyFiles</TargetsForTfmSpecificContentInPackage>
    <GeneratePackageOnBuild>true</GeneratePackageOnBuild>
    <NoPackageAnalysis>true</NoPackageAnalysis>
    <NoWarn>$(NoWarn);NU5118</NoWarn>
    <DisableFody>true</DisableFody>
    <WeaverDirPath Condition="'$(WeaverDirPath)' == ''">..\$(PackageId)\bin\$(Configuration)\</WeaverDirPath>
    <WeaverPropsFile Condition="'$(WeaverPropsFile)' == ''">$(MSBuildThisFileDirectory)..\Weaver.props</WeaverPropsFile>
    <GitHubOrganization Condition="'$(GitHubOrganization)' == ''">Fody</GitHubOrganization>
    <LocalGitRootFolder Condition="'$(LocalGitRootFolder)' == ''">$([MSBuild]::GetDirectoryNameOfFileAbove('$(MSBuildProjectDirectory)', '.git\index'))</LocalGitRootFolder>
    <LocalGitRootFolder Condition="'$(LocalGitRootFolder)' == ''">$(FodySolutionDir)</LocalGitRootFolder>
    <LocalGitRootFolder Condition="!HasTrailingSlash('$(LocalGitRootFolder)')">$(LocalGitRootFolder)\</LocalGitRootFolder>
  </PropertyGroup>

  <ItemGroup>
    <PackageIconFile Include="$(LocalGitRootFolder)*icon*.png" />
    <PackageLicenseFile Include="$(LocalGitRootFolder)*license*" />
  </ItemGroup>

  <PropertyGroup>
    <PackageIconFileName Condition="'$(PackageIconFileName)' == ''">@(PackageIconFile->'%(Filename)%(Extension)')</PackageIconFileName>
    <PackageLicenseFileName Condition="'$(PackageLicenseFileName)' == ''">@(PackageLicenseFile->'%(Filename)%(Extension)')</PackageLicenseFileName>
  </PropertyGroup>

  <PropertyGroup Condition="'$(GitHubOrganization)' == 'Fody'">
    <PackageIconUrl Condition="'$(PackageIconUrl)' == ''">https://raw.githubusercontent.com/Fody/$(SolutionName)/master/package_icon.png</PackageIconUrl>
    <PackageLicenseExpression Condition="'$(PackageLicenseUrl)' == '' AND '$(PackageLicenseExpression)' == '' AND '$(PackageLicenseFile)' == ''">MIT</PackageLicenseExpression>
    <PackageProjectUrl Condition="'$(PackageProjectUrl)' == ''">http://github.com/Fody/$(SolutionName)</PackageProjectUrl>
  </PropertyGroup>

  <PropertyGroup Condition="'$(GitHubOrganization)' != 'Fody'">
    <PackageProjectUrl Condition="'$(PackageProjectUrl)' == ''">http://github.com/$(GitHubOrganization)/$(PackageId)</PackageProjectUrl>
    <PackageIconUrl Condition="'$(PackageIconUrl)' == '' And '$(PackageIconFileName)' != ''">https://raw.githubusercontent.com/$(GitHubOrganization)/$(PackageId)/master/$(PackageIconFileName)</PackageIconUrl>
    <PackageLicenseFile Condition="'$(PackageLicenseExpression)' == '' AND '$(PackageLicenseFile)' == '' AND '$(PackageLicenseUrl)' == '' AND '$(PackageLicenseFileName)' != ''">$(PackageLicenseFileName)</PackageLicenseFile>
  </PropertyGroup>

  <ItemGroup>
    <!-- this project targets netstandard2.0 (unnecessarily) and has the below ref to work around this bug https://github.com/Microsoft/msbuild/issues/2661 -->
    <ProjectReference Include="..\$(PackageId)\$(PackageId).csproj" PrivateAssets="All" Condition="$(TargetFramework)=='fake'" />
  </ItemGroup>

</Project>

```
<!-- endsnippet -->


### FodyPackaging.targets

<!-- snippet: FodyPackaging.targets -->
```targets
<Project>
  <Target Name="IncludeFodyFiles">
    <ItemGroup>
      <NetClassicFilesToInclude Include="$(WeaverDirPath)\net4*\$(PackageId).dll" />
      <NetStandardFilesToInclude Include="$(WeaverDirPath)\netstandard2*\$(PackageId).dll" />
    </ItemGroup>

    <Error Text="FodyPackaging: No NetClassic weavers found to include in package. Maybe the build order is wrong?" Condition="'@(NetClassicFilesToInclude)'==''" />
    <Error Text="FodyPackaging: No NetStandard weavers found to include in package. Maybe the build order is wrong?" Condition="'@(NetStandardFilesToInclude)'==''" />

    <ItemGroup>
      <NetClassicFilesToInclude Include="$(WeaverDirPath)\net4*\$(PackageId).xcf" />
      <NetStandardFilesToInclude Include="$(WeaverDirPath)\netstandard2*\$(PackageId).xcf" />
      <NetClassicFilesToInclude Include="$(WeaverDirPath)\net4*\$(PackageId).pdb" />
      <NetStandardFilesToInclude Include="$(WeaverDirPath)\netstandard2*\$(PackageId).pdb" />

      <TfmSpecificPackageFile Include="@(NetClassicFilesToInclude)" PackagePath="netclassicweaver\%(Filename)%(Extension)" />
      <TfmSpecificPackageFile Include="@(NetStandardFilesToInclude)" PackagePath="netstandardweaver\%(Filename)%(Extension)" />
      <TfmSpecificPackageFile Include="$(WeaverPropsFile)" PackagePath="build\$(PackageId).props" />
    </ItemGroup>
  </Target>
</Project>

```
<!-- endsnippet -->


## Weaver.props

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

