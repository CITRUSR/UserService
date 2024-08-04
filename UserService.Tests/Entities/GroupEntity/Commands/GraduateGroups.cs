using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class GraduateGroups(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void GraduateGroup_ShouldBe_Success()
    {
        await SeedDataForTests();

        DateTime graduatedTime = DateTime.Now;

        var command = new GraduateGroupsCommand(
            Context.Groups.Select(x => x.Id).ToList(),
            graduatedTime
        );
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

    [Fact]
    public async void GraduateGroup_ShouldBe_GroupAlreadyGraduatedException()
    {
        await SeedDataForTests();

        var group = Context.Groups.First();

        group.GraduatedAt = DateTime.Now;

        await Context.SaveChangesAsync(CancellationToken.None);

        var command = new GraduateGroupsCommand(new List<int> { group.Id }, DateTime.Now);
        var handler = new GraduateGroupsCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupAlreadyGraduatedException>();
    }

    private async Task SeedDataForTests()
    {
        var students = Fixture.CreateMany<Student>(5);
        var groups = Fixture.CreateMany<Group>(3);
        foreach (var group in groups)
        {
            group.GraduatedAt = null;
            foreach (var student in students)
            {
                group.Students.Add(student);
            }
        }

        await AddGroupsToContext([.. groups]);
    }
}
