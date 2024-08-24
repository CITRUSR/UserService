using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class GraduateGroupsCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task GraduateGroupsCached_ShouldBe_Success()
    {
        var group = Fixture.Build<Group>().Without(x => x.GraduatedAt).Create();

        await DbHelper.AddGroupsToContext(group);

        DateTime graduatedTime = DateTime.Now;

        var key = CacheKeys.ById<Group, int>(group.Id);

        await CacheService.SetObjectAsync<Group>(key, group);

        var command = new GraduateGroupsCommand([group.Id], graduatedTime);

        var handler = new GraduateGroupsCommandHandlerCached(
            CacheService,
            new GraduateGroupsCommandHandler(Context)
        );

        var groups = await handler.Handle(command, CancellationToken.None);

        var groupFromCache = await CacheService.GetObjectAsync<Group>(key);

        groupFromCache.Should().BeNull();
    }
}
