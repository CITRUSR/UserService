using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class DeleteGroups(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task DeleteGroups_ShouldBe_Success()
    {
        var groups = Fixture.CreateMany<Group>(3);

        await AddGroupsToContext([.. groups]);

        var command = new DeleteGroupsCommand(groups.Select(x => x.Id).ToList());
        var handler = new DeleteGroupsCommandHandler(Context);

        await handler.Handle(command, CancellationToken.None);
        Context.Groups.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteGroups_ShouldBe_GroupNotFoundException()
    {
        var command = new DeleteGroupsCommand([123, 12312]);
        var handler = new DeleteGroupsCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}
