using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Application.Enums;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroups(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithPageSize()
    {
        await TestPagination(pageSize: 10, page: 1, expectedCount: 10, expectedMaxPage: 2);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithPageNumber()
    {
        await TestPagination(pageSize: 10, page: 2, expectedCount: 2, expectedMaxPage: 2);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithFiltrationByGroupAsc()
    {
        await TestFiltration(GroupSortState.GroupAsc, (group1, group2) => [group1, group2]);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithFiltrationByGroupDesc()
    {
        await TestFiltration(GroupSortState.GroupDesc, (group1, group2) => [group1, group2]);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_All()
    {
        await TestGraduatedStatus(GroupGraduatedStatus.All, 2);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_OnlyActive()
    {
        await TestGraduatedStatus(GroupGraduatedStatus.OnlyActive, 1, (group1, group2) => group2);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_OnlyGraduated()
    {
        await TestGraduatedStatus(
            GroupGraduatedStatus.OnlyGraduated,
            1,
            (group1, group2) => group1
        );
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithIsDeletedStatus_OnlyActive()
    {
        await TestDeletedStatus(DeletedStatus.OnlyActive, 1, (group1, group2) => group2);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithIsDeletedStatus_OnlyDeleted()
    {
        await TestDeletedStatus(DeletedStatus.OnlyDeleted, 1, (group1, group2) => group1);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithIsDeletedStatus_All()
    {
        await TestDeletedStatus(DeletedStatus.All, 2);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithSearchStringByAbbr()
    {
        await TestSearchString("AA", 1, group => group.Speciality.Abbreavation == "AAA");
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithSearchStringByCurrentCourse()
    {
        await TestSearchString("1-A", 1, group => group.CurrentCourse == 1);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithSearchStringBySubGroup()
    {
        await TestSearchString("3", 1, group => group.SubGroup == 3);
    }

    private async Task TestPagination(
        int pageSize,
        int page,
        int expectedCount,
        int expectedMaxPage
    )
    {
        await SeedDataForPageTests();
        var query = CreateQuery(pageSize: pageSize, page: page);
        var groupsRes = await Action(query);

        groupsRes.Items.Should().HaveCount(expectedCount);
        groupsRes.MaxPage.Should().Be(expectedMaxPage);
    }

    private async Task TestFiltration(
        GroupSortState sortState,
        Func<Group, Group, Group[]> expectedOrder
    )
    {
        var (group1, group2, query) = await SeedDataForFiltrationTests(sortState);
        var groups = await Action(query);

        groups.Items.Should().BeEquivalentTo(expectedOrder(group1, group2));
    }

    private async Task TestGraduatedStatus(
        GroupGraduatedStatus graduatedStatus,
        int expectedCount,
        Func<Group, Group, Group> expectedGroup = null
    )
    {
        var (group1, group2, query) = await SeedDataForGraduatedStatusTests(graduatedStatus);
        var groups = await Action(query);

        groups.Items.Should().HaveCount(expectedCount);
        if (expectedGroup != null)
        {
            groups.Items[0].Should().BeEquivalentTo(expectedGroup(group1, group2));
        }
    }

    private async Task TestDeletedStatus(
        DeletedStatus deletedStatus,
        int expectedCount,
        Func<Group, Group, Group> expectedGroup = null
    )
    {
        var (group1, group2, query) = await SeedDataForDeletedStatusTests(deletedStatus);
        var groups = await Action(query);

        groups.Items.Should().HaveCount(expectedCount);
        if (expectedGroup != null)
        {
            groups.Items[0].Should().BeEquivalentTo(expectedGroup(group1, group2));
        }
    }

    private async Task TestSearchString(
        string searchString,
        int expectedCount,
        Func<Group, bool> predicate
    )
    {
        var (group1, group2, query) = await SeedDataForSearchStringTests(searchString);
        var groups = await Action(query);

        groups.Items.Should().HaveCount(expectedCount);
        groups.Items.Should().ContainSingle(group => predicate(group));
    }

    private async Task<(Group, Group, GetGroupsQuery)> SeedDataForSearchStringTests(
        string searchString
    )
    {
        var (group1, group2) = CreateGroupsWithSpeciality();
        await DbHelper.AddGroupsToContext(group1, group2);

        var query = CreateQuery(searchString: searchString);
        return (group1, group2, query);
    }

    private async Task<(Group, Group, GetGroupsQuery)> SeedDataForGraduatedStatusTests(
        GroupGraduatedStatus graduatedStatus
    )
    {
        var (group1, group2) = CreateGroupsWithGraduationStatus();
        await DbHelper.AddGroupsToContext(group1, group2);

        var query = CreateQuery(graduatedStatus: graduatedStatus);
        return (group1, group2, query);
    }

    private async Task SeedDataForPageTests()
    {
        var groups = Fixture.CreateMany<Group>(12);
        await DbHelper.AddGroupsToContext([.. groups]);
    }

    private async Task<(Group, Group, GetGroupsQuery)> SeedDataForFiltrationTests(
        GroupSortState sortState
    )
    {
        var (group1, group2) = CreateGroupsWithSpeciality();
        await DbHelper.AddGroupsToContext(group1, group2);

        var query = CreateQuery(sortState: sortState);
        return (group1, group2, query);
    }

    private async Task<(Group, Group, GetGroupsQuery)> SeedDataForDeletedStatusTests(
        DeletedStatus deletedStatus
    )
    {
        var (group1, group2) = CreateGroupsWithDeletedStatus();
        await DbHelper.AddGroupsToContext(group1, group2);

        var query = CreateQuery(deletedStatus: deletedStatus);
        return (group1, group2, query);
    }

    private async Task<PaginationList<Group>> Action(GetGroupsQuery query)
    {
        var handler = new GetGroupsQueryHandler(Context);
        return await handler.Handle(query, CancellationToken.None);
    }

    private GetGroupsQuery CreateQuery(
        int pageSize = 10,
        int page = 1,
        string searchString = "",
        GroupSortState sortState = GroupSortState.GroupDesc,
        GroupGraduatedStatus graduatedStatus = GroupGraduatedStatus.All,
        DeletedStatus deletedStatus = DeletedStatus.All
    )
    {
        return new GetGroupsQuery
        {
            PageSize = pageSize,
            Page = page,
            SearchString = searchString,
            SortState = sortState,
            GraduatedStatus = graduatedStatus,
            DeletedStatus = deletedStatus,
        };
    }

    private (Group, Group) CreateGroupsWithSpeciality()
    {
        var speciality1 = Fixture.Build<Speciality>().With(x => x.Abbreavation, "AAA").Create();
        var speciality2 = Fixture.Build<Speciality>().With(x => x.Abbreavation, "BBB").Create();

        var group1 = Fixture
            .Build<Group>()
            .With(x => x.Speciality, speciality1)
            .With(x => x.CurrentCourse, 1)
            .With(x => x.SubGroup, 3)
            .Create();
        var group2 = Fixture
            .Build<Group>()
            .With(x => x.Speciality, speciality2)
            .With(x => x.CurrentCourse, 2)
            .With(x => x.SubGroup, 4)
            .Create();

        return (group1, group2);
    }

    private (Group, Group) CreateGroupsWithGraduationStatus()
    {
        var group1 = Fixture.Build<Group>().With(x => x.GraduatedAt, DateTime.Now).Create();
        var group2 = Fixture.Build<Group>().Without(x => x.GraduatedAt).Create();

        return (group1, group2);
    }

    private (Group, Group) CreateGroupsWithDeletedStatus()
    {
        var group1 = Fixture.Build<Group>().With(x => x.IsDeleted, true).Create();
        var group2 = Fixture.Build<Group>().Without(x => x.IsDeleted).Create();

        return (group1, group2);
    }
}
