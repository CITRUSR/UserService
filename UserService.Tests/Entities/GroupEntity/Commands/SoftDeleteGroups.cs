using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.SoftDeleteGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class SoftDeleteGroups(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task SoftDeleteGroups_ShouldBe_Success()
    {
        var groups = Fixture.CreateMany<Group>(7);

        await DbHelper.AddGroupsToContext([.. groups]);

        var command = new SoftDeleteGroupsCommand([.. groups.Select(x => x.Id)]);

        var groupsRes = await Action(command);

        foreach (var group in groupsRes)
        {
            group.IsDeleted.Should().BeTrue();
        }
    }

    [Fact]
    public async Task SoftDeleteGroups_ShouldBe_GroupNotFounxException()
    {
        var command = new SoftDeleteGroupsCommand([123, 125, 1252]);

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    private async Task<List<Group>> Action(SoftDeleteGroupsCommand command)
    {
        var handler = new SoftDeleteGroupsCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}
