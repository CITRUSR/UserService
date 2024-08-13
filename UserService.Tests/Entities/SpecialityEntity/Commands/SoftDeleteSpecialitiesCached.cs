using FluentAssertions;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class SoftDeleteSpecialitiesCached(DatabaseFixture databaseFixture)
    : RedisTest(databaseFixture)
{
    [Fact]
    public async Task SoftDeleteSpecialitiesCached_ShouldBe_Success()
    {
        var specialities = Fixture.CreateMany<Speciality>(5);

        await DbHelper.AddSpecialitiesToContext([.. specialities]);

        var command = new SoftDeleteSpecialitiesCommand(specialities.Select(x => x.Id).ToList());

        var handler = new SoftDeleteSpecialitiesCommandHandlerCached(
            new SoftDeleteSpecialitiesCommandHandler(Context),
            CacheService
        );

        var specialitiesRes = await handler.Handle(command, CancellationToken.None);

        foreach (var speciality in specialitiesRes)
        {
            var specialityFromCache = await CacheService.GetObjectAsync<Speciality>(
                CacheKeys.ById<Speciality, int>(speciality.Id)
            );

            specialityFromCache.Should().BeEquivalentTo(speciality);
        }
    }
}
