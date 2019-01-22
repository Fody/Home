## Fody support for defining dependencies in the build order

You do this by defining a property group in your project named `FodyDependsOnTargets`. The content of this property will then be passed to the `DependsOnTargets` of the Fody weaving task. 

http://msdn.microsoft.com/en-us/library/t50z2hka.aspx

> DependsOnTargets: The targets that must be executed before this target can be executed or top-level dependency analysis can occur. Multiple targets are separated by semicolons.

An example use case of this is to force Fody to run after CodeContracts.

```xml
<PropertyGroup>
  <FodyDependsOnTargets>
    CodeContractRewrite
  </FodyDependsOnTargets>
</PropertyGroup>
```
