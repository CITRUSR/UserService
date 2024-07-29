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

        var id = await Action(command);

        var groupFromCache = await CacheService.GetStringAsync(CacheKeys.ById<Group, int>(group.Id));

        Context.Groups.Find(id).Should().BeNull();
        groupFromCache.Should().BeNull();
    }

    private async Task<int> Action(DeleteGroupCommand command)
    {
        var handler = new DeleteGroupCommandHandlerCached(new DeleteGroupCommandHandler(Context), CacheService);

        return await handler.Handle(command, CancellationToken.None);
    }
}