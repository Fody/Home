# How re-signing of assemblies is done

The WeavingTask looks for the two settings that indicate a project file has produced a signed assembly


## SignAssembly

```xml
<PropertyGroup>
  <SignAssembly>true</SignAssembly>
</PropertyGroup>
```


## AssemblyOriginatorKeyFile 

```xml
<PropertyGroup>
  <AssemblyOriginatorKeyFile>Key.snk</AssemblyOriginatorKeyFile>
</PropertyGroup>
```


So if `SignAssembly` is true and `AssemblyOriginatorKeyFile` contains a valid path to a key file then the resultant processed assembly will be re-signed.