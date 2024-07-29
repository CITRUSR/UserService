using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Persistance;

namespace UserService.Tests.Common;

public class CommonTest : IAsyncLifetime, IClassFixture<DatabaseFixture>
{
    protected readonly IAppDbContext Context;
    protected readonly Fixture Fixture;
    private readonly DatabaseFixture _databaseFixture;

    public CommonTest(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        Context = CreateDbContext();
        Fixture = new Fixture();
    }

    public async Task InitializeAsync()
    {
        await Context.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.RollbackTransactionAsync();
    }

    private IAppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_databaseFixture.ConnectionString)
            .Options;

        var db = new AppDbContext(options);

        db.Database.EnsureCreated();

        return db;
    }
}