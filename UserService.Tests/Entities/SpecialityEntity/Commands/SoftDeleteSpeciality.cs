using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class SoftDeleteSpeciality(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task SoftDeleteSpeciality_ShouldBe_Success()
    {
        var specialities = Fixture.CreateMany<Speciality>(3);

        await DbHelper.AddSpecialitiesToContext([.. specialities]);

        var command = new SoftDeleteSpecialitiesCommand(specialities.Select(x => x.Id).ToList());

        var specialitiesRes = await Action(command);

        foreach (var speciality in Context.Specialities)
        {
            {
                speciality.IsDeleted.Should().BeTrue();
            }
        }
    }

    [Fact]
    public async Task SoftDeleteSpeciality_ShouldBe_SpecialityNotFoundException()
    {
        var command = new SoftDeleteSpecialitiesCommand([123, 1232]);

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    private async Task<List<Speciality>> Action(SoftDeleteSpecialitiesCommand command)
    {
        var handler = new SoftDeleteSpecialitiesCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}
