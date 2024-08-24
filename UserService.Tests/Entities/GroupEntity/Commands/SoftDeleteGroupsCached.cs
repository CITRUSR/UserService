using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class SoftDeleteGroupsCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task SoftDeleteGroupsCached_ShouldBe_Success()
    {
        var groups = Fixture.CreateMany<Group>(5);

        await DbHelper.AddGroupsToContext([.. groups]);

        foreach (var group in groups)
        {
            await CacheService.SetObjectAsync<Group>(CacheKeys.ById<Group, int>(group.Id), group);
        }

        var command = new SoftDeleteGroupsCommand([.. groups.Select(x => x.Id)]);

        var handler = new SoftDeleteGroupsCommandHandlerCached(
            CacheService,
            new SoftDeleteGroupsCommandHandler(Context)
        );

        var groupsRes = await handler.Handle(command, CancellationToken.None);

        foreach (var group in groupsRes)
        {
            var specialityFromCache = await CacheService.GetObjectAsync<Group>(
                CacheKeys.ById<Group, int>(group.Id)
            );

            specialityFromCache.Should().BeNull();
        }
    }
}
