using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class CreateGroupCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task CreateGroupCached_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();
        var curator = Fixture.Create<Teacher>();

        await DbHelper.AddSpecialitiesToContext(speciality);
        await DbHelper.AddTeachersToContext(curator);

        var command = Fixture
            .Build<CreateGroupCommand>()
            .With(x => x.CuratorId, curator.Id)
            .With(x => x.SpecialityId, speciality.Id)
            .Create();

        var handler = new CreateGroupCommandHandlerCached(
            CacheService,
            new CreateGroupCommandHandler(Context)
        );

        var group = await handler.Handle(command, CancellationToken.None);

        var key = CacheKeys.ById<Group, int>(group.Id);

        var cacheString = await CacheService.GetStringAsync(key);

        var groupFromCache = await CacheService.GetObjectAsync<Group>(key);

        cacheString.Should().NotBeNullOrEmpty();
        groupFromCache
            .Should()
            .BeEquivalentTo(
                group,
                options => options.Excluding(x => x.Curator).Excluding(x => x.Speciality)
            );
    }
}
