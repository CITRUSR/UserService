using Testcontainers.PostgreSql;

namespace UserService.Tests.Common;

public class DatabaseFixture : IAsyncLifetime
{
    public string ConnectionString => _postgres.GetConnectionString();

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithUsername("test")
        .WithPassword("test")
        .WithDatabase("testDb")
        .WithExposedPort(5433)
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgres.DisposeAsync();
    }
}
