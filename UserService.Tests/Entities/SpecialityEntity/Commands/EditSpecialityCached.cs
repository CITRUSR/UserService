using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class EditSpecialityCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async void EditSpecialityCached_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await AddSpecialitiesToContext(speciality);

        var newSpeciality = Fixture.Build<Speciality>().With(x => x.Id, speciality.Id).Create();

        var command = new EditSpecialityCommand(
            speciality.Id,
            newSpeciality.Name,
            newSpeciality.Abbreavation,
            newSpeciality.Cost,
            newSpeciality.DurationMonths,
            newSpeciality.IsDeleted
        );

        var handler = new EditSpecialityCommandHandlerCached(
            new EditSpecialityCommandHandler(Context),
            CacheService
        );

        var specialityRes = await handler.Handle(command, CancellationToken.None);

        var key = CacheKeys.ById<Speciality, int>(specialityRes.Id);

        var cachedString = await CacheService.GetStringAsync(key);
        var cachedObject = await CacheService.GetObjectAsync<Speciality>(key);

        cachedString.Should().NotBeNull();
        cachedObject.Should().BeEquivalentTo(newSpeciality);
    }
}
