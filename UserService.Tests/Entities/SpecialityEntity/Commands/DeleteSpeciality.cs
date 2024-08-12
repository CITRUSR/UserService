using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class DeleteSpeciality(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task DeleteSpeciality_ShouldBe_Success()
    {
        var specialities = Fixture.CreateMany<Speciality>(3);

        await AddSpecialitiesToContext([.. specialities]);

        var command = new DeleteSpecialityCommand(specialities.Select(x => x.Id).ToList());

        var specialitiesRes = await Action(command);

        var t = Context.Specialities;

        Context.Specialities.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteSpeciality_ShouldBe_SpecialityNotFoundException()
    {
        var command = new DeleteSpecialityCommand([1123, 123]);

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    private async Task<List<Speciality>> Action(DeleteSpecialityCommand command)
    {
        var handler = new DeleteSpecialityCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}
