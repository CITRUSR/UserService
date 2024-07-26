using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class GraduateGroups : CommonTest
{
    [Fact]
    public async void GraduateGroup_ShouldBe_Success()
    {
        ClearDataBase();

        var students = Fixture.CreateMany<Student>(5);
        var groups = Fixture.CreateMany<Group>();
        foreach (var group in groups)
        {
            foreach (var student in students)
            {
                group.Students.Add(student);
            }
        }


        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync();

        DateTime graduatedTime = DateTime.Now;

        var command = new GraduateGroupsCommand(groups.Select(x => x.Id).ToList(), graduatedTime);
        var handler = new GraduateGroupsCommandHandler(Context);

        var groupsRes = await handler.Handle(command, CancellationToken.None);

        foreach (var group in groupsRes)
        {
            group.GraduatedAt.Should().Be(graduatedTime);
            group.Students.Where(x => x.DroppedOutAt == null).Should().BeNullOrEmpty();
        }
    }

    [Fact]
    public async void GraduateGroup_ShouldBe_GroupNotFoundException()
    {
        var command = new GraduateGroupsCommand(new List<int> { 12 }, DateTime.Now);
        var handler = new GraduateGroupsCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}