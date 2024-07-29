using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class DeleteSpeciality(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void DeleteSpeciality_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await Context.Specialities.AddAsync(speciality);
        await Context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteSpecialityCommand(speciality.Id);

        var id = await Action(command);

        Context.Specialities.FirstOrDefault(x => x.Id == id).Should().BeNull();
    }

    [Fact]
    public async void DeleteSpeciality_ShouldBe_SpecialityNotFoundException()
    {
        var command = new DeleteSpecialityCommand(123);

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    private async Task<int> Action(DeleteSpecialityCommand command)
    {
        var handler = new DeleteSpecialityCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}