using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class DeleteSpecialityCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task DeleteSpecialityCached_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await DbHelper.AddSpecialitiesToContext(speciality);

        var command = new DeleteSpecialityCommand([speciality.Id]);

        var handler = new DeleteSpecialityCommandHandlerCached(
            CacheService,
            new DeleteSpecialityCommandHandler(Context)
        );

        var specialities = await handler.Handle(command, CancellationToken.None);

        foreach (var s in specialities)
        {
            var key = CacheKeys.ById<Speciality, int>(s.Id);

            var cachedString = await CacheService.GetStringAsync(key);
            var cachedSpeciality = await CacheService.GetObjectAsync<Speciality>(key);

            cachedString.Should().BeNull();
            cachedSpeciality.Should().BeNull();
        }
    }
}
