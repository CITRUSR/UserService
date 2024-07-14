using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.Group.Commands.EditGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Group.Commands;

public class EditGroup : CommonTest
{
    [Fact]
    public async void EditGroup_ShouldBe_Success()
    {
        ClearDataBase();

        var speciality = Fixture.Create<Speciality>();
        var curator = Fixture.Create<Teacher>();

        var oldGroup = Fixture.Create<Domain.Entities.Group>();

        var newGroup = Fixture.Build<Domain.Entities.Group>()
            .With(x => x.CuratorId, curator.Id)
            .With(x => x.SpecialityId, speciality.Id)
            .With(x => x.Id, oldGroup.Id)
            .With(x => x.Curator, curator)
            .With(x => x.Speciality, speciality)
            .With(x => x.StartedAt, oldGroup.StartedAt)
            .With(x => x.GraduatedAt, oldGroup.GraduatedAt)
            .Create();

        await Context.Specialities.AddAsync(speciality);
        await Context.Teachers.AddAsync(curator);
        await Context.Groups.AddAsync(oldGroup);

        await Context.SaveChangesAsync();

        var command = new EditGroupCommand(newGroup.Id, speciality.Id, curator.Id, newGroup.CurrentCourse,
            newGroup.CurrentSemester, newGroup.SubGroup);

        var id = await Action(command);

        Context.Groups.FirstOrDefault(x => x.Id == id).Should().BeEquivalentTo(newGroup);
    }

    [Fact]
    public async void EditGroup_ShouldBe_GroupNotFoundException()
    {
        var command = Fixture.Create<EditGroupCommand>();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Fact]
    public async void EditGroup_ShouldBe_SpecialityNotFoundException()
    {
        var group = Fixture.Create<Domain.Entities.Group>();

        await Context.Groups.AddAsync(group);
        await Context.SaveChangesAsync();

        var command = Fixture.Build<EditGroupCommand>()
            .With(x => x.Id, group.Id)
            .Create();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async void EditGroup_ShouldBe_TeacherNotFoundException()
    {
        var group = Fixture.Create<Domain.Entities.Group>();

        var speciality = Fixture.Create<Speciality>();

        await Context.Groups.AddAsync(group);
        await Context.Specialities.AddAsync(speciality);
        await Context.SaveChangesAsync();

        var command = Fixture.Build<EditGroupCommand>()
            .With(x => x.Id, group.Id)
            .With(x => x.SpecialityId, speciality.Id)
            .Create();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<TeacherNotFoundException>();
    }

    private async Task<int> Action(EditGroupCommand command)
    {
        var handler = new EditGroupCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}