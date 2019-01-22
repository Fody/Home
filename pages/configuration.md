## FodyWeavers.xml

Fody requires a `FodyWeavers.xml` file to be present in the project directory. If it is missing, a default file will be created the first time the project is built.

This file specifies the list of weavers that should be applied to the project, and in what order. Weavers can also be configured here.

The file format is:

```XML
<Weavers>
  <WeaverA />
  <WeaverB ConfigForWeaverB="Value" />
</Weavers>
```

The `<Weavers>` element supports the following attributes:

- `VerifyAssembly`: Set to `true` to run PEVerify on the build result. See [Assembly verification](AssemblyVerification).
- `VerifyIgnoreCodes`: A comma-separated list of error codes which should be ignored during assembly verification.
- `GenerateXsd`: Set to `false` to disable generation of the `FodyWeavers.xsd` file which provides IntelliSense support for `FodyWeavers.xml`. This overrides the `FodyGenerateXsd` MSBuild property.

## MSBuild properties

The following options can be set through MSBuild properties:

- `DisableFody`: Set to `false` to disable Fody entirely.
- `FodyGenerateXsd`: Set to `false` to disable generation of the `FodyWeavers.xsd` file which provides IntelliSense support for `FodyWeavers.xml`.

## Build constants

The following build constant is recognized by Fody:

- `FodyVerifyAssembly`: Enables [assembly verification](AssemblyVerification) with PEVerify.
