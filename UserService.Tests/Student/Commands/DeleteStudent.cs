using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Student.Commands.DeleteStudent;
using UserService.Tests.Common;

namespace UserService.Tests.Student.Commands;

public class DeleteStudent : CommonTest
{
    [Fact]
    public async void DeleteStudent_ShouldBe_Success()
    {
        var student = Fixture.Create<Domain.Entities.Student>();

        await Context.AddAsync(student);

        var command = new DeleteStudentCommand(student.Id);
        var handler = new DeleteStudentCommandHandler(Context);

        await handler.Handle(command, CancellationToken.None);

        Context.Students.FirstOrDefault(x => x.Id == command.Id).Should().BeNull();
    }

    [Fact]
    public async void DeleteStudent_ShouldBe_NotFoundException()
    {
        var student = Fixture.Create<Domain.Entities.Student>();

        await Context.AddAsync(student);

        var command = new DeleteStudentCommand(12123125);
        var handler = new DeleteStudentCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}