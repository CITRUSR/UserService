using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Testcontainers.Redis;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;

namespace UserService.Tests.Common;

public class RedisTest(DatabaseFixture databaseFixture) : CommonTest(databaseFixture), IAsyncLifetime
{
    protected ICacheService CacheService { get; set; }

    private RedisCache _redisCache;
    private RedisContainer _redisContainer;

    public async Task InitializeAsync()
    {
        _redisContainer = CreateRedisContainer();

        await _redisContainer.StartAsync();

        _redisCache = CreateRedisCache();
        CacheService = new CacheService(_redisCache);
    }

    public async Task DisposeAsync()
    {
        _redisCache.Dispose();

        await _redisContainer.DisposeAsync();
    }

    private RedisContainer CreateRedisContainer()
    {
        return new RedisBuilder()
            .WithImage("redis:7.4-rc")
            .WithExposedPort("6379")
            .WithHostname("redis")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
            .Build();
    }

    private RedisCache CreateRedisCache()
    {
        var redisPort = _redisContainer.GetMappedPublicPort("6379");

        RedisCacheOptions options = new RedisCacheOptions
        {
            Configuration = $"localhost:{redisPort}",
            InstanceName = "MyRedisCache",
        };

        return new RedisCache(options);
    }
}