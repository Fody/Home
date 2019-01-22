# Write an Addin


## Naming conventions

Fody provides a set of tools that simplify the building of a weaver addin. To benefit from of all this functionality, follow this naming conventions:

|                  | |
|------------------|-|
| Repository Name: | `https://github.com/MyOrganization/MyAddin.Fody` |
| Solution Name:   | MyAddin.Fody |
| Weaver project Name: | MyAddin.Fody |
| Weaver runtime project name: | MyAddin |


## Building a weaver from the scratch

Create a solution with two projects, a `Directory.Build.props` file and an `appveyor.yml` file:

![sample-solution.png](sample-solution.png)

 * In this sample the '.props' have been split into two files, to have the project specific settings in their own file.

---
MyAddin.csproj:

 * In this project you can host attributes that users apply to their code to control your addin.
 * This project also generates your final NuGet package.
 * This project must references the `FodyPackaging` package.
 * The target frameworks depend on what targets you weaver can support (see [Supported Runtimes And Ide](supported-runtimes-and-ide.md))

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net40;netstandard1.0;netstandard2.0</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FodyPackaging"
                      Version="3.2.17"
                      PrivateAssets="All"/>
    <PackageReference Include="Fody"
                      Version="3.2.17"
                      PrivateAssets="None"/>
  </ItemGroup>

</Project>
```

---

MyAddin.Fody.csproj:

 * This project contains your weaving code.
 * This project must references the `FodyHelpers` package.
 * Target frameworks should be .Net4.6 and Netstandard 2.0.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFrameworks>net46;netstandard2.0</TargetFrameworks>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="FodyHelpers"
                      Version="3.2.17" />
  </ItemGroup>

</Project>
```

---

Directory.Build.props

 * This file contains the project specific properties like name and version.
 * It imports the `Common.props`

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <PropertyGroup>
    <Copyright>Copyright Me $([System.DateTime]::UtcNow.ToString(yyyy)).</Copyright>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <Product>MyAddin.Fody</Product>
    <Version>1.0</Version>
    <Description>What my addin does.</Description>
  </PropertyGroup>

  <Import Project="Common.props" />

</Project>
```

---

Common.props

 * This file contains properties that apply to any weaver of your organization.
 * It contains some common stuff that you may not need in all cases, but can be simply ignored.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>

  <PropertyGroup>
    <Authors>Me</Authors>
    <Company>My Company</Company>
    <Title Condition="'$(Title)' == ''">$(Product)</Title>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <GitHubOrganization>MyNameAtGithub</GitHubOrganization>
  </PropertyGroup>

  <PropertyGroup Condition="'$(TargetFramework)' == 'net40-client'">
    <TargetFrameworkIdentifier>.NETFramework</TargetFrameworkIdentifier>
    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
    <TargetFrameworkProfile>client</TargetFrameworkProfile>
  </PropertyGroup>

  <PropertyGroup Condition="'$(TargetFramework)' == 'portable40-net40+sl5+win8+wp8+wpa81'">
    <TargetFrameworkIdentifier>.NETPortable</TargetFrameworkIdentifier>
    <TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
    <TargetFrameworkProfile>Profile328</TargetFrameworkProfile>
    <DefineConstants>$(DefineConstants);PORTABLE</DefineConstants>
  </PropertyGroup>

  <ItemGroup Condition="'$(TargetFramework)' == 'portable40-net40+sl5+win8+wp8+wpa81'">
    <Reference Include="System" />
    <Reference Include="System.Core" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="JetBrains.Annotations" Version="2018.2.1" PrivateAssets="All" />
    <PackageReference Include="Microsoft.SourceLink.GitHub" Version="1.0.0-beta-63127-02" PrivateAssets="All" />
  </ItemGroup>

  <ItemGroup>
    <WeaverFiles Include="$(SolutionDir)$(SolutionName)\bin\$(Configuration)\Net46\$(SolutionName).dll" />
  </ItemGroup>

  <ItemGroup>
    <TfmSpecificPackageFile Include="build\*" PackagePath="build" />
  </ItemGroup>

</Project>
```

---

