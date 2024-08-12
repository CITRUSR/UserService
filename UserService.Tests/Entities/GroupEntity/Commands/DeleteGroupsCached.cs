using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class DeleteGroupsCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task DeleteGroupsCached_ShouldBe_Success()
    {
        var groups = Fixture.CreateMany<Group>(3);

        await AddGroupsToContext([.. groups]);

        foreach (var group in groups)
        {
            await CacheService.SetObjectAsync(CacheKeys.ById<Group, int>(group.Id), group);
        }

        var command = new DeleteGroupsCommand(groups.Select(x => x.Id).ToList());

        var handler = new DeleteGroupsCommandHandlerCached(
            new DeleteGroupsCommandHandler(Context),
            CacheService
        );

        var groupRes = await handler.Handle(command, CancellationToken.None);

        foreach (var group in groups)
        {
            var groupFromCache = await CacheService.GetStringAsync(
                CacheKeys.ById<Group, int>(group.Id)
            );

            groupFromCache.Should().BeNull();
        }
    }
}
