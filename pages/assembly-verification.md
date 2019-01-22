## Assembly Verification 

Post build verification via [PeVerify](https://msdn.microsoft.com/en-us/library/62bwd2yd.aspx) is supported.

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
