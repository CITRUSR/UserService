using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Group.Commands.TransferGroupToNextSemester;
using UserService.Tests.Common;

namespace UserService.Tests.Group.Commands;

public class TransferGroupToNextSemester : CommonTest
{
    [Fact]
    public async void TransferGroupToNextSemester_ShouldBe_Success()
    {
        ClearDataBase();

        var group = Fixture.Create<Domain.Entities.Group>();
        var oldSemester = group.CurrentSemester;

        await Context.Groups.AddAsync(group);
        await Context.SaveChangesAsync();

        var command = new TransferGroupToNextSemesterCommand(group.Id);
        var handler = new TransferGroupToNextSemesterCommandHandler(Context);

        var id = await handler.Handle(command, CancellationToken.None);

        group.CurrentSemester.Should().Be(++oldSemester);
    }

    [Fact]
    public async void TransferGroupToNextSemester_ShouldBe_GroupNotFoundException()
    {
        var command = new TransferGroupToNextSemesterCommand(213);
        var handler = new TransferGroupToNextSemesterCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}