using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.GroupEntity.Commands;

public class DeleteGroup : CommonTest
{
    [Fact]
    public async void DeleteGroup_ShouldBe_Success()
    {
        ClearDataBase();

        var group = Fixture.Create<Group>();
        await Context.Groups.AddAsync(group);
        await Context.SaveChangesAsync();

        var command = new DeleteGroupCommand(group.Id);
        var handler = new DeleteGroupCommandHandler(Context);

        var id = await handler.Handle(command, CancellationToken.None);
        Context.Groups.Should().BeEmpty();
    }

    [Fact]
    public async void DeleteGroup_ShouldBe_GroupNotFoundException()
    {
        var command = new DeleteGroupCommand(123);
        var handler = new DeleteGroupCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}