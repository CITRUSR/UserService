using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.Student.Commands.CreateStudent;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Student.Commands;

public class CreateStudent : CommonTest
{
    [Fact]
    public async void CreateStudent_ShouldBe_Success()
    {
        var group = Fixture.Create<Group>();

        await Context.AddAsync(group);

        var command = Fixture.Build<CreateStudentCommand>().With(x => x.GroupId, group.Id).Create();

        CreateStudentCommandHandler handler = new CreateStudentCommandHandler(Context);

        await handler.Handle(command, CancellationToken.None);

        Context.Students.Should().HaveCount(1);
    }

    [Fact]
    public async void CreateStudent_ShouldBe_ThrowRPCExceptionNotFoundGroup()
    {
        var command = Fixture.Create<CreateStudentCommand>();

        var handler = new CreateStudentCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}