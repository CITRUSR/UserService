using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Student.Commands.EditStudent;
using UserService.Tests.Common;

namespace UserService.Tests.Student.Commands;

public class EditStudent : CommonTest
{
    [Fact]
    public async void EditStudent_ShouldBe_Success()
    {
        int oldGroupId = 12;

        var oldStudent = Fixture.Build<Domain.Entities.Student>().With(x => x.GroupId, oldGroupId).Create();

        var oldGroup = Fixture.Build<Domain.Entities.Group>().With(x => x.Id, oldGroupId).Create();
        oldGroup.Students.Add(oldStudent);

        var newGroup = Fixture.Create<Domain.Entities.Group>();

        var newStudent = Fixture.Build<Domain.Entities.Student>().With(x => x.GroupId, newGroup.Id)
            .With(x => x.Id, oldStudent.Id).With(x => x.SsoId, oldStudent.SsoId).With(x => x.Group, newGroup)
            .With(x => x.DroppedOutAt, oldStudent.DroppedOutAt).Create();

        await Context.AddAsync(oldStudent);
        await Context.AddAsync(oldGroup);
        await Context.AddAsync(newGroup);

        var command = new EditStudentCommand(oldStudent.Id, newStudent.FirstName, newStudent.LastName,
            newStudent.PatronymicName, newStudent.GroupId);

        var handler = new EditStudentCommandHandler(Context);

        await handler.Handle(command, CancellationToken.None);

        Context.Students.FirstOrDefault(x => x.Id == oldStudent.Id).Should().BeEquivalentTo(newStudent);
        Context.Groups.FirstOrDefault(x => x.Id == oldGroupId).Students.Should().NotContain(oldStudent);
        Context.Groups.FirstOrDefault(x => x.Id == newGroup.Id).Students.Should().Contain(oldStudent);
    }

    [Fact]
    public async void EditStudent_ShouldBe_UserNotFoundException()
    {
        var oldStudent = Fixture.Create<Domain.Entities.Student>();

        await Context.AddAsync(oldStudent);

        var command = Fixture.Create<EditStudentCommand>();
        var handler = new EditStudentCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<StudentNotFoundException>();
    }

    [Fact]
    public async void EditStudent_ShouldBe_GroupNotFoundException()
    {
        var oldStudent = Fixture.Create<Domain.Entities.Student>();

        await Context.AddAsync(oldStudent);

        var command = Fixture.Build<EditStudentCommand>().With(x => x.Id, oldStudent.Id).Create();
        var handler = new EditStudentCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}