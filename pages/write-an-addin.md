# Write an Addin


## Moving parts of a Fody Addin


### Lib/Reference project

BasicFodyAddin.csproj:

 * Contain all classes used for compile time to control the addin behavior at compile time. Often this is in the form of Attributes. 
 * Generally any usage and reference to this project is removed at compile time so it is not needed as part of application deployment.
 * The target frameworks depend on what targets the weaver can support (see [Supported Runtimes And Ide](supported-runtimes-and-ide.md))

This project is also used to produce the NuGet package. To achieve this the project consumes two NuGets:

 * [Fody](https://www.nuget.org/packages/Fody/) with `PrivateAssets="None"`. This results in producing NuGet package having a dependency on Fody with all `include="All"` in the nuspec. Note that while this project consumes the Fody NuGet, weaving is not performed on this project. This is due to the FodyPackaging NuGet (see below) including `<DisableFody>true</DisableFody>` in the MSBuild pipeline.
 * [FodyPackaging](https://www.nuget.org/packages/FodyPackaging/) with `PrivateAssets="All"`. This results in a NuGet package being produced by this project, but no dependency on FodyPackaging in the resulting NuGet package.

The produced NuGet package will:

 * Be named with `.Fody` suffix. This project should also contain all appropriate [NuGet metadata properties](https://docs.microsoft.com/en-us/dotnet/core/tools/csproj#nuget-metadata-properties). Many of these properties have defaults in [FodyPackaging](https://github.com/Fody/Fody/blob/master/FodyPackaging/build/FodyPackaging.props), but can be overridden.
 * Target, and hence support from a consumer perspective, the same frameworks that this project targets. 
 * Be created in a directory named `nugets` at the root of the solution.

<!-- snippet: BasicFodyAddin.csproj -->
```csproj
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net452;netstandard2.0</TargetFrameworks>
    <SignAssembly>true</SignAssembly>
    <AssemblyOriginatorKeyFile>key.snk</AssemblyOriginatorKeyFile>
    <Authors>Simon Cropp</Authors>
    <Copyright>Copyright Simon Cropp $([System.DateTime]::UtcNow.ToString(yyyy)).</Copyright>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <Description>Injects a new type that writes "Hello World".</Description>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Fody"
                      Version="4.0.0-beta.1"
                      PrivateAssets="None" />
    <PackageReference Include="FodyPackaging"
                      Version="4.0.0-beta.1"
                      PrivateAssets="All" />
  </ItemGroup>
</Project>
```
<sup>[snippet source](/BasicFodyAddin/BasicFodyAddin/BasicFodyAddin.csproj#L1-L21)</sup>
<!-- endsnippet -->


### Weaver Project

BasicFodyAddin.Fody.csproj:

This project contains the weaving code.

 * Has a NuGet dependency on [FodyHelpers](https://www.nuget.org/packages/FodyHelpers/).
 * Should not have any runtime dependencies (excluding Mono Cecil); runtime dependencies should be combined using e.g. [ILMerge](https://github.com/dotnet/ILMerge) and the `/Internalize` flag.
 * The assembly must contain a public class named 'ModuleWeaver'. The namespace does not matter.

<!-- snippet: BasicFodyAddin.Fody.csproj -->
```csproj
<?xml version="1.0" encoding="utf-8"?>
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFrameworks>net46;netstandard2.0</TargetFrameworks>
    <DebugType>portable</DebugType>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="FodyHelpers" Version="4.0.0-beta.1" />
  </ItemGroup>
</Project>
```
<sup>[snippet source](/BasicFodyAddin/BasicFodyAddin.Fody/BasicFodyAddin.Fody.csproj#L1-L10)</sup>
<!-- endsnippet -->


#### Target Frameworks

This project must target `net46` for [msbuild.exe](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild) support, and `netstandard2.0` for [dotnet build](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-build) support.


#### Output of the project

It outputs a file named `BasicFodyAddin.Fody`. The '.Fody' suffix is necessary to be picked up by Fody at compile time.


#### ModuleWeaver

ModuleWeaver.cs is where the target assembly is modified. Fody will pick up this type during its processing. Note that the class must be named as `ModuleWeaver`.

`ModuleWeaver` must use the base class of `BaseModuleWeaver` which exists in the [FodyHelpers NuGet](https://www.nuget.org/packages/FodyHelpers/).

 * Inherit from `BaseModuleWeaver`.
 * The class must be public, non static, and not abstract.
 * Have an empty constructor.

<!-- snippet: ModuleWeaver -->
```cs
public class ModuleWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var objectType = FindType("System.Object");
        var objectImport = ModuleDefinition.ImportReference(objectType);
        ModuleDefinition.Types.Add(new TypeDefinition("MyNamespace", "MyType", TypeAttributes.Public, objectImport));
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        return Enumerable.Empty<string>();
    }
}
```
<sup>[snippet source](/src/Docs/Snippets/ModuleWeaver.cs#L6-L23)</sup>
```cs
public class ModuleWeaver : BaseModuleWeaver
{

    public override void Execute()
    {
        var ns = GetNamespace();
        var type = new TypeDefinition(ns, "Hello", TypeAttributes.Public, TypeSystem.ObjectReference);

        AddConstructor(type);

        AddHelloWorld(type);

        ModuleDefinition.Types.Add(type);
        LogInfo("Added type 'Hello' with method 'World'.");
    }



    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
    }


    string GetNamespace()
    {
        var namespaceFromConfig = GetNamespaceFromConfig();
        var namespaceFromAttribute = GetNamespaceFromAttribute();
        if (namespaceFromConfig != null && namespaceFromAttribute != null)
        {
            throw new WeavingException("Configuring namespace from both Config and Attribute is not supported.");
        }

        if (namespaceFromAttribute != null)
        {
            return namespaceFromAttribute;
        }

        return namespaceFromConfig;
    }

    string GetNamespaceFromConfig()
    {
        var namespaceAttribute = Config.Attribute("Namespace");
        if (namespaceAttribute == null)
        {
            return null;
        }

        var value = namespaceAttribute.Value;
        ValidateNamespace(value);
        return value;
    }

    string GetNamespaceFromAttribute()
    {
        var attributes = ModuleDefinition.Assembly.CustomAttributes;
        var namespaceAttribute = attributes
            .SingleOrDefault(x => x.AttributeType.FullName == "NamespaceAttribute");
        if (namespaceAttribute == null)
        {
            return null;
        }

        attributes.Remove(namespaceAttribute);
        var value = (string)namespaceAttribute.ConstructorArguments.First().Value;
        ValidateNamespace(value);
        return value;
    }

    static void ValidateNamespace(string value)
    {
        if (value is null || string.IsNullOrWhiteSpace(value))
        {
            throw new WeavingException("Invalid namespace");
        }
    }

    void AddConstructor(TypeDefinition newType)
    {
        var attributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        var method = new MethodDefinition(".ctor", attributes, TypeSystem.VoidReference);
        var objectConstructor = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition.GetConstructors().First());
        var processor = method.Body.GetILProcessor();
        processor.Emit(OpCodes.Ldarg_0);
        processor.Emit(OpCodes.Call, objectConstructor);
        processor.Emit(OpCodes.Ret);
        newType.Methods.Add(method);
    }

    void AddHelloWorld(TypeDefinition newType)
    {
        var method = new MethodDefinition("World", MethodAttributes.Public, TypeSystem.StringReference);
        var processor = method.Body.GetILProcessor();
        processor.Emit(OpCodes.Ldstr, "Hello World");
        processor.Emit(OpCodes.Ret);
        newType.Methods.Add(method);
    }


    public override bool ShouldCleanReference => true;

}
```
<sup>[snippet source](/BasicFodyAddin/BasicFodyAddin.Fody/ModuleWeaver.cs#L8-L121)</sup>
<!-- endsnippet -->


##### BaseModuleWeaver.Execute

Called to perform the manipulation of the module. The current module can be accessed and manipulated via `BaseModuleWeaver.ModuleDefinition`.

<!-- snippet: Execute -->
```cs
public override void Execute()
{
    var ns = GetNamespace();
    var type = new TypeDefinition(ns, "Hello", TypeAttributes.Public, TypeSystem.ObjectReference);

    AddConstructor(type);

    AddHelloWorld(type);

    ModuleDefinition.Types.Add(type);
    LogInfo("Added type 'Hello' with method 'World'.");
}
```
<sup>[snippet source](/BasicFodyAddin/BasicFodyAddin.Fody/ModuleWeaver.cs#L12-L27)</sup>
<!-- endsnippet -->


##### BaseModuleWeaver.GetAssembliesForScanning

Called by Fody when it is building up a type cache for lookups. This method should return all possible assemblies that the weaver may require while resolving types. In this case BasicFodyAddin requires `System.Object`, so `GetAssembliesForScanning` returns `netstandard` and `mscorlib`. It is safe to return assembly names that are not used by the current target assembly as these will be ignored.

To use this type cache, a `ModuleWeaver` can call `BaseModuleWeaver.FindType` within `Execute` method.

<!-- snippet: GetAssembliesForScanning -->
```cs
public override IEnumerable<string> GetAssembliesForScanning()
{
    yield return "netstandard";
    yield return "mscorlib";
}
```
<sup>[snippet source](/BasicFodyAddin/BasicFodyAddin.Fody/ModuleWeaver.cs#L29-L37)</sup>
<!-- endsnippet -->


##### BaseModuleWeaver.ShouldCleanReference

When `BasicFodyAddin.dll` is referenced by a consuming project, it is only for the purposes configuring the weaving via attributes. As such, it is not required at runtime. With this in mind `BaseModuleWeaver` has an opt in feature to remove the reference, meaning the target weaved application does not need `BasicFodyAddin.dll` at runtime. This feature can be opted in to via the following code in `ModuleWeaver`:

<!-- snippet: ShouldCleanReference -->
```cs
public override bool ShouldCleanReference => true;
```
<sup>[snippet source](/BasicFodyAddin/BasicFodyAddin.Fody/ModuleWeaver.cs#L114-L118)</sup>
<!-- endsnippet -->


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


#### Throwing exceptions

When writing an addin there are a points to note when throwing an Exception.

 * Exceptions thrown from an addin will be caught and interpreted as a build error. So this will stop the build.
 * The exception information will be logged to the MSBuild `BuildEngine.LogErrorEvent` method.
 * If the exception type is `WeavingException` then it will be interpreted as an "error". So the addin is explicitly throwing an exception with the intent of stopping processing and logging a simple message to the build log. In this case the message logged will be the contents of `WeavingException.Message` property. If the `WeavingException` has a property `SequencePoint` then that information will be passed to the build engine so a user can navigate to the error.
 * If the exception type is *not* a `WeavingException` then it will be interpreted as an "unhandled exception". So something has gone seriously wrong with the addin. It most likely has a bug. In this case message logged be much bore verbose and will contain the full contents of the Exception. The code for getting the message can be found here in [ExceptionExtensions](https://github.com/Fody/Fody/blob/master/FodyCommon/ExceptionExtensions.cs).


### Passing config via to FodyWeavers.xml

This file exists at a project level in the users target project and is used to pass configuration to the 'ModuleWeaver'.

So if the FodyWeavers.xml file contains the following:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <BasicFodyAddin Namespace="MyNamespace"/>
</Weavers>
```

The property of the `ModuleWeaver.Config` will be an [XElement](https://docs.microsoft.com/en-us/dotnet/api/system.xml.linq.xelement) containing:

```xml
<BasicFodyAddin Namespace="MyNamespace"/>
```


#### Supporting intellisense for FodyWeavers.xml

Fody will create or update a schema file (FodyWeavers.xsd) for every FodyWeavers.xml during compilation, adding all detected weavers. Every weaver now can provide a schema fragment describing it's individual properties and content that can be set. This file must be part of the weaver project and named `<project name>.xcf`. It contains the element describing the type of the configuration node. The file must be published side by side with the weaver file; however FodyPackaging will configure this correctly based on the convention `WeaverName.Fody.xcf`.

Sample content of the `BasicFodyAddin.Fody.xcf`:

<!-- snippet: BasicFodyAddin.Fody.Xcf -->
```xcf
<?xml version="1.0" encoding="utf-8" ?>
<xs:complexType xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xs:attribute name="Namespace" type="xs:string">
    <xs:annotation>
      <xs:documentation>The namespace to use for the injected type</xs:documentation>
    </xs:annotation>
  </xs:attribute>
</xs:complexType>
```
<sup>[snippet source](/BasicFodyAddin/BasicFodyAddin.Fody/BasicFodyAddin.Fody.xcf#L1-L8)</sup>
<!-- endsnippet -->

Fody will then combine all `.xcf` fragments with the weavers information to the final `.xsd`:

<!-- snippet: FodyWeavers.xsd -->
```xsd
<?xml version="1.0" encoding="utf-8"?>
<xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <!-- This file was generated by Fody. Manual changes to this file will be lost when your project is rebuilt. -->
  <xs:element name="Weavers">
    <xs:complexType>
      <xs:all>
        <xs:element name="BasicFodyAddin" minOccurs="0" maxOccurs="1">
          <xs:complexType>
            <xs:attribute name="Namespace" type="xs:string">
              <xs:annotation>
                <xs:documentation>A list of assembly names to exclude from the default action of "embed all Copy Local references", delimited with |</xs:documentation>
              </xs:annotation>
            </xs:attribute>
          </xs:complexType>
        </xs:element>
      </xs:all>
      <xs:attribute name="VerifyAssembly" type="xs:boolean">
        <xs:annotation>
          <xs:documentation>'true' to run assembly verification (PEVerify) on the target assembly after all weavers have been executed.</xs:documentation>
        </xs:annotation>
      </xs:attribute>
      <xs:attribute name="VerifyIgnoreCodes" type="xs:string">
        <xs:annotation>
          <xs:documentation>A comma-separated list of error codes that can be safely ignored in assembly verification.</xs:documentation>
        </xs:annotation>
      </xs:attribute>
      <xs:attribute name="GenerateXsd" type="xs:boolean">
        <xs:annotation>
          <xs:documentation>'false' to turn off automatic generation of the XML Schema file.</xs:documentation>
        </xs:annotation>
      </xs:attribute>
    </xs:complexType>
  </xs:element>
</xs:schema>
```
<sup>[snippet source](/BasicFodyAddin/SmokeTest/FodyWeavers.xsd#L1-L34)</sup>
<!-- endsnippet -->


### AssemblyToProcess Project

A target assembly to process and then validate with unit tests.


### Tests Project

Contains all tests for the weaver.

The project has a NuGet dependency on [FodyHelpers](https://www.nuget.org/packages/FodyHelpers/).

It has a reference to the `AssemblyToProcess` project, so that `AssemblyToProcess.dll` is copied to the bin directory of the test project.

FodyHelpers contains a utility [WeaverTestHelper](https://github.com/Fody/Fody/blob/master/FodyHelpers/Testing/WeaverTestHelper.cs) for executing test runs on a target assembly using a ModuleWeaver.

A test can then be run as follows:

<!-- snippet: WeaverTests -->
```cs
public class WeaverTests
{
    static TestResult testResult;

    static WeaverTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    [Fact]
    public void ValidateHelloWorldIsInjected()
    {
        var type = testResult.Assembly.GetType("TheNamespace.Hello");
        var instance = (dynamic)Activator.CreateInstance(type);

        Assert.Equal("Hello World", instance.World());
    }
}
```
<sup>[snippet source](/BasicFodyAddin/Tests/WeaverTests.cs#L5-L27)</sup>
<!-- endsnippet -->

By default `ExecuteTestRun` will perform a [PeVerify](https://docs.microsoft.com/en-us/dotnet/framework/tools/peverify-exe-peverify-tool) on the resultant assembly.


## Build Server


### AppVeyor

To configure an adding to build using [AppVeyor](https://www.appveyor.com/) use the following `appveyor.yml`:

<!-- snippet: appveyor.yml -->
```yml
image: Visual Studio 2017
build_script:
- cmd: dotnet build --configuration Release
test:
  assemblies:
    - '**\*Tests.dll'
artifacts:
- path: nugets\**\*.nupkg
```
<sup>[snippet source](/BasicFodyAddin/appveyor.yml#L1-L8)</sup>
<!-- endsnippet -->


## Usage


### NuGet installation

Install the [BasicFodyAddin.Fody NuGet package](https://nuget.org/packages/BasicFodyAddin.Fody/) and update the [Fody NuGet package](https://nuget.org/packages/Fody/):

```powershell
PM> Install-Package Fody
PM> Install-Package BasicFodyAddin.Fody
```

The `Install-Package Fody` is required since NuGet always defaults to the oldest, and most buggy, version of any dependency.


### Add to FodyWeavers.xml

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Weavers>
  <BasicFodyAddin />
</Weavers>
```


## Deployment

Addins are deployed through [NuGet](https://nuget.org/) packages. The package must:

 * Contain two weaver assemblies, one in each of the folders `netclassicweaver` and `netstandardweaver`, to support both .Net Classic and .Net Core.
 * Contain a runtime library, compiled for every supported framework, under the `lib` folder.
 * Contain an MSBbuild .props file in the `build` folder that registers the weaver at compile time. The name of the file must be  the package id with the `.props` extension. See [Addin Discover](addin-discovery.md) for details.
 * Haven an id with the same name of the weaver assembly should be the same and be suffixed with ".Fody". For example the [Virtuosity NuGet package](https://nuget.org/packages/Virtuosity.Fody/) is named `Virtuosity.Fody` and contains the weaver assembly `Virtuosity.Fody.dll` and the run time assembly `Virtuosity.dll`.
 * Have a single dependency on **only** the [Fody NuGet package](https://nuget.org/packages/Fody/). **Do not add any other NuGet dependencies as Fody does not support loading these files at compile time.**

Note that the addins used via [in-solution-weaving](in-solution-weaving.md) are handled differently.
