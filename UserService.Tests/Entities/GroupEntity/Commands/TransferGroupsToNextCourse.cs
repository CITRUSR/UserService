using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class TransferGroupsToNextCourse(DatabaseFixture databaseFixture)
    : CommonTest(databaseFixture)
{
    [Fact]
    public async void TransferGroupsToNextCourse_ShouldBe_SuccessWithList()
    {
        var courses = await Arrange(2);

        var command = new TransferGroupsToNextCourseCommand(
            Context.Groups.Select(x => x.Id).ToList()
        );

        var groups = await Action(command);

        Assert(groups, courses);
    }

    [Fact]
    public async void TransferGroupsToNextCourse_ShouldBe_GroupCourseOutOfRangeException()
    {
        await Arrange(4);

        var command = new TransferGroupsToNextCourseCommand(
            Context.Groups.Select(x => x.Id).ToList()
        );

        Func<Task> act = () => Action(command);

        await act.Should().ThrowAsync<GroupCourseOutOfRangeException>();
    }

    [Fact]
    public async void TransferGroupsToNextCourse_ShouldBe_GroupNotFoundException()
    {
        var command = new TransferGroupsToNextCourseCommand([123]);

        Func<Task> act = () => Action(command);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    private async Task<Dictionary<Group, byte>> Arrange(int CurrentCourse)
    {
        var speciality = Fixture.Build<Speciality>().With(x => x.DurationMonths, 46).Create();

        var group1 = Fixture
            .Build<Group>()
            .With(x => x.CurrentCourse, CurrentCourse)
            .With(x => x.Speciality, speciality)
            .Create();

        var group2 = Fixture
            .Build<Group>()
            .With(x => x.CurrentCourse, CurrentCourse)
            .With(x => x.Speciality, speciality)
            .Create();

        await AddGroupsToContext(group1, group2);

        return Context.Groups.ToDictionary(x => x, x => x.CurrentCourse);
    }

    private async Task<List<Group>> Action(TransferGroupsToNextCourseCommand command)
    {
        var handler = new TransferGroupsToNextCourseCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }

    private void Assert(List<Group> groups, Dictionary<Group, byte> courses)
    {
        foreach (var group in groups)
        {
            courses[group].Should().Be(--group.CurrentCourse);
        }
    }
}
