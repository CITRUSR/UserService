using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.GroupEntity.Queries;

public class GetGroups : CommonTest
{
    [Fact]
    public async void GetGroups_ShouldBe_SuccessWithPageSize()
    {
        await SeedDataForPageTests();

        var query = new GetGroupsQuery
        {
            PageSize = 10,
            Page = 1,
            SearchString = "",
            SortState = GroupSortState.GroupAsc,
        };

        var groupsRes = await Action(query);

        groupsRes.Items.Should().HaveCount(10);
        groupsRes.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetGroups_ShouldBe_SuccessWithPageNumber()
    {
        await SeedDataForPageTests();

        var query = new GetGroupsQuery
        {
            PageSize = 10,
            Page = 2,
            SearchString = "",
            SortState = GroupSortState.GroupAsc,
        };

        var groupsRes = await Action(query);

        groupsRes.Items.Should().HaveCount(2);
        groupsRes.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetGroups_ShouldBeSuccessWithFiltrationByGroupAsc()
    {
        var (group1, group2) = await SeedDataForFiltrationTests();

        var query = new GetGroupsQuery
        {
            PageSize = 10,
            Page = 1,
            SearchString = "",
            SortState = GroupSortState.GroupAsc,
        };

        var groups = await Action(query);

        groups.Items[0].Should().BeEquivalentTo(group1);
        groups.Items[1].Should().BeEquivalentTo(group2);
    }

    [Fact]
    public async void GetGroups_ShouldBeSuccessWithFiltrationByGroupDesc()
    {
        var (group1, group2) = await SeedDataForFiltrationTests();

        var query = new GetGroupsQuery
        {
            PageSize = 10,
            Page = 1,
            SearchString = "",
            SortState = GroupSortState.GroupDesc,
        };

        var groups = await Action(query);

        groups.Items[1].Should().BeEquivalentTo(group1);
        groups.Items[0].Should().BeEquivalentTo(group2);
    }

    private async Task SeedDataForPageTests()
    {
        ClearDataBase();

        var groups = Fixture.CreateMany<Group>(12);

        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync();
    }

    private async Task<(Group, Group)> SeedDataForFiltrationTests()
    {
        ClearDataBase();

        var speciality1 = Fixture.Build<Speciality>()
            .With(x => x.Abbreavation, "ABC")
            .Create();

        var speciality2 = Fixture.Build<Speciality>()
            .With(x => x.Abbreavation, "BBC")
            .Create();

        var group1 = Fixture.Build<Group>()
            .With(x => x.CurrentCourse, 1)
            .With(x => x.Speciality, speciality1)
            .Create();

        var group2 = Fixture.Build<Group>()
            .With(x => x.CurrentCourse, 2)
            .With(x => x.Speciality, speciality2)
            .Create();

        await Context.Groups.AddAsync(group1);
        await Context.Groups.AddAsync(group2);
        await Context.SaveChangesAsync();

        return (group1, group2);
    }

    private async Task<PaginationList<Group>> Action(GetGroupsQuery query)
    {
        var handler = new GetGroupsQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}