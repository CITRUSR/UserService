using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Student.Commands.DropOutStudent;
using UserService.Tests.Common;

namespace UserService.Tests.Student.Commands;

public class DropOutStudent : CommonTest
{
    [Fact]
    public async void DropOutStudent_ShouldBe_Success()
    {
        var student = Fixture.Create<Domain.Entities.Student>();
        student.DroppedOutAt = null;

        await Context.AddAsync(student);

        var command = Fixture.Build<DropOutStudentCommand>().With(x => x.Id, student.Id).Create();
        var handler = new DropOutStudentCommandHandler(Context);

        await handler.Handle(command, CancellationToken.None);

        Context.Students.FirstOrDefault(x => x.Id == student.Id).DroppedOutAt.Should().Be(command.DroppedOutTime);
    }

    [Fact]
    public async void DropOutStudent_ShouldBe_NotFoundException()
    {
        var student = Fixture.Create<Domain.Entities.Student>();

        await Context.AddAsync(student);

        var command = Fixture.Create<DropOutStudentCommand>();
        var handler = new DropOutStudentCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}