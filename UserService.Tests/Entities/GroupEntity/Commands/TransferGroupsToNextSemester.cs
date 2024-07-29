using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class TransferGroupsToNextSemester(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void TransferGroupsToNextSemester_ShouldBe_SuccessWithList()
    {
        var semesters = await Arrange(6);

        var command = new TransferGroupsToNextSemesterCommand
        {
            IdGroups = Context.Groups.Select(x => x.Id).ToList(),
        };

        var ids = await Action(command);

        Assert(ids, semesters);
    }

    [Fact]
    public async void TransferGroupsToNextSemester_ShouldBe_SuccessWithoutList()
    {
        var semesters = await Arrange(6);

        var command = new TransferGroupsToNextSemesterCommand();

        var ids = await Action(command);

        Assert(ids, semesters);
    }

    [Fact]
    public async void TransferGroupsToNextSemester_ShouldBe_GroupSemesterOutOfRange()
    {
        var semesters = await Arrange(8);

        var command = new TransferGroupsToNextSemesterCommand();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<GroupSemesterOutOfRangeException>();
    }

    private async Task<Dictionary<Group, byte>> Arrange(int currentSemester)
    {
        var speciality = Fixture.Build<Speciality>()
            .With(x => x.DurationMonths, 46)
            .Create();

        var group1 = Fixture.Build<Group>()
            .With(x => x.CurrentSemester, currentSemester)
            .With(x => x.Speciality, speciality)
            .Create();

        var group2 = Fixture.Build<Group>()
            .With(x => x.CurrentSemester, currentSemester)
            .With(x => x.Speciality, speciality)
            .Create();

        await Context.Groups.AddAsync(group1);
        await Context.Groups.AddAsync(group2);
        await Context.SaveChangesAsync(CancellationToken.None);

        return Context.Groups.ToDictionary(x => x, x => x.CurrentSemester);
    }

    private async Task<List<int>> Action(TransferGroupsToNextSemesterCommand command)
    {
        var handler = new TransferGroupsToNextSemesterCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }

    private void Assert(List<int> GroupsId, Dictionary<Group, byte> semesters)
    {
        foreach (var group in Context.Groups.Where(x => GroupsId.Contains(x.Id)))
        {
            semesters[group].Should().Be(--group.CurrentSemester);
        }
    }
}