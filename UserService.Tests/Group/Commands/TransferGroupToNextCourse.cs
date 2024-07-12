using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Group.Commands.TransferGroupToNextCourse;
using UserService.Tests.Common;

namespace UserService.Tests.Group.Commands;

public class TransferGroupToNextCourse : CommonTest
{
    [Fact]
    public async void TransferGroupToNextCourse_ShouldBe_Success()
    {
        ClearDataBase();

        var group = Fixture.Create<Domain.Entities.Group>();
        var oldCourse = group.CurrentCourse;

        await Context.Groups.AddAsync(group);
        await Context.SaveChangesAsync();

        var command = new TransferGroupToNextCourseCommand(group.Id);
        var handler = new TransferGroupToNextCourseCommandHandler(Context);

        var id = await handler.Handle(command, CancellationToken.None);

        Context.Groups.First().CurrentCourse.Should().Be(++oldCourse);
    }

    [Fact]
    public async void TransferGroupToNextCourse_ShouldBe_GroupNotFoundException()
    {
        var command = new TransferGroupToNextCourseCommand(12);
        var handler = new TransferGroupToNextCourseCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}