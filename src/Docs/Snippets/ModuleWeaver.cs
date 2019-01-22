using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

#region ModuleWeaver

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

#endregion