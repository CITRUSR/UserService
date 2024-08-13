using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class TransferGroupsToNextSemester(DatabaseFixture databaseFixture)
    : CommonTest(databaseFixture)
{
    [Fact]
    public async Task TransferGroupsToNextSemester_ShouldBe_SuccessWithList()
    {
        var semesters = await Arrange(6);

        var command = new TransferGroupsToNextSemesterCommand(
            Context.Groups.Select(x => x.Id).ToList()
        );

        var groups = await Action(command);

        Assert(groups, semesters);
    }

    [Fact]
    public async Task TransferGroupsToNextSemester_ShouldBe_GroupSemesterOutOfRange()
    {
        await Arrange(8);

        var command = new TransferGroupsToNextSemesterCommand(
            Context.Groups.Select(x => x.Id).ToList()
        );

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<GroupSemesterOutOfRangeException>();
    }

    [Fact]
    public async Task TransferGroupsToNextSemester_ShouldBe_GroupNotFoundException()
    {
        var command = new TransferGroupsToNextSemesterCommand([123]);

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    private async Task<Dictionary<Group, byte>> Arrange(int currentSemester)
    {
        var speciality = Fixture.Build<Speciality>().With(x => x.DurationMonths, 46).Create();

        var group1 = Fixture
            .Build<Group>()
            .With(x => x.CurrentSemester, currentSemester)
            .With(x => x.Speciality, speciality)
            .Create();

        var group2 = Fixture
            .Build<Group>()
            .With(x => x.CurrentSemester, currentSemester)
            .With(x => x.Speciality, speciality)
            .Create();

        await DbHelper.AddSpecialitiesToContext(speciality);
        await DbHelper.AddGroupsToContext(group1, group2);

        return Context.Groups.ToDictionary(x => x, x => x.CurrentSemester);
    }

    private async Task<List<Group>> Action(TransferGroupsToNextSemesterCommand command)
    {
        var handler = new TransferGroupsToNextSemesterCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }

    private void Assert(List<Group> groups, Dictionary<Group, byte> semesters)
    {
        foreach (var group in groups)
        {
            semesters[group].Should().Be(--group.CurrentSemester);
        }
    }
}
