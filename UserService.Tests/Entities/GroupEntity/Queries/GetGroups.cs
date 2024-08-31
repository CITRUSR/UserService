using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroups
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;
    private readonly GetGroupsQuery _query;

    public GetGroups()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _query = new GetGroupsQuery
        {
            Page = 1,
            PageSize = 2,
            DeletedStatus = DeletedStatus.All,
            GraduatedStatus = GroupGraduatedStatus.All,
            SearchString = "",
            SortState = GroupSortState.GroupAsc
        };
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithFiltrationByGroupAsc()
    {
        await TestWithFiltrationByGroup(
            GroupSortState.GroupAsc,
            (groupA, groupB) => [groupA, groupB]
        );
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithFiltrationByGroupDesc()
    {
        await TestWithFiltrationByGroup(
            GroupSortState.GroupDesc,
            (groupA, groupB) => [groupB, groupA]
        );
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_All()
    {
        await TestWithGraduatedStatus(
            GroupGraduatedStatus.All,
            (groupA, groupB) => [groupA, groupB]
        );
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_OnlyActive()
    {
        await TestWithGraduatedStatus(
            GroupGraduatedStatus.OnlyActive,
            (groupA, groupB) => [groupB]
        );
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_OnlyGraduated()
    {
        await TestWithGraduatedStatus(
            GroupGraduatedStatus.OnlyGraduated,
            (groupA, groupB) => [groupA]
        );
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithIsDeletedStatus_OnlyActive()
    {
        await TestWithDeletedStatus(DeletedStatus.OnlyActive, (groupA, groupB) => [groupB]);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithIsDeletedStatus_OnlyDeleted()
    {
        await TestWithDeletedStatus(DeletedStatus.OnlyDeleted, (groupA, groupB) => [groupA]);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithIsDeletedStatus_All()
    {
        await TestWithDeletedStatus(DeletedStatus.All, (groupA, groupB) => [groupA, groupB]);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithSearchStringByAbbr()
    {
        await TestWithSearchString("ag", (groupA, groupB) => [groupA, groupB]);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithSearchStringByCurrentCourse()
    {
        await TestWithSearchString("1", (groupA, groupB) => [groupA]);
    }

    [Fact]
    public async Task GetGroups_ShouldBe_SuccessWithSearchStringBySubGroup()
    {
        await TestWithSearchString("4", (groupA, groupB) => [groupB]);
    }

    private async Task TestWithFiltrationByGroup(
        GroupSortState groupSortState,
        Func<Group, Group, Group[]> expectedOrder
    )
    {
        var groupA = _fixture.Build<Group>().With(x => x.CurrentCourse, 1).Create();
        var groupB = _fixture.Build<Group>().With(x => x.CurrentCourse, 2).Create();

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([groupA, groupB]);

        var handler = new GetGroupsQueryHandler(_mockDbContext.Object);

        var query = _query with { SortState = groupSortState };

        var result = await handler.Handle(query, default);

        result
            .Items.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(
                expectedOrder(groupA, groupB).Select(x => x.Id),
                options => options.WithStrictOrdering()
            );
    }

    private async Task TestWithGraduatedStatus(
        GroupGraduatedStatus graduatedStatus,
        Func<Group, Group, Group[]> expectedGroup
    )
    {
        var groupA = _fixture.Build<Group>().With(x => x.GraduatedAt, DateTime.Now).Create();
        var groupB = _fixture.Build<Group>().Without(x => x.GraduatedAt).Create();

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([groupA, groupB]);

        var handler = new GetGroupsQueryHandler(_mockDbContext.Object);

        var query = _query with { GraduatedStatus = graduatedStatus };

        var result = await handler.Handle(query, default);

        result
            .Items.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(expectedGroup(groupA, groupB).Select(x => x.Id));
    }

    private async Task TestWithDeletedStatus(
        DeletedStatus deletedStatus,
        Func<Group, Group, Group[]> expectedGroup
    )
    {
        var groupA = _fixture.Build<Group>().With(x => x.IsDeleted, true).Create();
        var groupB = _fixture.Build<Group>().Without(x => x.IsDeleted).Create();

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([groupA, groupB]);

        var handler = new GetGroupsQueryHandler(_mockDbContext.Object);

        var query = _query with { DeletedStatus = deletedStatus };

        var result = await handler.Handle(query, default);

        result
            .Items.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(expectedGroup(groupA, groupB).Select(x => x.Id));
    }

    private async Task TestWithSearchString(
        string SearchString,
        Func<Group, Group, Group[]> expectedGroup
    )
    {
        var specaility = _fixture.Build<Speciality>().With(x => x.Abbreavation, "AG").Create();

        var groupA = _fixture
            .Build<Group>()
            .With(x => x.Speciality, specaility)
            .With(x => x.CurrentCourse, 1)
            .With(x => x.SubGroup, 7)
            .Create();
        var groupB = _fixture
            .Build<Group>()
            .With(x => x.SubGroup, 4)
            .With(x => x.CurrentCourse, 7)
            .With(x => x.Speciality, specaility)
            .Create();

        _mockDbContext.Setup(x => x.Groups).ReturnsDbSet([groupA, groupB]);

        var handler = new GetGroupsQueryHandler(_mockDbContext.Object);

        var query = _query with { SearchString = SearchString };

        var result = await handler.Handle(query, default);

        result
            .Items.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(expectedGroup(groupA, groupB).Select(x => x.Id));
    }
}
