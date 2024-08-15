using UserService.Application.Abstraction;

namespace UserService.Tests.Common;

public class CommonTest : IAsyncLifetime, IClassFixture<DatabaseFixture>
{
    protected readonly DbContextHelper DbHelper;
    protected readonly IAppDbContext Context;
    protected readonly Fixture Fixture;
    private readonly DatabaseFixture _databaseFixture;

    public CommonTest(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;

        Context = ContextFactory.Create(_databaseFixture.ConnectionString);

        DbHelper = new DbContextHelper(Context);

        Fixture = new Fixture();
        FixtureInitializer.Initialize(Fixture);
    }

    public async Task InitializeAsync()
    {
        await Context.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.RollbackTransactionAsync();
    }
}
