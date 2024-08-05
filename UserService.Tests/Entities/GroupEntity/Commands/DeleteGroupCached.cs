using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class DeleteGroupCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async void DeleteGroupCached_ShouldBe_Success()
    {
        var group = Fixture.Create<Group>();

        await AddGroupsToContext(group);

        await CacheService.SetObjectAsync(CacheKeys.ById<Group, int>(group.Id), group);

        var command = new DeleteGroupCommand(group.Id);

        var handler = new DeleteGroupCommandHandlerCached(
            new DeleteGroupCommandHandler(Context),
            CacheService
        );

        var id = await handler.Handle(command, CancellationToken.None);

        var groupFromCache = await CacheService.GetStringAsync(
            CacheKeys.ById<Group, int>(group.Id)
        );

        Context.Groups.Find(id).Should().BeNull();
        groupFromCache.Should().BeNull();
    }
}
