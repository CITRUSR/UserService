using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Services;

public class CacheService(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task CacheService_GetStringAsync_ShouldBe_Success()
    {
        string testString = "12345A";

        await CacheService.SetObjectAsync("test", testString);

        var stringFromCache = CacheService.GetStringAsync("test");
        stringFromCache.Should().NotBeNull();
    }

    [Fact]
    public async Task CacheService_SetObjectAsync_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await CacheService.SetObjectAsync("test", speciality);

        var stringFromCache = await CacheService.GetStringAsync("test");
        var objectFromCache = await CacheService.GetObjectAsync<Speciality>("test");

        stringFromCache.Should().NotBeNull();
        objectFromCache.Should().BeEquivalentTo(speciality);
    }

    [Fact]
    public async Task CacheService_GetObjectAsync_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await CacheService.SetObjectAsync("test", speciality);

        var objectFromCache = await CacheService.GetObjectAsync<Speciality>("test");

        objectFromCache.Should().BeEquivalentTo(speciality);
    }

    [Fact]
    public async Task CacheService_RemoveAsync_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await CacheService.SetObjectAsync("test", speciality);

        await CacheService.RemoveAsync("test");

        string stringFromCache = await CacheService.GetStringAsync("test");

        stringFromCache.Should().BeNull();
    }

    [Fact]
    public async Task CacheService_GetOrCreate_ShouldBe_SuccessWithGet()
    {
        await CacheService.SetObjectAsync("test", "test");

        var specialityRes = await CacheService.GetOrCreateAsync("test", async () => "");

        specialityRes.Should().NotBeEmpty();
    }

    [Fact]
    public async Task CacheService_GetOrCreate_ShouldBe_SuccessWithCreate()
    {
        var specialityRes = await CacheService.GetOrCreateAsync("test", async () => "test");

        var stringFromCache = await CacheService.GetObjectAsync<string>("test");
        stringFromCache.Should().BeEquivalentTo(specialityRes);
    }
}
