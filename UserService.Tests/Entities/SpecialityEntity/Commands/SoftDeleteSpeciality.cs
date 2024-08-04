using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class SoftDeleteSpeciality(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void SoftDeleteSpeciality_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await AddSpecialitiesToContext(speciality);

        var command = new SoftDeleteSpecialityCommand(speciality.Id);

        var id = await Action(command);

        Context.Specialities.Find(id).IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async void SoftDeleteSpeciality_ShouldBe_SpecialityNotFoundException()
    {
        var command = new SoftDeleteSpecialityCommand(123);

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    private async Task<int> Action(SoftDeleteSpecialityCommand command)
    {
        var handler = new SoftDeleteSpecialityCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}