appveyor.yml

 * this file enables you to setup CI builds on [AppVeyor](https://www.appveyor.com/) with only a few clicks.

```yml
image: Visual Studio 2017
configuration: Release
platform: Any CPU
build_script:
- cmd: MSBuild -restore /p:Configuration=Release /verbosity:minimal
artifacts:
- path: nugets/*.nupkg
```

---

Now the scaffold has been set up. This solution creates a NuGet package including all necessary files.


## The weaver assembly

 * The weaver assembly is the assembly suffixed with ".Fody".
 * It should not have any runtime dependencies (excluding Mono Cecil); runtime dependencies should be combined using e.g. [ILMerge](https://github.com/dotnet/ILMerge) and the `/Internalize` flag.
 * The assembly must contain a public class named 'ModuleWeaver'. The namespace does not matter.


### The ModuleWeaver Class

Add a public class named 'ModuleWeaver', which derives from the `BaseModuleWeaver` class provided by the `FodyHelpers` assembly, to the project.

 * Inherit from `BaseModuleWeaver`.
 * The class must be public, non static, and not abstract.
 * Have an empty constructor.

For example the minimum class would look like this

snippet: ModuleWeaver


### Throwing exceptions

When writing an addin there are a points to note when throwing an Exception.

 * Exceptions thrown from an addin will be caught and interpreted as a build error. So this will stop the build.
 * The exception information will be logged to the MSBuild `BuildEngine.LogErrorEvent` method.
 * If the exception type is `WeavingException` then it will be interpreted as an "error". So the addin is explicitly throwing an exception with the intent of stopping processing and logging a simple message to the build log. In this case the message logged will be the contents of `WeavingException.Message` property. If the `WeavingException` has a property `SequencePoint` then that information will be passed to the build engine so a user can navigate to the error.
 * If the exception type is *not* a `WeavingException` then it will be interpreted as an "unhandled exception". So something has gone seriously wrong with the addin. It most likely has a bug. In this case message logged be much bore verbose and will contain the full contents of the Exception. The code for getting the message can be found here in [ExceptionExtensions](https://github.com/Fody/Fody/blob/master/FodyCommon/ExceptionExtensions.cs).


### Configuration via FodyWeavers.xml

This file exists at a project level in the users target project and is used to pass configuration to the 'ModuleWeaver'.

So if the FodyWeavers.xml file contains the following

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <Virtuosity MyProperty="PropertyValue"/>
</Weavers>
```

The Config property of the ModuleWeaver will be an XElement containing

```xml
<Virtuosity MyProperty="PropertyValue"/>
```


### Supporting intellisense for FodyWeavers.xml

To simplify configuration Fody will create or update a schema file (FodyWeavers.xsd) for every FodyWeavers.xml during compile, 
containing all detected weavers. Every weaver now can provide a schema fragment describing it's individual properties and content
that can be set. This file must be part of the weaver project and named `<project name>.xcf`.
It contains the element describing the type of the configuration node. 
The file must be published side by side with the weaver file; however FodyPackaging will configure this correctly, you just have to 
provide the file.

Sample content of the `Virtuosity.Fody.xcf`:

```xml
<xs:complexType xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xs:all>
    <xs:element name="Content" type="xs:string" minOccurs="0" maxOccurs="1">
      <xs:annotation>
        <xs:documentation>This is the documentation for the content</xs:documentation>
      </xs:annotation>
    </xs:element>
  </xs:all>
  <xs:attribute name="MyProperty" type="xs:string">
    <xs:annotation>
      <xs:documentation>This is the documentation for my property</xs:documentation>
    </xs:annotation>
  </xs:attribute>
  <xs:attribute name="AnotherProperty" type="xs:string" >
    <xs:annotation>
      <xs:documentation>This is the documentation for another property</xs:documentation>
    </xs:annotation>
  </xs:attribute>
</xs:complexType>
```

Fody will then combine all `.xcf` fragments with the weavers information to the final `.xsd`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xs:element name="Weavers">
    <xs:complexType>
      <xs:all>
        <xs:element name="Virtuosity" minOccurs="0" maxOccurs="1">
          <xs:complexType>
            <xs:all>
              <xs:element name="Content" type="xs:string" minOccurs="0" maxOccurs="1">
                <xs:annotation>
                  <xs:documentation>This is the documentation for the content</xs:documentation>
                </xs:annotation>
              </xs:element>
            </xs:all>
            <xs:attribute name="MyProperty" type="xs:string">
              <xs:annotation>
                <xs:documentation>This is the documentation for my property</xs:documentation>
              </xs:annotation>
            </xs:attribute>
            <xs:attribute name="AnotherProperty" type="xs:string">
              <xs:annotation>
                <xs:documentation>This is the documentation for another property</xs:documentation>
              </xs:annotation>
            </xs:attribute>
          </xs:complexType>
        </xs:element>
        <xs:element name="NullGuard" minOccurs="0" maxOccurs="1" type="xs:anyType" />
        <xs:element name="JetBrainsAnnotations" minOccurs="0" maxOccurs="1" type="xs:anyType" />
      </xs:all>
      <xs:attribute name="VerifyAssembly" type="xs:boolean">
        <xs:annotation>
          <xs:documentation>'true' to run assembly verification on the target assembly after all weavers have been finished.</xs:documentation>
        </xs:annotation>
      </xs:attribute>
      <xs:attribute name="VerifyIgnoreCodes" type="xs:string">
        <xs:annotation>
          <xs:documentation>A comma separated list of error codes that can be safely ignored in assembly verification.</xs:documentation>
        </xs:annotation>
      </xs:attribute>
    </xs:complexType>
  </xs:element>
</xs:schema>
```


## Deployment

Addins are deployed through [NuGet](https://nuget.org/) packages.

 * The package should contain two weaver assemblies in the folders `netclassicweaver` and `netstandardweaver` to support both .Net Classic and .Net Core.
 * The package should contain a runtime library, compiled for every supported framework, under the `lib` folder.
 * The package should contain an MSBbuild .props file in the `build` folder that registers the weaver at compile time.
   The name of the file must be  the package id with the `.props` extension. See [Addin Search Strategies](AddinSearchPaths) for details.
 * The id of the package and the name of the weaver assembly should be the same and be suffixed with ".Fody". For example the [Virtuosity NuGet package](https://nuget.org/packages/Virtuosity.Fody/) is named `Virtuosity.Fody` and contains the weaver assembly `Virtuosity.Fody.dll` and the run time assembly `Virtuosity.dll`.
 * The package should have a single dependency on **only** the [Fody NuGet package](https://nuget.org/packages/Fody/). **Do not add any other NuGet dependencies as Fody does not support loading these files at compile time.**


## Design guidelines

A weaver only manipulates with the IL code, so generally it's runtime agnostic. However if the injected code adds references to runtime methods, the weaved code won't work with any runtime that does not provide this method.

A weaver will limit the required target runtime by the version(s) of the build time assembly - the one that is in the lib folder and gets referenced by the target project - that it provides.

It's good practice to only demand what is really required. E.g. if only need to read a file, don't open it in write mode. Same applies to your weaver - if it only uses Net4.0 methods, it should not demand Net4.5.

There are several reasons to not limit the usage of a weaver to a higher target framework than really required. Users of your weaver may just not be able to change the target framework of their build output to something higher; one reason would be that the output is used in many other projects maintained by other teams, and changing the target framework would set off an avalanche of compatibility issues. A popular example is e.g. [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/). Even the latest package still supports Net2.0, just because removing it could introduce unknown side effects. Such a library could not make use of Fody at all, if the weaver demands Net4.5 without any need.

To easily verify that a weaver is compatible with the specified frameworks, add a simple smoke-test like in the sample [BasicFodyAddin](https://github.com/Fody/BasicFodyAddin/tree/master/SmokeTest) - ensure it covers enough code to be meaningful.

**Once a weaver is published, only increase the supported framework versions when truly necessary.**

If some user has a project that targets framework version X.0 and has already used your weaver, NuGet will mess up the whole project when trying to update a weaver that longer supports version X.0, leaving the users project in an unrecoverable state with mixed netclassic and netstandard references.