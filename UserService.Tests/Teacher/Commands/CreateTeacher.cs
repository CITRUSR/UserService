using FluentAssertions;
using UserService.Application.CQRS.Teacher.Commands.CreateTeacher;
using UserService.Tests.Common;

namespace UserService.Tests.Teacher.Commands;

public class CreateTeacher : CommonTest
{
    [Fact]
    public async void CreateTeacher_ShouldBe_Success()
    {
        var command = Fixture.Create<CreateTeacherCommand>();

        var id = await Action(command);

        Context.Teachers.FindAsync(id).Should().NotBeNull();
    }

    private async Task<Guid> Action(CreateTeacherCommand command)
    {
        var handler = new CreateTeacherCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}