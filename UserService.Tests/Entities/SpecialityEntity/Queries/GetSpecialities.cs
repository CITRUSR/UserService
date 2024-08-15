using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.Enums;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialities(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithPageSize()
    {
        await TestPagination(pageSize: 10, page: 1, expectedCount: 10, expectedMaxPage: 2);
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithPageNumber()
    {
        await TestPagination(pageSize: 10, page: 2, expectedCount: 2, expectedMaxPage: 2);
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithNameAscOrdering()
    {
        await TestOrdering(
            SpecialitySortState.NameAsc,
            (speciality1, speciality2) => [speciality1, speciality2]
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithNameDescOrdering()
    {
        await TestOrdering(
            SpecialitySortState.NameDesc,
            (speciality1, speciality2) => [speciality2, speciality1]
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithAbbrAscOrdering()
    {
        await TestOrdering(
            SpecialitySortState.AbbreviationAsc,
            (speciality1, speciality2) => [speciality1, speciality2]
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithAbbrDescOrdering()
    {
        await TestOrdering(
            SpecialitySortState.AbbreviationDesc,
            (speciality1, speciality2) => [speciality2, speciality1]
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithCostAscOrdering()
    {
        await TestOrdering(
            SpecialitySortState.CostAsc,
            (speciality1, speciality2) => [speciality1, speciality2]
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithCostDescOrdering()
    {
        await TestOrdering(
            SpecialitySortState.CostDesc,
            (speciality1, speciality2) => [speciality2, speciality1]
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithDurationMonthsAscOrdering()
    {
        await TestOrdering(
            SpecialitySortState.DurationMonthsAsc,
            (speciality1, speciality2) => [speciality1, speciality2]
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithDurationMonthsDescOrdering()
    {
        await TestOrdering(
            SpecialitySortState.DurationMonthsDesc,
            (speciality1, speciality2) => [speciality2, speciality1]
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithDeletedStatus_All()
    {
        await TestDeletedStatus(DeletedStatus.All, 2);
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithDeletedStatus_OnlyDeleted()
    {
        await TestDeletedStatus(
            DeletedStatus.OnlyDeleted,
            1,
            (speciality1, speciality2) => speciality1
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithDeletedStatus_OnlyActive()
    {
        await TestDeletedStatus(
            DeletedStatus.OnlyActive,
            1,
            (speciality1, speciality2) => speciality2
        );
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithSearchStringName()
    {
        await TestSearchString("AA", 1, speciality => speciality.Name == "AAA");
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithSearchStringAbbr()
    {
        await TestSearchString("CC", 1, speciality => speciality.Abbreavation == "CCC");
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

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(expectedCount);
        specialities.MaxPage.Should().Be(expectedMaxPage);
    }

    private async Task TestOrdering(
        SpecialitySortState sortState,
        Func<Speciality, Speciality, Speciality[]> predicate
    )
    {
        var (specialityA, specialityB) = await SeedData();

        var query = CreateQuery(sortState: sortState);

        var specialities = await Action(query);

        specialities
            .Items.Should()
            .BeEquivalentTo(
                predicate(specialityA, specialityB),
                options => options.WithStrictOrdering()
            );
    }

    private async Task TestDeletedStatus(
        DeletedStatus deletedStatus,
        int expectedCount,
        Func<Speciality, Speciality, Speciality> expectedSpeciality = null
    )
    {
        var (specialityA, specialityB) = await SeedData();

        var query = CreateQuery(deletedStatus: deletedStatus);

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(expectedCount);

        if (expectedSpeciality != null)
        {
            specialities
                .Items[0]
                .Should()
                .BeEquivalentTo(expectedSpeciality(specialityA, specialityB));
        }
    }

    private async Task TestSearchString(
        string searchString,
        int expectedCount,
        Func<Speciality, bool> predicate
    )
    {
        var (speciality1, speciality2) = await SeedData();

        var query = CreateQuery(searchString: searchString);

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(expectedCount);
        specialities.Items.Should().ContainSingle(speciality => predicate(speciality));
    }

    private async Task<(Speciality, Speciality)> SeedData()
    {
        Speciality specialityA = Fixture
            .Build<Speciality>()
            .With(x => x.Name, "AAA")
            .With(x => x.Abbreavation, "CCC")
            .With(x => x.Cost, 1)
            .With(x => x.DurationMonths, 1)
            .With(x => x.IsDeleted, true)
            .Create();
        Speciality specialityB = Fixture
            .Build<Speciality>()
            .With(x => x.Name, "BBB")
            .With(x => x.Abbreavation, "DDD")
            .With(x => x.Cost, 2)
            .With(x => x.DurationMonths, 2)
            .With(x => x.IsDeleted, false)
            .Create();

        await DbHelper.AddSpecialitiesToContext([specialityA, specialityB]);
        return (specialityA, specialityB);
    }

    private async Task SeedDataForPageTests()
    {
        var specialities = Fixture.CreateMany<Speciality>(12);

        await DbHelper.AddSpecialitiesToContext([.. specialities]);
    }

    private GetSpecialitiesQuery CreateQuery(
        int page = 1,
        int pageSize = 10,
        string searchString = "",
        SpecialitySortState sortState = SpecialitySortState.NameAsc,
        DeletedStatus deletedStatus = DeletedStatus.All
    )
    {
        return new GetSpecialitiesQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchString = searchString,
            DeletedStatus = deletedStatus,
            SortState = sortState,
        };
    }

    private async Task<PaginationList<Speciality>> Action(GetSpecialitiesQuery query)
    {
        var handler = new GetSpecialitiesQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}
