using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class CreateStudent(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void CreateStudent_ShouldBe_Success()
    {
        var group = Fixture.Create<Group>();

        await Context.Groups.AddAsync(group);

        var command = Fixture.Build<CreateStudentCommand>()
            .With(x => x.GroupId, group.Id)
            .With(x => x.FirstName, "ASDASDASFASGAS")
            .With(x => x.LastName, "ASDASDASGAS")
            .With(x => x.PatronymicName, "asaASa")
            .Create();

        CreateStudentCommandHandler handler = new CreateStudentCommandHandler(Context);

        await handler.Handle(command, CancellationToken.None);

        Context.Students.Should().HaveCount(1);
    }

    [Fact]
    public async void CreateStudent_ShouldBe_NotFoundException()
    {
        var command = Fixture.Create<CreateStudentCommand>();

        var handler = new CreateStudentCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}