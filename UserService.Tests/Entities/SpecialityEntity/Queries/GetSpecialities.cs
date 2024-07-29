using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialities(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithPageSize()
    {
        await SeedDataForPageTests();

        var query = CreateQuery();

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(10);
        specialities.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithPageNumber()
    {
        await SeedDataForPageTests();

        var query = CreateQuery(page: 2);

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(2);
        specialities.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithNameAscOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForNameTests();

        var query = CreateQuery();

        var specialities = await Action(query);

        specialities.Items.Should().BeEquivalentTo([specialityA, specialityB]);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithNameDescOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForNameTests();

        var query = CreateQuery(sortState: SpecialitySortState.NameDesc);

        var specialities = await Action(query);

        specialities.Items.Should().BeEquivalentTo([specialityB, specialityA]);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithAbbrAscOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForAbbrTests();

        var query = CreateQuery(sortState: SpecialitySortState.AbbreviationAsc);

        var specialities = await Action(query);

        specialities.Items.Should().BeEquivalentTo([specialityA, specialityB]);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithAbbrDescOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForAbbrTests();

        var query = CreateQuery(sortState: SpecialitySortState.AbbreviationDesc);

        var specialities = await Action(query);

        specialities.Items.Should().BeEquivalentTo([specialityB, specialityA]);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithCostAscOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForCostTests();

        var query = CreateQuery(sortState: SpecialitySortState.CostAsc);

        var specialities = await Action(query);

        specialities.Items.Should().BeEquivalentTo([specialityA, specialityB]);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithCostDescOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForCostTests();

        var query = CreateQuery(sortState: SpecialitySortState.CostDesc);

        var specialities = await Action(query);

        specialities.Items.Should().BeEquivalentTo([specialityB, specialityA]);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDurationMonthsAscOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForDurationMonthsTests();

        var query = CreateQuery(sortState: SpecialitySortState.DurationMonthsAsc);

        var specialities = await Action(query);

        specialities.Items.Should().BeEquivalentTo([specialityA, specialityB]);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDurationMonthsDescOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForDurationMonthsTests();

        var query = CreateQuery(sortState: SpecialitySortState.DurationMonthsDesc);

        var specialities = await Action(query);

        specialities.Items.Should().BeEquivalentTo([specialityB, specialityA]);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDeletedStatus_All()
    {
        await SeedDataForDeletedStatusTests();

        var query = CreateQuery(deletedStatus: SpecialityDeletedStatus.All);

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(2);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDeletedStatus_OnlyDeleted()
    {
        var (speciality1, speciality2) = await SeedDataForDeletedStatusTests();

        var query = CreateQuery(deletedStatus: SpecialityDeletedStatus.OnlyDeleted);

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(1);
        specialities.Items[0].Should().BeEquivalentTo(speciality1);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDeletedStatus_OnlyActive()
    {
        var (speciality1, speciality2) = await SeedDataForDeletedStatusTests();

        var query = CreateQuery(deletedStatus: SpecialityDeletedStatus.OnlyActive);

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(1);
        specialities.Items[0].Should().BeEquivalentTo(speciality2);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithSearchStringName()
    {
        await TestSearchString("AA", 1, speciality => speciality.Name == "AAA");
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithSearchStringAbbr()
    {
        await TestSearchString("CC", 1, speciality => speciality.Abbreavation == "CCC");
    }

    private async Task TestSearchString(string searchString, int expectedCount, Func<Speciality, bool> predicate)
    {
        var (speciality1, speciality2, query) = await SeedDataForSearchStringTests(searchString);
        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(expectedCount);
        specialities.Items.Should().ContainSingle(speciality => predicate(speciality));
    }

    private async Task<(Speciality, Speciality, GetSpecialitiesQuery)> SeedDataForSearchStringTests(string searchString)
    {
        Speciality specialityA = Fixture.Build<Speciality>()
            .With(x => x.Name, "AAA")
            .With(x => x.Abbreavation, "CCC")
            .Create();
        Speciality specialityB = Fixture.Build<Speciality>()
            .With(x => x.Name, "BBB")
            .With(x => x.Abbreavation, "DDD")
            .Create();

        await AddSpecialitiesToContext([specialityA, specialityB]);

        var query = CreateQuery(searchString: searchString);

        return (specialityA, specialityB, query);
    }

    private async Task<(Speciality, Speciality)> SeedDataForDeletedStatusTests()
    {
        var specialityA = Fixture.Build<Speciality>()
            .With(x => x.IsDeleted, true)
            .Create();

        var specialityB = Fixture.Build<Speciality>()
            .With(x => x.IsDeleted, false)
            .Create();

        await AddSpecialitiesToContext([specialityA, specialityB]);

        return (specialityA, specialityB);
    }

    private async Task<(Speciality, Speciality)> SeedDataForDurationMonthsTests()
    {
        Speciality specialityA = Fixture.Build<Speciality>()
            .With(x => x.DurationMonths, 1)
            .Create();
        Speciality specialityB = Fixture.Build<Speciality>()
            .With(x => x.DurationMonths, 2)
            .Create();

        await AddSpecialitiesToContext([specialityA, specialityB]);
        return (specialityA, specialityB);
    }

    private async Task<(Speciality, Speciality)> SeedDataForCostTests()
    {
        Speciality specialityA = Fixture.Build<Speciality>()
            .With(x => x.Cost, 1)
            .Create();
        Speciality specialityB = Fixture.Build<Speciality>()
            .With(x => x.Cost, 2)
            .Create();

        await AddSpecialitiesToContext([specialityA, specialityB]);
        return (specialityA, specialityB);
    }

    private async Task<(Speciality, Speciality)> SeedDataForAbbrTests()
    {
        Speciality specialityA = Fixture.Build<Speciality>()
            .With(x => x.Abbreavation, "ABC")
            .Create();
        Speciality specialityB = Fixture.Build<Speciality>()
            .With(x => x.Abbreavation, "BBC")
            .Create();

        await AddSpecialitiesToContext([specialityA, specialityB]);
        return (specialityA, specialityB);
    }

    private async Task<(Speciality, Speciality)> SeedDataForNameTests()
    {
        Speciality specialityA = Fixture.Build<Speciality>()
            .With(x => x.Name, "ABC")
            .Create();
        Speciality specialityB = Fixture.Build<Speciality>()
            .With(x => x.Name, "BBC")
            .Create();

        await AddSpecialitiesToContext([specialityA, specialityB]);
        return (specialityA, specialityB);
    }

    private async Task SeedDataForPageTests()
    {
        var specialities = Fixture.CreateMany<Speciality>(12);

        await AddSpecialitiesToContext(specialities.ToArray());
    }

    private GetSpecialitiesQuery CreateQuery(int page = 1, int pageSize = 10, string searchString = "",
        SpecialitySortState sortState = SpecialitySortState.NameAsc,
        SpecialityDeletedStatus deletedStatus = SpecialityDeletedStatus.All)
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

    private async Task AddSpecialitiesToContext(params Speciality[] specialities)
    {
        await Context.Specialities.AddRangeAsync(specialities);
        await Context.SaveChangesAsync(CancellationToken.None);
    }

    private async Task<PaginationList<Speciality>> Action(GetSpecialitiesQuery query)
    {
        var handler = new GetSpecialitiesQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}