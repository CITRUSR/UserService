using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Testcontainers.Redis;
using UserService.Application.Abstraction;
using UserService.Persistance.Cache;
using UserService.Tests.Common.Factories;

namespace UserService.Tests.Common;

public class RedisTest(DatabaseFixture databaseFixture)
    : CommonTest(databaseFixture),
        IAsyncLifetime
{
    protected ICacheService CacheService { get; private set; }
    private RedisCache _redisCache;
    private RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.4-rc")
        .WithExposedPort("6379")
        .WithHostname("redis")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
        .Build();

    public new async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();

        _redisCache = RedisFactory.Create(_redisContainer.GetMappedPublicPort("6379"));
        CacheService = new CacheService(_redisCache);
    }

    public new async Task DisposeAsync()
    {
        _redisCache.Dispose();

        await _redisContainer.DisposeAsync();
    }
}
