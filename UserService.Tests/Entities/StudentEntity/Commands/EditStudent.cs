using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.EditStudent;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class EditStudent(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task EditStudent_ShouldBe_Success()
    {
        int oldGroupId = 12;

        var oldStudent = Fixture.Build<Student>().With(x => x.GroupId, oldGroupId).Create();

        var oldGroup = Fixture.Build<Group>().With(x => x.Id, oldGroupId).Create();
        oldGroup.Students.Add(oldStudent);

        var newGroup = Fixture.Create<Group>();

        var newStudent = Fixture
            .Build<Student>()
            .With(x => x.GroupId, newGroup.Id)
            .With(x => x.Id, oldStudent.Id)
            .With(x => x.SsoId, oldStudent.SsoId)
            .With(x => x.Group, newGroup)
            .With(x => x.DroppedOutAt, oldStudent.DroppedOutAt)
            .Create();

        await AddStudentsToContext(oldStudent);
        await AddGroupsToContext(oldGroup, newGroup);

        var command = new EditStudentCommand(
            oldStudent.Id,
            newStudent.FirstName,
            newStudent.LastName,
            newStudent.PatronymicName,
            newStudent.GroupId,
            newStudent.IsDeleted
        );

        await Action(command);

        Context
            .Students.FirstOrDefault(x => x.Id == oldStudent.Id)
            .Should()
            .BeEquivalentTo(newStudent);
        Context
            .Groups.FirstOrDefault(x => x.Id == oldGroupId)
            .Students.Should()
            .NotContain(oldStudent);
        Context
            .Groups.FirstOrDefault(x => x.Id == newGroup.Id)
            .Students.Should()
            .Contain(oldStudent);
    }

    [Fact]
    public async Task EditStudent_ShouldBe_StudentNotFoundException()
    {
        var oldStudent = Fixture.Create<Student>();

        await AddStudentsToContext(oldStudent);

        var command = Fixture.Create<EditStudentCommand>();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }

    [Fact]
    public async Task EditStudent_ShouldBe_GroupNotFoundException()
    {
        var oldStudent = Fixture.Create<Student>();

        await AddStudentsToContext(oldStudent);

        var command = Fixture.Build<EditStudentCommand>().With(x => x.Id, oldStudent.Id).Create();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    public async Task<Student> Action(EditStudentCommand command)
    {
        var handler = new EditStudentCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}
