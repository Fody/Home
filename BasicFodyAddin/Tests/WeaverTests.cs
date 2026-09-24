using System;
using Fody;
using TestResult = Fody.TestResult;

#region WeaverTests

// tests that weave into a shared folder must not run in parallel
[NotInParallel]
public class WeaverTests
{
    static TestResult testResult;

    static WeaverTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
    }

    [Test]
    public async Task ValidateHelloWorldIsInjected()
    {
        var type = testResult.Assembly.GetType("TheNamespace.Hello");
        var instance = (dynamic)Activator.CreateInstance(type);

        await Assert.That((string)instance.World()).IsEqualTo("Hello World");
    }
}

#endregion
