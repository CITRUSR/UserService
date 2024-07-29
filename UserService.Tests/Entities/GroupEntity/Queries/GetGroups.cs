using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Queries
{
    public class GetGroupsTests(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
    {
        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithPageSize()
        {
            await SeedDataForPageTests();
            var query = CreateQuery(pageSize: 10, page: 1);
            var groupsRes = await ExecuteQuery(query);

            groupsRes.Items.Should().HaveCount(10);
            groupsRes.MaxPage.Should().Be(2);
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithPageNumber()
        {
            await SeedDataForPageTests();
            var query = CreateQuery(pageSize: 10, page: 2);
            var groupsRes = await ExecuteQuery(query);

            groupsRes.Items.Should().HaveCount(2);
            groupsRes.MaxPage.Should().Be(2);
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithFiltrationByGroupAsc()
        {
            var (group1, group2, query) = await SeedDataForFiltrationTests(GroupSortState.GroupAsc);
            var groups = await ExecuteQuery(query);

            groups.Items.Should().BeEquivalentTo(new[] { group1, group2 });
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithFiltrationByGroupDesc()
        {
            var (group1, group2, query) = await SeedDataForFiltrationTests(GroupSortState.GroupDesc);
            var groups = await ExecuteQuery(query);

            groups.Items.Should().BeEquivalentTo(new[] { group2, group1 });
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_All()
        {
            var (group1, group2, query) = await SeedDataForGraduatedStatusTests(GroupGraduatedStatus.All);
            var groups = await ExecuteQuery(query);

            groups.Items.Should().HaveCount(2);
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_OnlyActive()
        {
            var (group1, group2, query) = await SeedDataForGraduatedStatusTests(GroupGraduatedStatus.OnlyActive);
            var groups = await ExecuteQuery(query);

            groups.Items.Should().HaveCount(1);
            groups.Items[0].Should().BeEquivalentTo(group2);
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithGroupGraduatedStatus_OnlyGraduated()
        {
            var (group1, group2, query) = await SeedDataForGraduatedStatusTests(GroupGraduatedStatus.OnlyGraduated);
            var groups = await ExecuteQuery(query);

            groups.Items.Should().HaveCount(1);
            groups.Items[0].Should().BeEquivalentTo(group1);
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithSearchStringByAbbr()
        {
            await TestSearchString("AA", 1, group => group.Speciality.Abbreavation == "AAA");
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithSearchStringByCurrentCourse()
        {
            await TestSearchString("1-A", 1, group => group.CurrentCourse == 1);
        }

        [Fact]
        public async void GetGroups_ShouldBe_SuccessWithSearchStringBySubGroup()
        {
            await TestSearchString("3", 1, group => group.SubGroup == 3);
        }

        private async Task TestSearchString(string searchString, int expectedCount, Func<Group, bool> predicate)
        {
            var (group1, group2, query) = await SeedDataForSearchStringTests(searchString);
            var groups = await ExecuteQuery(query);

            groups.Items.Should().HaveCount(expectedCount);
            groups.Items.Should().ContainSingle(group => predicate(group));
        }

        private async Task<(Group, Group, GetGroupsQuery)> SeedDataForSearchStringTests(string searchString)
        {
            var (group1, group2) = CreateGroupsWithSpeciality();
            await AddGroupsToContext(group1, group2);

            var query = CreateQuery(searchString: searchString);
            return (group1, group2, query);
        }

        private async Task<(Group, Group, GetGroupsQuery)> SeedDataForGraduatedStatusTests(
            GroupGraduatedStatus graduatedStatus)
        {
            var (group1, group2) = CreateGroupsWithGraduationStatus();
            await AddGroupsToContext(group1, group2);

            var query = CreateQuery(graduatedStatus: graduatedStatus);
            return (group1, group2, query);
        }

        private async Task SeedDataForPageTests()
        {
            var groups = Fixture.CreateMany<Group>(12);
            await AddGroupsToContext(groups.ToArray());
        }

        private async Task<(Group, Group, GetGroupsQuery)> SeedDataForFiltrationTests(GroupSortState sortState)
        {
            var (group1, group2) = CreateGroupsWithSpeciality();
            await AddGroupsToContext(group1, group2);

            var query = CreateQuery(sortState: sortState);
            return (group1, group2, query);
        }

        private async Task<PaginationList<Group>> ExecuteQuery(GetGroupsQuery query)
        {
            var handler = new GetGroupsQueryHandler(Context);
            return await handler.Handle(query, CancellationToken.None);
        }

        private GetGroupsQuery CreateQuery(int pageSize = 10, int page = 1, string searchString = "",
            GroupSortState sortState = GroupSortState.GroupDesc,
            GroupGraduatedStatus graduatedStatus = GroupGraduatedStatus.All)
        {
            return new GetGroupsQuery
            {
                PageSize = pageSize,
                Page = page,
                SearchString = searchString,
                SortState = sortState,
                GraduatedStatus = graduatedStatus,
            };
        }

        private (Group, Group) CreateGroupsWithSpeciality()
        {
            var speciality1 = Fixture.Build<Speciality>().With(x => x.Abbreavation, "AAA").Create();
            var speciality2 = Fixture.Build<Speciality>().With(x => x.Abbreavation, "BBB").Create();

            var group1 = Fixture.Build<Group>().With(x => x.Speciality, speciality1).With(x => x.CurrentCourse, 1)
                .With(x => x.SubGroup, 3).Create();
            var group2 = Fixture.Build<Group>().With(x => x.Speciality, speciality2).With(x => x.CurrentCourse, 2)
                .With(x => x.SubGroup, 4).Create();

            return (group1, group2);
        }

        private (Group, Group) CreateGroupsWithGraduationStatus()
        {
            var group1 = Fixture.Build<Group>().With(x => x.GraduatedAt, DateTime.Now).Create();
            var group2 = Fixture.Build<Group>().Without(x => x.GraduatedAt).Create();

            return (group1, group2);
        }
    }
}