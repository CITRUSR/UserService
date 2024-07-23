using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Testcontainers.Redis;

namespace UserService.Tests.Common;

public class RedisTest : CommonTest, IAsyncLifetime
{
    protected RedisCache Redis { get; set; }

    private RedisContainer _redisContainer;

    public async Task InitializeAsync()
    {
        _redisContainer = CreateRedisContainer();

        await _redisContainer.StartAsync();

        Redis = CreateRedisCache();
    }

    public async Task DisposeAsync()
    {
        Redis.Dispose();

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