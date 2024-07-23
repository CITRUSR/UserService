using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Domain.Entities;
using UserService.Persistance.Extensions;
using UserService.Tests.Common;

namespace UserService.Tests.GroupEntity.Queries;

public class GetGroupsCached : RedisTest
{
    [Fact]
    public async void GetGroupsCached_ShouldBe_SuccessWithoutCache_WithValidatedQuery()
    {
        var groups =
            await GetGroupsCachedWithValidatedQuery(1, GroupSortState.GroupAsc, GroupGraduatedStatus.OnlyActive);

        Context.Groups.Should().BeEquivalentTo(groups.Items);
        Redis.GetString(CacheKeys.GetGroups(1, 10)).Should().NotBeNull();
        JsonConvert.DeserializeObject<PaginationList<Group>>(Redis.GetString(CacheKeys.GetGroups(1, 10)))
            .Should().BeEquivalentTo(groups);
    }

    [Fact]
    public async void GetGroupsCached_ShouldBe_SuccessWithoutCache_WithOutValidatedQuery()
    {
        var groups =
            await GetGroupsCachedWithValidatedQuery(1, GroupSortState.GroupDesc, GroupGraduatedStatus.OnlyActive);

        Context.Groups.Should().BeEquivalentTo(groups.Items);
        Redis.GetString(CacheKeys.GetGroups(1, 10)).Should().BeNull();
    }

    [Fact]
    public async void GetGroupsCached_ShouldBe_SuccessWithCache()
    {
        await SeedDataForTestsWithValidatedQueryForCaching();

        var query = CreateQuery();

        var baseHandler = new GetGroupsQueryHandler(Context);

        var groups = await baseHandler.Handle(query, CancellationToken.None);

        await Redis.SetObjetAsync(CacheKeys.GetGroups(1, 10), groups);

        var res = await Action(query);

        Redis.GetString(CacheKeys.GetGroups(1, 10)).Should().NotBeNull();
        JsonConvert.DeserializeObject<PaginationList<Group>>(Redis.GetString(CacheKeys.GetGroups(1, 10)))
            .Should().BeEquivalentTo(res);
    }

    private async Task<PaginationList<Group>> GetGroupsCachedWithValidatedQuery(int page, GroupSortState sortState,
        GroupGraduatedStatus graduatedStatus)
    {
        await SeedDataForTestsWithValidatedQueryForCaching();
        var query = CreateQuery(page: page, sortState: sortState, graduatedStatus: graduatedStatus);

        return await Action(query);
    }

    private async Task SeedDataForTestsWithValidatedQueryForCaching()
    {
        var groups = CreateGroups(10);

        await AddGroupsToContext(groups);
    }

    private async Task AddGroupsToContext(List<Group> groups)
    {
        ClearDataBase();

        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync();
    }

    private List<Group> CreateGroups(int count)
    {
        List<Group> groups = new List<Group>();

        for (int i = 0; i < count; i++)
        {
            groups.Add(Fixture.Build<Group>()
                .Without(x => x.Curator)
                .Without(x => x.GraduatedAt)
                .Create());
        }

        return groups;
    }

    private GetGroupsQuery CreateQuery(int page = 1,
        int pageSize = 10,
        string searchString = "",
        GroupSortState sortState = GroupSortState.GroupAsc,
        GroupGraduatedStatus graduatedStatus = GroupGraduatedStatus.OnlyActive)
    {
        return new GetGroupsQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchString = searchString,
            GraduatedStatus = graduatedStatus,
            SortState = sortState,
        };
    }

    private async Task<PaginationList<Group>> Action(GetGroupsQuery query)
    {
        var handler = new GetGroupsQueryHandlerCached(Redis, new GetGroupsQueryHandler(Context));

        return await handler.Handle(query, CancellationToken.None);
    }
}