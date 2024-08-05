using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialityByIdCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async void GetSpecialityById_ShouldBe_Success_WithoutCache()
    {
        var speciality = Fixture.Create<Speciality>();

        await AddSpecialitiesToContext(speciality);

        var query = new GetSpecialityByIdQuery(speciality.Id);

        var handler = new GetSpecialityByIdQueryHandlerCached(
            CacheService,
            new GetSpecialityByIdQueryHandler(Context)
        );

        var specialityRes = await handler.Handle(query, CancellationToken.None);

        var key = CacheKeys.ById<Speciality, int>(speciality.Id);

        var cachedString = await CacheService.GetStringAsync(key);
        var spsecialityFromCache = await CacheService.GetObjectAsync<Speciality>(key);

        cachedString.Should().NotBeNullOrEmpty();
        specialityRes.Should().BeEquivalentTo(spsecialityFromCache);
    }
}
