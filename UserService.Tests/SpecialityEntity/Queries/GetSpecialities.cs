using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.SpecialityEntity.Queries;

public class GetSpecialities : CommonTest
{
    private struct CreateSpecialityOptions(string name, string abbreviation, int durationMonths, decimal cost)
    {
        public string Name { get; set; } = name;
        public string Abbreviation { get; set; } = abbreviation;
        public int DurationMonths { get; set; } = durationMonths;
        public decimal Cost { get; set; } = cost;
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithPageSize()
    {
        await SeedDataForPageTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(10);
        specialities.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithPageNumber()
    {
        await SeedDataForPageTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 2,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(2);
        specialities.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithNameAscOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForNameTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityA);
        specialities.Items[1].Should().BeEquivalentTo(specialityB);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithNameDescOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForNameTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.NameDesc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityB);
        specialities.Items[1].Should().BeEquivalentTo(specialityA);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithAbbrAscOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForAbbrTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.AbbreviationAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityA);
        specialities.Items[1].Should().BeEquivalentTo(specialityB);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithAbbrDescOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForAbbrTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.AbbreviationDesc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityB);
        specialities.Items[1].Should().BeEquivalentTo(specialityA);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithCostAscOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForCostTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.CostAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityA);
        specialities.Items[1].Should().BeEquivalentTo(specialityB);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithCostDescOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForCostTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.CostDesc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityB);
        specialities.Items[1].Should().BeEquivalentTo(specialityA);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDurationMonthsAscOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForDurationMonthsTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.DurationMonthsAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityA);
        specialities.Items[1].Should().BeEquivalentTo(specialityB);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDurationMonthsDescOrdering()
    {
        var (specialityA, specialityB) = await SeedDataForDurationMonthsTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.DurationMonthsDesc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityB);
        specialities.Items[1].Should().BeEquivalentTo(specialityA);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithSearchStringWithName()
    {
        var specialityA = Fixture.Build<Speciality>()
            .With(x => x.Name, "AAA")
            .Create();

        var specialityB = Fixture.Build<Speciality>()
            .With(x => x.Name, "BBB")
            .Create();

        await Context.Specialities.AddRangeAsync([specialityA, specialityB]);
        await Context.SaveChangesAsync();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "A",
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityA);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithSearchStringWithAbbr()
    {
        var specialityA = Fixture.Build<Speciality>()
            .With(x => x.Abbreavation, "AAA")
            .Create();

        var specialityB = Fixture.Build<Speciality>()
            .With(x => x.Abbreavation, "BBB")
            .Create();

        await Context.Specialities.AddRangeAsync([specialityA, specialityB]);
        await Context.SaveChangesAsync();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "A",
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items[0].Should().BeEquivalentTo(specialityA);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDeletedStatus_All()
    {
        await SeedDataForDeletedStatusTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = SpecialityDeletedStatus.All,
        };

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(2);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDeletedStatus_OnlyDeleted()
    {
        var (speciality1, speciality2) = await SeedDataForDeletedStatusTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = SpecialityDeletedStatus.OnlyDeleted,
        };

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(1);
        specialities.Items[0].Should().BeEquivalentTo(speciality1);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithDeletedStatus_OnlyActive()
    {
        var (speciality1, speciality2) = await SeedDataForDeletedStatusTests();

        var query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SearchString = "",
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = SpecialityDeletedStatus.OnlyActive,
        };

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(1);
        specialities.Items[0].Should().BeEquivalentTo(speciality2);
    }

    private async Task<(Speciality, Speciality)> SeedDataForDeletedStatusTests()
    {
        ClearDataBase();

        var speciality1 = Fixture.Build<Speciality>()
            .With(x => x.IsDeleted, true)
            .Create();

        var speciality2 = Fixture.Build<Speciality>()
            .With(x => x.IsDeleted, false)
            .Create();

        await Context.Specialities.AddRangeAsync([speciality1, speciality2]);
        await Context.SaveChangesAsync();

        return (speciality1, speciality2);
    }

    private async Task<(Speciality, Speciality)> SeedDataForDurationMonthsTests()
    {
        CreateSpecialityOptions optionsForFirstSpeciality = Fixture.Build<CreateSpecialityOptions>()
            .With(x => x.DurationMonths, 1)
            .Create();
        CreateSpecialityOptions optionsForSecondSpeciality = Fixture.Build<CreateSpecialityOptions>()
            .With(x => x.DurationMonths, 2)
            .Create();

        return await SeedDataForOrderingTests(optionsForFirstSpeciality, optionsForSecondSpeciality);
    }

    private async Task<(Speciality, Speciality)> SeedDataForCostTests()
    {
        CreateSpecialityOptions optionsForFirstSpeciality = Fixture.Build<CreateSpecialityOptions>()
            .With(x => x.Cost, 1)
            .Create();
        CreateSpecialityOptions optionsForSecondSpeciality = Fixture.Build<CreateSpecialityOptions>()
            .With(x => x.Cost, 2)
            .Create();

        return await SeedDataForOrderingTests(optionsForFirstSpeciality, optionsForSecondSpeciality);
    }

    private async Task<(Speciality, Speciality)> SeedDataForAbbrTests()
    {
        CreateSpecialityOptions optionsForFirstSpeciality = Fixture.Build<CreateSpecialityOptions>()
            .With(x => x.Abbreviation, "ABC")
            .Create();
        CreateSpecialityOptions optionsForSecondSpeciality = Fixture.Build<CreateSpecialityOptions>()
            .With(x => x.Abbreviation, "BBC")
            .Create();

        return await SeedDataForOrderingTests(optionsForFirstSpeciality, optionsForSecondSpeciality);
    }

    private async Task<(Speciality, Speciality)> SeedDataForNameTests()
    {
        CreateSpecialityOptions optionsForFirstSpeciality = Fixture.Build<CreateSpecialityOptions>()
            .With(x => x.Name, "ABC")
            .Create();
        CreateSpecialityOptions optionsForSecondSpeciality = Fixture.Build<CreateSpecialityOptions>()
            .With(x => x.Name, "BBC")
            .Create();

        return await SeedDataForOrderingTests(optionsForFirstSpeciality, optionsForSecondSpeciality);
    }

    private async Task<(Speciality, Speciality)> SeedDataForOrderingTests(
        CreateSpecialityOptions specialityOptionsForFirstSpeciality,
        CreateSpecialityOptions specialityOptionsForSecondSpeciality)
    {
        ClearDataBase();

        var speciality1 = Fixture.Build<Speciality>()
            .With(x => x.Name, specialityOptionsForFirstSpeciality.Name)
            .With(x => x.Abbreavation, specialityOptionsForFirstSpeciality.Abbreviation)
            .With(x => x.DurationMonths, specialityOptionsForFirstSpeciality.DurationMonths)
            .With(x => x.Cost, specialityOptionsForFirstSpeciality.Cost)
            .Create();

        var speciality2 = Fixture.Build<Speciality>()
            .With(x => x.Name, specialityOptionsForSecondSpeciality.Name)
            .With(x => x.Abbreavation, specialityOptionsForSecondSpeciality.Abbreviation)
            .With(x => x.DurationMonths, specialityOptionsForSecondSpeciality.DurationMonths)
            .With(x => x.Cost, specialityOptionsForSecondSpeciality.Cost)
            .Create();

        await Context.Specialities.AddRangeAsync([speciality1, speciality2]);
        await Context.SaveChangesAsync();

        return (speciality1, speciality2);
    }

    private async Task SeedDataForPageTests()
    {
        ClearDataBase();

        var specialities = Fixture.CreateMany<Speciality>(12);

        await Context.Specialities.AddRangeAsync(specialities);
        await Context.SaveChangesAsync();
    }

    private async Task<PaginationList<Speciality>> Action(GetSpecialitiesQuery query)
    {
        var handler = new GetSpecialitiesQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}