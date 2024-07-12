using FluentAssertions;
using UserService.Application.CQRS.Group.Commands.TransferGroupsToNextCourse;
using UserService.Tests.Common;

namespace UserService.Tests.Group.Commands;

public class TransferGroupsToNextCourse : CommonTest
{
    [Fact]
    public async void TransferGroupsToNextCourse_ShouldBe_SuccessWithList()
    {
        var groups = Fixture.CreateMany<Domain.Entities.Group>(5);

        var courses = groups.ToDictionary(x => x, x => x.CurrentCourse);

        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync();

        var command = new TransferGroupsToNextCourseCommand
        {
            IdGroups = groups.Select(x => x.Id).ToList(),
        };

        var handler = new TransferGroupsToNextCourseCommandHandler(Context);

        var ids = await handler.Handle(command, CancellationToken.None);

        foreach (var group in Context.Groups.Where(x => ids.Contains(x.Id)))
        {
            courses[group].Should().Be(--group.CurrentCourse);
        }
    }

    [Fact]
    public async void TransferGroupsToNextCourse_ShouldBe_SuccessWithoutList()
    {
        var groups = Fixture.CreateMany<Domain.Entities.Group>(5);

        var courses = groups.ToDictionary(x => x, x => x.CurrentCourse);

        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync();

        var command = new TransferGroupsToNextCourseCommand();
        var handler = new TransferGroupsToNextCourseCommandHandler(Context);

        var ids = await handler.Handle(command, CancellationToken.None);

        foreach (var group in Context.Groups.Where(x => ids.Contains(x.Id)))
        {
            courses[group].Should().Be(--group.CurrentCourse);
        }
    }
}