using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class EditSpecialityCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task EditSpecialityCached_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await DbHelper.AddSpecialitiesToContext(speciality);

        var newSpeciality = Fixture.Build<Speciality>().With(x => x.Id, speciality.Id).Create();

        var command = new EditSpecialityCommand(
            speciality.Id,
            newSpeciality.Name,
            newSpeciality.Abbreavation,
            newSpeciality.Cost,
            newSpeciality.DurationMonths,
            newSpeciality.IsDeleted
        );

        var key = CacheKeys.ById<Speciality, int>(speciality.Id);

        await CacheService.SetObjectAsync<Speciality>(key, speciality);

        var handler = new EditSpecialityCommandHandlerCached(
            new EditSpecialityCommandHandler(Context),
            CacheService
        );

        var specialityRes = await handler.Handle(command, CancellationToken.None);

        var cachedObject = await CacheService.GetObjectAsync<Speciality>(key);

        cachedObject.Should().BeNull();
    }
}
