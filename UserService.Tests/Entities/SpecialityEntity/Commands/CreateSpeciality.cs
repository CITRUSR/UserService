using FluentAssertions;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class CreateSpeciality : CommonTest
{
    [Fact]
    public async void CreateSpeciality_ShouldBe_Success()
    {
        ClearDataBase();

        var command = Fixture.Create<CreateSpecialityCommand>();

        var handler = new CreateSpecialityCommandHandler(Context);

        var id = await handler.Handle(command, CancellationToken.None);

        Context.Specialities.FindAsync(id).Should().NotBeNull();
    }
}