using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class GraduateGroup : CommonTest
{
    [Fact]
    public async void GraduateGroup_ShouldBe_Success()
    {
        ClearDataBase();

        var students = Fixture.CreateMany<Student>(5);
        var group = Fixture.Create<Group>();
        foreach (var student in students)
        {
            group.Students.Add(student);
        }

        await Context.Groups.AddAsync(group);
        await Context.SaveChangesAsync();

        DateTime graduatedTime = DateTime.Now;

        var command = new GraduateGroupCommand(group.Id, graduatedTime);
        var handler = new GraduateGroupCommandHandler(Context);

        var id = await handler.Handle(command, CancellationToken.None);
        group.GraduatedAt.Should().Be(graduatedTime);
        group.Students.Where(x => x.DroppedOutAt == null).Should().BeNullOrEmpty();
    }

    [Fact]
    public async void GraduateGroup_ShouldBe_GroupNotFoundException()
    {
        var command = new GraduateGroupCommand(123, DateTime.Now);
        var handler = new GraduateGroupCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}