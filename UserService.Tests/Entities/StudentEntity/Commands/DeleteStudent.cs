using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DeleteStudent(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void DeleteStudent_ShouldBe_Success()
    {
        var group = Fixture.Create<Group>();

        var student = Fixture.Build<Student>().With(x => x.GroupId, group.Id).Create();

        group.Students.Add(student);

        await AddStudentsToContext(student);
        await AddGroupsToContext(group);

        var command = new DeleteStudentCommand(student.Id);
        var handler = new DeleteStudentCommandHandler(Context);

        await handler.Handle(command, CancellationToken.None);

        Context.Students.FirstOrDefault(x => x.Id == command.Id).Should().BeNull();
        Context.Groups.FirstOrDefault(x => x.Id == group.Id).Students.Should().NotContain(student);
    }

    [Fact]
    public async void DeleteStudent_ShouldBe_NotFoundException()
    {
        var student = Fixture.Create<Student>();

        await AddStudentsToContext(student);

        var command = new DeleteStudentCommand(Guid.NewGuid());
        var handler = new DeleteStudentCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }
}
