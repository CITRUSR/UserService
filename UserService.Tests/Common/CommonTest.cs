using AutoFixture;
using UserService.Persistance;

namespace UserService.Tests.Common;

public class CommonTest : IDisposable
{
    protected readonly AppDbContext Context;
    protected readonly Fixture Fixture;

    public CommonTest()
    {
        Context = ContextFactory.Create();
        Fixture = new Fixture();
    }

    public void Dispose()
    {
        ContextFactory.Destroy(Context);
    }
}