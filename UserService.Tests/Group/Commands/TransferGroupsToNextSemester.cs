using FluentAssertions;
using UserService.Application.CQRS.Group.Commands.TransferGroupsToNextSemester;
using UserService.Tests.Common;

namespace UserService.Tests.Group.Commands;

public class TransferGroupsToNextSemester : CommonTest
{
    [Fact]
    public async void TransferGroupsToNextSemester_ShouldBe_SuccessWithList()
    {
        ClearDataBase();

        var groups = Fixture.CreateMany<Domain.Entities.Group>(5).ToList();

        var semesters = groups.ToDictionary(group => group, group => group.CurrentSemester);

        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync();

        var command = new TransferGroupsToNextSemesterCommand
        {
            IdGroups = groups.Select(x => x.Id).ToList(),
        };

        var handler = new TransferGroupsToNextSemesterCommandHandler(Context);

        var ids = await handler.Handle(command, CancellationToken.None);

        foreach (var group in groups.Where(x => ids.Contains(x.Id)))
        {
            semesters[group].Should().Be(--group.CurrentSemester);
        }
    }

    [Fact]
    public async void TransferGroupsToNextSemester_ShouldBe_SuccessWithoutList()
    {
        ClearDataBase();

        var groups = Fixture.CreateMany<Domain.Entities.Group>(5);

        var semesters = groups.ToDictionary(x => x, x => x.CurrentSemester);

        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync();

        var command = new TransferGroupsToNextSemesterCommand();

        var handler = new TransferGroupsToNextSemesterCommandHandler(Context);

        var ids = await handler.Handle(command, CancellationToken.None);

        foreach (var group in Context.Groups.Where(x => ids.Contains(x.Id)))
        {
            semesters[group].Should().Be(--group.CurrentSemester);
        }
    }
}