using VerifyXunit;
using Xunit.Abstractions;

public class DocoUpdater :
    VerifyBase
{
    public DocoUpdater(ITestOutputHelper output) :
        base(output)
    {
    }
}