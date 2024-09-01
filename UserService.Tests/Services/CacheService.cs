using FluentAssertions;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Services;

public class CacheService : IClassFixture<RedisFixture>
{
    private readonly IFixture _fixture;
    private readonly RedisFixture _redisFixture;

    public CacheService(RedisFixture redisFixture)
    {
        _fixture = new Fixture();
        _redisFixture = redisFixture;
    }

    [Fact]
    public async Task CacheService_GetStringAsync_ShouldBe_Success()
    {
        string testString = "12345A";

        var key = Guid.NewGuid().ToString();

        await _redisFixture.CacheService.SetObjectAsync(key, testString);

        var stringFromCache = _redisFixture.CacheService.GetStringAsync(key);
        stringFromCache.Should().NotBeNull();
    }

    [Fact]
    public async Task CacheService_SetObjectAsync_ShouldBe_Success()
    {
        var speciality = _fixture.Create<Speciality>();

        var key = Guid.NewGuid().ToString();

        await _redisFixture.CacheService.SetObjectAsync(key, speciality);

        var stringFromCache = await _redisFixture.CacheService.GetStringAsync(key);
        var objectFromCache = await _redisFixture.CacheService.GetObjectAsync<Speciality>(key);

        stringFromCache.Should().NotBeNull();
        objectFromCache.Should().BeEquivalentTo(speciality);
    }

    [Fact]
    public async Task CacheService_GetObjectAsync_ShouldBe_Success()
    {
        var speciality = _fixture.Create<Speciality>();

        var key = Guid.NewGuid().ToString();

        await _redisFixture.CacheService.SetObjectAsync(key, speciality);

        var objectFromCache = await _redisFixture.CacheService.GetObjectAsync<Speciality>(key);

        objectFromCache.Should().BeEquivalentTo(speciality);
    }

    [Fact]
    public async Task CacheService_RemoveAsync_ShouldBe_Success()
    {
        var speciality = _fixture.Create<Speciality>();

        var key = Guid.NewGuid().ToString();

        await _redisFixture.CacheService.SetObjectAsync(key, speciality);

        await _redisFixture.CacheService.RemoveAsync(key);

        string stringFromCache = await _redisFixture.CacheService.GetStringAsync(key);

        stringFromCache.Should().BeNull();
    }

    [Fact]
    public async Task CacheService_GetOrCreate_ShouldBe_SuccessWithGet()
    {
        var key = Guid.NewGuid().ToString();

        await _redisFixture.CacheService.SetObjectAsync(key, "test");

        var specialityRes = await _redisFixture.CacheService.GetOrCreateAsync(key, async () => "");

        specialityRes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CacheService_GetOrCreate_ShouldBe_SuccessWithCreate()
    {
        var key = Guid.NewGuid().ToString();

        var specialityRes = await _redisFixture.CacheService.GetOrCreateAsync(
            key,
            async () => "test"
        );

        var stringFromCache = await _redisFixture.CacheService.GetObjectAsync<string>(key);
        stringFromCache.Should().BeEquivalentTo(specialityRes);
    }
}
