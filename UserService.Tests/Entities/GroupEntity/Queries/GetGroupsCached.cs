using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupsCached : RedisTest
{
    [Fact]
    public async void GetGroupsCached_ShouldBe_SuccessWithoutCache_WithValidatedQuery()
    {
        var groups =
            await GetGroupsCachedWithValidatedQuery(1, GroupSortState.GroupAsc, GroupGraduatedStatus.OnlyActive);

        var groupsFromCache =
            await CacheService.GetObjectAsync<PaginationList<Group>>(CacheKeys.GetEntities<Group>(1, 10));

        Context.Groups.Should().BeEquivalentTo(groups.Items);
        CacheService.GetStringAsync(CacheKeys.GetEntities<Group>(1, 10)).Should().NotBeNull();

        ClearSpecialityGroups(groups);

        groupsFromCache.Should().BeEquivalentTo(groups);
    }

    [Fact]
    public async void GetGroupsCached_ShouldBe_SuccessWithoutCache_WithOutValidatedQuery()
    {
        var groups =
            await GetGroupsCachedWithValidatedQuery(1, GroupSortState.GroupDesc, GroupGraduatedStatus.OnlyActive);

        Context.Groups.Should().BeEquivalentTo(groups.Items);

        var cacheString = await CacheService.GetStringAsync(CacheKeys.GetEntities<Group>(1, 10));
        cacheString.Should().BeNull();
    }

    [Fact]
    public async void GetGroupsCached_ShouldBe_SuccessWithCache()
    {
        await SeedDataForTestsWithValidatedQueryForCaching();

        var query = CreateQuery();

        var baseHandler = new GetGroupsQueryHandler(Context);

        var groups = await baseHandler.Handle(query, CancellationToken.None);

        await CacheService.SetObjectAsync(CacheKeys.GetEntities<Group>(1, 10), groups);

        await Action(query);

        var cacheString = await CacheService.GetStringAsync(CacheKeys.GetEntities<Group>(1, 10));
        var groupsFromCache =
            await CacheService.GetObjectAsync<PaginationList<Group>>(CacheKeys.GetEntities<Group>(1, 10));

        ClearSpecialityGroups(groups);

        cacheString.Should().NotBeNull();
        groupsFromCache.Should().BeEquivalentTo(groups);
    }

    private void ClearSpecialityGroups(PaginationList<Group> groups)
    {
        foreach (var group in groups.Items)
        {
            group.Speciality.Groups.Clear();
        }
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
        // var groups = Fixture.CreateMany<Group>(10);

        await AddGroupsToContext(groups.ToList());
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
        var handler = new GetGroupsQueryHandlerCached(new GetGroupsQueryHandler(Context), CacheService);

        return await handler.Handle(query, CancellationToken.None);
    }
}