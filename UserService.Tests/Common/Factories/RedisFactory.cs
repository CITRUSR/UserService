using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace UserService.Tests.Common.Factories;

public class RedisFactory
{
    public static RedisCache Create(ushort port)
    {
        RedisCacheOptions options = new RedisCacheOptions
        {
            Configuration = $"localhost:{port}",
            InstanceName = "MyRedisCache",
        };

        return new RedisCache(options);
    }
}
