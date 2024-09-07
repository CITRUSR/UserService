using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialities
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;
    private readonly GetSpecialitiesQuery _query;

    public GetSpecialities()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            SortState = SpecialitySortState.NameAsc,
            SearchString = "",
            DeletedStatus = DeletedStatus.All,
        };
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
        await TestDeletedStatus(DeletedStatus.All, (studentA, studentB) => [studentA, studentB]);
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithDeletedStatus_OnlyDeleted()
    {
        await TestDeletedStatus(DeletedStatus.OnlyDeleted, (studentA, studentB) => [studentA]);
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithDeletedStatus_OnlyActive()
    {
        await TestDeletedStatus(DeletedStatus.OnlyActive, (studentA, studentB) => [studentB]);
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithSearchStringName()
    {
        await TestSearchString("AA", 1, speciality => speciality.Name == "AAA");
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithSearchStringAbbr()
    {
        await TestSearchString("CC", 1, speciality => speciality.Abbreviation == "CCC");
    }

    private async Task TestOrdering(
        SpecialitySortState sortState,
        Func<Speciality, Speciality, Speciality[]> predicate
    )
    {
        Speciality specialityA = _fixture
            .Build<Speciality>()
            .With(x => x.Name, "AAA")
            .With(x => x.Abbreviation, "CCC")
            .With(x => x.Cost, 1)
            .With(x => x.DurationMonths, 1)
            .With(x => x.IsDeleted, true)
            .Create();

        Speciality specialityB = _fixture
            .Build<Speciality>()
            .With(x => x.Name, "BBB")
            .With(x => x.Abbreviation, "DDD")
            .With(x => x.Cost, 2)
            .With(x => x.DurationMonths, 2)
            .With(x => x.IsDeleted, false)
            .Create();

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([specialityA, specialityB]);

        var query = _query with { SortState = sortState };

        var handler = new GetSpecialitiesQueryHandler(_mockDbContext.Object);

        var result = await handler.Handle(query, default);

        result
            .Items.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(
                predicate(specialityA, specialityB).Select(x => x.Id),
                options => options.WithStrictOrdering()
            );
    }

    private async Task TestDeletedStatus(
        DeletedStatus deletedStatus,
        Func<Speciality, Speciality, Speciality[]> expectedSpeciality
    )
    {
        var specialityA = _fixture.Build<Speciality>().With(x => x.IsDeleted, true).Create();
        var specialityB = _fixture.Build<Speciality>().With(x => x.IsDeleted, false).Create();

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([specialityA, specialityB]);

        var query = _query with { DeletedStatus = deletedStatus };

        var handler = new GetSpecialitiesQueryHandler(_mockDbContext.Object);

        var result = await handler.Handle(query, default);

        result
            .Items.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(expectedSpeciality(specialityA, specialityB).Select(x => x.Id));
    }

    private async Task TestSearchString(
        string searchString,
        int expectedCount,
        Func<Speciality, bool> predicate
    )
    {
        var specialityA = _fixture.Build<Speciality>().With(x => x.Name, "AAA").Create();

        var specialityB = _fixture.Build<Speciality>().With(x => x.Abbreviation, "CCC").Create();

        _mockDbContext.Setup(x => x.Specialities).ReturnsDbSet([specialityA, specialityB]);

        var query = _query with { SearchString = searchString };

        var handler = new GetSpecialitiesQueryHandler(_mockDbContext.Object);

        var result = await handler.Handle(query, default);

        result.Items.Count.Should().Be(expectedCount);

        result.Items.Should().Contain(speciality => predicate(speciality));
    }
}
