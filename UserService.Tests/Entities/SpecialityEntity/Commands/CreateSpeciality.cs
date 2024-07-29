using FluentAssertions;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class CreateSpeciality(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void CreateSpeciality_ShouldBe_Success()
    {
        var command = Fixture.Build<CreateSpecialityCommand>()
            .With(x => x.Abbreavation, "ASDASAS")
            .Create();

        var handler = new CreateSpecialityCommandHandler(Context);

        var id = await handler.Handle(command, CancellationToken.None);

        Context.Specialities.FindAsync(id).Should().NotBeNull();
    }
}