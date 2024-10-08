﻿using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Testcontainers.Redis;
using UserService.Application.Abstraction;
using UserService.Persistance.Cache;
using UserService.Tests.Common.Factories;

namespace UserService.Tests.Common;

public class RedisFixture : IAsyncLifetime
{
    public ICacheService CacheService { get; private set; }
    private RedisCache _redisCache;
    private RedisContainer _redisContainer = new RedisBuilder()
        .WithImage("redis:7.4-rc")
        .WithExposedPort("6379")
        .WithHostname("redis")
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(6379))
        .Build();

    private readonly IOptions<CacheOptions> options;

    public RedisFixture()
    {
        var cacheOptions = new CacheOptions { SlidingExpirationTime = 60 };

        options = Options.Create(cacheOptions);
    }

    public async Task InitializeAsync()
    {
        await _redisContainer.StartAsync();

        _redisCache = RedisFactory.Create(_redisContainer.GetMappedPublicPort("6379"));

        CacheService = new CacheService(_redisCache, options);
    }

    public async Task DisposeAsync()
    {
        _redisCache.Dispose();

        await _redisContainer.DisposeAsync();
    }
}
