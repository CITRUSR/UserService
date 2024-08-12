using FluentAssertions;
using UserService.Application.CQRS.TeacherEntity.Commands.CreateTeacher;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.TeacherEntity.Commands;

public class CreateTeacher(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task CreateTeacher_ShouldBe_Success()
    {
        var command = Fixture
            .Build<CreateTeacherCommand>()
            .With(x => x.FirstName, "asadas")
            .With(x => x.LastName, "asadas")
            .With(x => x.PatronymicName, "asadas")
            .Create();

        var id = await Action(command);

        Context.Teachers.FindAsync(id).Should().NotBeNull();
    }

    private async Task<Guid> Action(CreateTeacherCommand command)
    {
        var handler = new CreateTeacherCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}
