using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.Enums;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialitiesCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async void GetSpecialitiesCached_ShouldBe_Success_WithoutCache_WithValidatedQuery()
    {
        await SeedData();

        var query = CreateQuery();

        var specialitiesRes = await Action(query);

        var key = CacheKeys.GetEntities<Speciality>(query.Page, query.PageSize);

        var cacheString = await CacheService.GetStringAsync(key);
        var specialitiesFromCache = await CacheService.GetObjectAsync<PaginationList<Speciality>>(
            key
        );

        cacheString.Should().NotBeNullOrEmpty();
        specialitiesFromCache.Should().BeEquivalentTo(specialitiesRes);
    }

    [Fact]
    public async void GetSpecialitiesCached_ShouldBe_Success_WithCache_WithValidatedQuery()
    {
        await SeedData();

        var specialities = Context.Specialities;

        var query = CreateQuery();

        var paginationList = await PaginationList<Speciality>.CreateAsync(
            specialities,
            query.Page,
            query.PageSize
        );

        var key = CacheKeys.GetEntities<Speciality>(query.Page, query.PageSize);

        await CacheService.SetObjectAsync(key, paginationList);

        await Action(query);

        var cacheString = await CacheService.GetStringAsync(key);
        var specialitiesFromCache = await CacheService.GetObjectAsync<PaginationList<Speciality>>(
            key
        );

        cacheString.Should().NotBeNullOrEmpty();
        specialitiesFromCache.Should().BeEquivalentTo(paginationList);
    }

    [Fact]
    public async void GetSpecialitiesCached_ShouldBe_Success_WithoutValidatedQuery()
    {
        await SeedData();

        var query = CreateQuery(sortState: SpecialitySortState.NameDesc);

        var key = CacheKeys.GetEntities<Speciality>(query.Page, query.PageSize);

        await Action(query);

        var cacheString = await CacheService.GetStringAsync(key);
        var specialitiesFromCache = await CacheService.GetObjectAsync<PaginationList<Speciality>>(
            key
        );

        cacheString.Should().BeNullOrEmpty();
        specialitiesFromCache.Should().BeNull();
    }

    private List<Speciality> CreateSpecialities(int count)
    {
        List<Speciality> specialities = new List<Speciality>();

        for (int i = 0; i < count; i++)
        {
            specialities.Add(Fixture.Build<Speciality>().Without(x => x.Id).Create());
        }

        return specialities;
    }

    private async Task<List<Speciality>> SeedData()
    {
        var specialities = CreateSpecialities(10);

        await AddSpecialitiesToContext([.. specialities]);

        return specialities;
    }

    private GetSpecialitiesQuery CreateQuery(
        int page = 1,
        int pageSize = 10,
        string searchString = "",
        SpecialitySortState sortState = SpecialitySortState.NameAsc,
        DeletedStatus deletedStatus = DeletedStatus.OnlyActive
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
        var handler = new GetSpecialitiesQueryHandlerCached(
            CacheService,
            new GetSpecialitiesQueryHandler(Context)
        );

        return await handler.Handle(query, CancellationToken.None);
    }
}
