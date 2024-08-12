using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.EditSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class EditSpeciality(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task EditSpeciality_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        var newSpeciality = Fixture.Build<Speciality>().With(x => x.Id, speciality.Id).Create();

        await AddSpecialitiesToContext(speciality);

        var command = new EditSpecialityCommand(
            speciality.Id,
            newSpeciality.Name,
            newSpeciality.Abbreavation,
            newSpeciality.Cost,
            newSpeciality.DurationMonths,
            newSpeciality.IsDeleted
        );

        var specialityRes = await Action(command);

        Context.Specialities.Find(specialityRes.Id).Should().BeEquivalentTo(newSpeciality);
    }

    [Fact]
    public async Task EditSpeciality_ShouldBe_SpecialityNotFoundException()
    {
        var command = Fixture.Create<EditSpecialityCommand>();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    private async Task<Speciality> Action(EditSpecialityCommand command)
    {
        var handler = new EditSpecialityCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}
