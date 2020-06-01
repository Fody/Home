# Configuration

## Configuring the weavers

Fody requires an XML configuration that lists all weavers to be applied to the project.

 * It specifies the list of weavers that should be applied to the project.
 * Weavers are applied in the order listed in the configuration. 
 * Weavers can also be configured here. See the documentation of each individual weaver to see what configuration options it has.

The format is:

```xml
<Weavers>
  <WeaverA />
  <WeaverB ConfigForWeaverB="Value" />
</Weavers>
```

The `<Weavers>` element is mandatory and supports the following attributes:

 * `VerifyAssembly`: Set to `true` to run PEVerify on the build result. See [Assembly verification](#assembly-verification).
 * `VerifyIgnoreCodes`: A comma-separated list of error codes which should be ignored during assembly verification. See [Assembly verification](#assembly-verification).
 * `GenerateXsd`: Set to `false` to disable generation of the `FodyWeavers.xsd` file which provides [IntelliSense](https://docs.microsoft.com/en-us/visualstudio/ide/using-intellisense) support for `FodyWeavers.xml`. This overrides the `FodyGenerateXsd` MSBuild property.

The content of the `<Weavers>` element is a list of all weavers, where the name of the element corresponds to the weaver name.


### Configuration options

There are several ways to specify the configuration


#### A file in each project directory

The default is a file named `FodyWeavers.xml` in each projects directory. 

 * If it is missing, and no other configuration can be found, a default file will be created the first time the project is built.
 * An XML schema will be created aside of this file, to provide [IntelliSense](https://docs.microsoft.com/en-us/visualstudio/ide/using-intellisense) support for editing the `FodyWeavers.xml`
 * This file must not contain entries for weavers that are not installed. However, entries can be ommited if they are defined in one of the alternate configurations.


#### An MSBuild property in the project file

An alternate way is to add a property named `WeaverConfiguration` in the project file:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    ...
    <SomeProperty>Some Value</SomeProperty>
    <WeaverConfiguration>
      <Weavers>
        <WeaverA />
        <WeaverB ConfigForWeaverB="$(SomeProperty)" />
      </Weavers>
    </WeaverConfiguration>
  </PropertyGroup>
```

The content of this property is the same as described above

 * This has highest precedence, it overrides any entries of the other configurations
 * Use MSBuild logic to dynamically control the behavior
 * Add the configuration e.g. once in the `Directory.build.props` file to share the same configuration among several projects.
 * To support sharing the configuration among several projects, entries for weavers that are not installed for a particular project are ignored, configure the superset of all weavers installed in all projects.
 * [IntelliSense](https://docs.microsoft.com/en-us/visualstudio/ide/using-intellisense) for the configuration is not available here


#### A file in the solution directory

The configuration may be shared among all projects of the solution by adding one file named `FodyWeavers.xml` in the solution directory.

 * [IntelliSense](https://docs.microsoft.com/en-us/visualstudio/ide/using-intellisense) for the configuration is not available here
 * Entries for weavers that are not installed for a particular project are ignored, so the superset can be configured for of all weavers installed in all projects.
 * This option has the lowest precedence and will be overwritten by the other options


### Merging configurations

When multiple configurations are provided, the one with the lowest precedence will control the execution order of the weaver, while the one with the higher precedence will override the content of the configuration node. 

Consider this combination:

`FodyWeavers.xml` in the solution directory:

```xml
<Weavers>
  <WeaverA />
  <WeaverB />
</Weavers>
```

---

`FodyWeavers.xml` in the project directory:

```xml
<Weavers>
  <WeaverC />
  <WeaverB Property1="B1" />
  <WeaverA Property1="A1" Property2="A2" />
</Weavers>
```

---

`WeaverConfiguration` entry in the project:

```xml
<WeaverConfiguration Condition="'$(Configuration)' == 'Debug'">
  <Weavers>
    <WeaverC Property1="C1" />
    <WeaverA Property3="A3" />
  </Weavers>
</WeaverConfiguration>
```

---

This will result in an effective configuration of

```xml
<Weavers>
  <WeaverA Property1="A1" Property2="A2" />
  <WeaverB Property1="B1" />
  <WeaverC />
</Weavers>
```

for non-debug and 

```xml
<Weavers>
  <WeaverA Property3="A3" />
  <WeaverB Property1="B1" />
  <WeaverC Property1="C1" />
</Weavers>
```
for debug builds.

To verify which configuration is active the configuration source can be found in the log when the output verbosity is set to detailed.


## Controlling the behavior of Fody

The following options can be set through MSBuild properties:

 * `DisableFody`: Set to `false` to disable Fody entirely.
 * `FodyGenerateXsd`: Set to `false` to disable generation of the `FodyWeavers.xsd` file which provides [IntelliSense](https://docs.microsoft.com/en-us/visualstudio/ide/using-intellisense) support for `FodyWeavers.xml`.
 * `FodyVerifyAssembly`: Enables [assembly verification](#assembly-verification) with PEVerify.


## Defining dependencies in the build order

This can be done by defining a property group project named `FodyDependsOnTargets`. The content of this property will then be passed to the `DependsOnTargets` of the Fody weaving task.

> DependsOnTargets: The targets that must be executed before this target can be executed or top-level dependency analysis can occur. Multiple targets are separated by semicolons.

An example use case of this is to force Fody to run after [CodeContracts](https://docs.microsoft.com/en-us/dotnet/framework/debug-trace-profile/code-contracts).

```xml
<PropertyGroup>
  <FodyDependsOnTargets>
    CodeContractRewrite
  </FodyDependsOnTargets>
</PropertyGroup>
```


## Assembly verification

Post build verification via [PeVerify](https://docs.microsoft.com/en-us/dotnet/framework/tools/peverify-exe-peverify-tool) is supported.

To enable do one of the following

Add `VerifyAssembly="true"` to FodyWeavers.xml:

```xml
<Weavers VerifyAssembly="true">
  <Anotar.Custom />
</Weavers>
```

Add a build constant with the value of `FodyVerifyAssembly`

To send ignore codes to PeVerify use `VerifyIgnoreCodes`.

```xml
<Weavers VerifyAssembly="true"
         VerifyIgnoreCodes="0x80131869">
  <Anotar.Custom />
</Weavers>
```