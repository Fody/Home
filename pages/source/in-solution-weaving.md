# Weaving code inside a solution


## Introduction

Sometimes it is necessary to have weaving code specific to a solution. In this case it is desirable to have that code exist in the same solution it is editing.

- _Note: Starting with Fody 4.0 the implicit lookup of weavers by naming conventions is no longer supported. 
  Follow the instructions below on how do migrate projects to continue using in solution weavers._


## Weaving inside a solution

Fody will [look for weavers by examining the `WeaverFiles` items](addin-search-paths.md) of the project. To use a custom weaver in a project, add an entry to the project file:

```xml
<ItemGroup>
  <WeaverFiles
      Include="$(SolutionDir)Weavers\bin\$(Configuration)\Net46\Weavers.dll"
      WeaverClassNames="MyWeaver1;MyWeaver2" />
</ItemGroup>
```

The `WeaverClassNames` entry is optional. If not specified, Fody will look for a class named `ModuleWeaver`.
If the weaver assembly contains more than one weaver, specify the class names of all the weavers to use in the target project.

The [Integration](https://github.com/Fody/Fody/tree/master/Integration) directory provides examples of this - refer to `WithOnlyInSolutionWeaver` and `WithNugetAndInSolutionWeavers` projects. **Note that the main [Fody Solution](https://github.com/Fody/Fody/blob/master/Fody.sln) needs to be build prior to using the Integration solution.**

To enable in-solution weaving:

  1. Add a project named e.g. 'Weavers' to the solution.
  1. Add a class named `ModuleWeaver` to the project. See [ModuleWeaver](ModuleWeaver).
     _Note: The `ModuleWeaver` is the default name for the weaver class. It is also possible to use multiple weavers with different class names by specifying their class names in the `WeaverClassNames` attribute._
  1. Change the solution build order so the 'Weavers' project is built before the projects using it.
     _Note: Do not add a project reference to the weaver_
  1. If a weaver class is explicitly specified in the `WeaverClassNames` attribute, Fody expects the configuration entry in the `FodyWeavers.xml` file to be named like the class. For example, if the class name is `NamedWeaver`:

```xml
<Weavers>
  <NamedWeaver />
</Weavers>
```