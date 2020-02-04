using System.Collections.Generic;
using System.Linq;
using Fody;

public class MyLoggingWeaver :
    BaseModuleWeaver
{
    public override void Execute()
    {
        // Write a log entry with a specific MessageImportance
        WriteMessage("Message", MessageImportance.High);

        // Write a log entry with the MessageImportance.Low level
        WriteDebug("Message");

        // Write a log entry with the MessageImportance.Normal level
        WriteInfo("Message");

        // Write a warning
        WriteWarning("Message");

        // Write an error
        WriteError("Message");

        var type = ModuleDefinition.GetType("MyType");
        var method = type.Methods.First();

        // Write an error using the first SequencePoint
        // of a method for the line information
        WriteWarning("Message", method);

        // Write an error using the first SequencePoint
        // of a method for the line information
        WriteError("Message", method);

        var sequencePoint = method.DebugInformation.SequencePoints.First();

        // Write an warning using a SequencePoint
        // for the line information
        WriteWarning("Message", sequencePoint);

        // Write an error using a SequencePoint
        // for the line information
        WriteError("Message", sequencePoint);
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        return Enumerable.Empty<string>();
    }
}