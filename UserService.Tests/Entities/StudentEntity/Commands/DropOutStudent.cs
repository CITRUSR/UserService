using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudent;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DropOutStudent(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void DropOutStudent_ShouldBe_Success()
    {
        var student = Fixture.Create<Student>();
        student.DroppedOutAt = null;

        await Context.Students.AddAsync(student);

        var command = Fixture.Build<DropOutStudentCommand>().With(x => x.Id, student.Id).Create();
        var handler = new DropOutStudentCommandHandler(Context);

        await handler.Handle(command, CancellationToken.None);

        Context.Students.FirstOrDefault(x => x.Id == student.Id).DroppedOutAt.Should().Be(command.DroppedOutTime);
    }

    [Fact]
    public async void DropOutStudent_ShouldBe_NotFoundException()
    {
        var student = Fixture.Create<Student>();

        await Context.Students.AddAsync(student);

        var command = Fixture.Create<DropOutStudentCommand>();
        var handler = new DropOutStudentCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}