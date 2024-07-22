using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Testcontainers.Redis;

namespace UserService.Tests.Common;

public class RedisTest : CommonTest, IAsyncLifetime
{
    protected RedisCache Redis { get; set; }
    private INetwork _network;

    private RedisContainer _redisContainer;

    public async Task InitializeAsync()
    {
        _network = CreateNetwork();

        _redisContainer = CreateRedisContainer(_network);

        await _redisContainer.StartAsync();

        Redis = CreateRedisCache();
    }

    public async Task DisposeAsync()
    {
        Redis.Dispose();

        await _redisContainer.DisposeAsync();

        await _network.DeleteAsync();
        await _network.DisposeAsync();
    }

    private INetwork CreateNetwork()
    {
        return new NetworkBuilder()
            .WithDriver(NetworkDriver.Bridge)
            .WithName("test-network")
            .Build();
    }

    private RedisContainer CreateRedisContainer(INetwork network)
    {
        return new RedisBuilder()
            .WithNetwork(network)
            .WithImage("redis:7.4-rc")
            .WithPortBinding("6370", "6379")
            .WithHostname("redis")
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