using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Services;

public class CacheService : RedisTest
{
    [Fact]
    public async void CacheService_GetStringAsync_ShouldBe_Success()
    {
        string testString = "12345A";

        await CacheService.SetObjectAsync("test", testString);

        var stringFromCache = CacheService.GetStringAsync("test");
        stringFromCache.Should().NotBeNull();
    }

    [Fact]
    public async void CacheService_SetObjectAsync_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await CacheService.SetObjectAsync("test", speciality);

        var stringFromCache = await CacheService.GetStringAsync("test");
        var objectFromCache = await CacheService.GetObjectAsync<Speciality>("test");

        stringFromCache.Should().NotBeNull();
        objectFromCache.Should().BeEquivalentTo(speciality);
    }

    [Fact]
    public async void CacheService_GetObjectAsync_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await CacheService.SetObjectAsync("test", speciality);

        var objectFromCache = await CacheService.GetObjectAsync<Speciality>("test");

        objectFromCache.Should().BeEquivalentTo(speciality);
    }

    [Fact]
    public async void CacheService_RemoveAsync_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await CacheService.SetObjectAsync("test", speciality);

        await CacheService.RemoveAsync("test");

        string stringFromCache = await CacheService.GetStringAsync("test");

        stringFromCache.Should().BeNull();
    }

    [Fact]
    public async void CacheService_GetOrCreate_ShouldBe_SuccessWithGet()
    {
        await CacheService.SetObjectAsync("test", "test");

        var specialityRes = await CacheService.GetOrCreateAsync("test", async () => "");

        specialityRes.Should().NotBeEmpty();
    }

    [Fact]
    public async void CacheService_GetOrCreate_ShouldBe_SuccessWithCreate()
    {
        var specialityRes = await CacheService.GetOrCreateAsync("test", async () => "test");

        var stringFromCache = await CacheService.GetObjectAsync<string>("test");
        stringFromCache.Should().BeEquivalentTo(specialityRes);
    }

    [Fact]
    public async void CacheService_RemovePagesWithObjectAsync_ShouldBe_Success()
    {
        await RemoveObjectFromPage(1);
        await RemoveObjectFromPage(2);
        await RemoveObjectFromPage(3);
    }

    private async Task RemoveObjectFromPage(int pageNumber)
    {
        for (int i = pageNumber; i <= CacheConstants.PagesForCaching; i++)
        {
            var page = await GetPage(pageNumber);
            await CacheService.SetObjectAsync(CacheKeys.GetEntities<Group>(i, 10), page);
        }

        var key = CacheKeys.GetEntities<Group>(pageNumber, 10);
        var paginationList = await CacheService.GetObjectAsync<PaginationList<Group>>(key);

        await CacheService.RemovePagesWithObjectAsync<Group, int>(paginationList.Items[0].Id,
            (group, i) => group.Id == i);

        for (int i = pageNumber; i < CacheConstants.PagesForCaching; i++)
        {
            var CacheString = await CacheService.GetStringAsync(CacheKeys.GetEntities<Group>(i, 10));
            CacheString.Should().BeNull();
        }
    }

    private async Task<PaginationList<Group>> GetPage(int pageNumber)
    {
        var groups = Fixture.CreateMany<Group>(CacheConstants.PagesForCaching * 10);

        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync();

        IQueryable<Group> dbGroups = Context.Groups;

        return await PaginationList<Group>.CreateAsync(dbGroups, pageNumber, 10);
    }
}