using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Application.Enums;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupsCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task GetGroupsCached_ShouldBe_SuccessWithoutCache_WithValidatedQuery()
    {
        var groups = await GetGroupsCachedWithValidatedQuery(
            1,
            GroupSortState.GroupAsc,
            GroupGraduatedStatus.OnlyActive,
            DeletedStatus.OnlyActive
        );

        var groupsFromCache = await CacheService.GetObjectAsync<PaginationList<Group>>(
            CacheKeys.GetEntities<Group>(1)
        );

        CacheService.GetStringAsync(CacheKeys.GetEntities<Group>(1)).Should().NotBeNull();

        groupsFromCache
            .Items.Should()
            .BeEquivalentTo(
                groups.Items,
                options => options.Excluding(x => x.Speciality).Excluding(x => x.Curator)
            );
        groupsFromCache.Should().BeEquivalentTo(groups, options => options.Excluding(x => x.Items));
    }

    [Fact]
    public async Task GetGroupsCached_ShouldBe_SuccessWithoutCache_WithOutValidatedQuery()
    {
        var groups = await GetGroupsCachedWithValidatedQuery(
            1,
            GroupSortState.GroupDesc,
            GroupGraduatedStatus.OnlyActive,
            DeletedStatus.All
        );

        var cacheString = await CacheService.GetStringAsync(CacheKeys.GetEntities<Group>(1));
        cacheString.Should().BeNull();
    }

    [Fact]
    public async Task GetGroupsCached_ShouldBe_SuccessWithCache()
    {
        await SeedDataForTestsWithValidatedQueryForCaching();

        var query = CreateQuery();

        var baseHandler = new GetGroupsQueryHandler(Context);

        var groups = await baseHandler.Handle(query, CancellationToken.None);

        await CacheService.SetObjectAsync(CacheKeys.GetEntities<Group>(1), groups);

        await Action(query);

        var cacheString = await CacheService.GetStringAsync(CacheKeys.GetEntities<Group>(1));
        var groupsFromCache = await CacheService.GetObjectAsync<PaginationList<Group>>(
            CacheKeys.GetEntities<Group>(1)
        );

        cacheString.Should().NotBeNull();
        groupsFromCache
            .Items.Should()
            .BeEquivalentTo(
                groups.Items,
                options => options.Excluding(x => x.Speciality).Excluding(x => x.Curator)
            );
        groupsFromCache.Should().BeEquivalentTo(groups, options => options.Excluding(x => x.Items));
    }

    private async Task<PaginationList<Group>> GetGroupsCachedWithValidatedQuery(
        int page,
        GroupSortState sortState,
        GroupGraduatedStatus graduatedStatus,
        DeletedStatus deletedStatus
    )
    {
        await SeedDataForTestsWithValidatedQueryForCaching();
        var query = CreateQuery(
            page: page,
            sortState: sortState,
            graduatedStatus: graduatedStatus,
            deletedStatus: deletedStatus
        );

        return await Action(query);
    }

    private async Task SeedDataForTestsWithValidatedQueryForCaching()
    {
        var speciality = Fixture.Create<Speciality>();

        await DbHelper.AddSpecialitiesToContext(speciality);

        var curator = Fixture.Create<Teacher>();

        await DbHelper.AddTeachersToContext(curator);

        var groups = CreateGroups(10, curator.Id, speciality);

        await DbHelper.AddGroupsToContext(groups.ToArray());
    }

    private List<Group> CreateGroups(int count, Guid curatorId, Speciality speciality)
    {
        List<Group> groups = new List<Group>();

        for (int i = 0; i < count; i++)
        {
            groups.Add(
                Fixture
                    .Build<Group>()
                    .Without(x => x.Curator)
                    .Without(x => x.GraduatedAt)
                    .Without(x => x.Id)
                    .With(x => x.CuratorId, curatorId)
                    .With(x => x.Speciality, speciality)
                    .Create()
            );
        }

        return groups;
    }

    private GetGroupsQuery CreateQuery(
        int page = 1,
        int pageSize = 10,
        string searchString = "",
        GroupSortState sortState = GroupSortState.GroupAsc,
        GroupGraduatedStatus graduatedStatus = GroupGraduatedStatus.OnlyActive,
        DeletedStatus deletedStatus = DeletedStatus.OnlyActive
    )
    {
        return new GetGroupsQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchString = searchString,
            GraduatedStatus = graduatedStatus,
            SortState = sortState,
            DeletedStatus = deletedStatus,
        };
    }

    private async Task<PaginationList<Group>> Action(GetGroupsQuery query)
    {
        var handler = new GetGroupsQueryHandlerCached(
            new GetGroupsQueryHandler(Context),
            CacheService
        );

        return await handler.Handle(query, CancellationToken.None);
    }
}
