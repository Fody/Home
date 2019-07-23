using Xunit.Abstractions;

public class DocoUpdater :
    XunitLoggingBase
{
    public DocoUpdater(ITestOutputHelper output) :
        base(output)
    {
    }
}