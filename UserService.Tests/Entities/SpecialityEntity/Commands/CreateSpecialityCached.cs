using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class CreateSpecialityCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async void CreateSpecialityCached_ShouldBe_Success()
    {
        var command = Fixture
            .Build<CreateSpecialityCommand>()
            .With(x => x.Abbreavation, "ASDASAS")
            .Create();

        var handler = new CreateSpecialityCommandHandlerCached(
            CacheService,
            new CreateSpecialityCommandHandler(Context)
        );

        var speciality = await handler.Handle(command, CancellationToken.None);

        var key = CacheKeys.ById<Speciality, int>(speciality.Id);

        var cachedString = await CacheService.GetStringAsync(key);
        var cachedObject = await CacheService.GetObjectAsync<Speciality>(key);

        cachedString.Should().NotBeNull();
        cachedObject.Should().BeEquivalentTo(speciality);
    }
}
