using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class EditGroup(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task EditGroup_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();

        await DbHelper.AddSpecialitiesToContext(speciality);

        var curator = Fixture.Create<Teacher>();

        await DbHelper.AddTeachersToContext(curator);

        var oldGroup = Fixture.Build<Group>().With(x => x.Id, 535478).Create();

        var newGroup = Fixture
            .Build<Group>()
            .With(x => x.CuratorId, curator.Id)
            .With(x => x.SpecialityId, speciality.Id)
            .With(x => x.Id, oldGroup.Id)
            .With(x => x.Curator, curator)
            .With(x => x.Speciality, speciality)
            .With(x => x.StartedAt, oldGroup.StartedAt)
            .With(x => x.GraduatedAt, oldGroup.GraduatedAt)
            .Create();

        await DbHelper.AddGroupsToContext(oldGroup);

        var command = new EditGroupCommand(
            newGroup.Id,
            speciality.Id,
            curator.Id,
            newGroup.CurrentCourse,
            newGroup.CurrentSemester,
            newGroup.SubGroup,
            newGroup.IsDeleted
        );

        var group = await Action(command);

        group.Should().BeEquivalentTo(newGroup);
    }

    [Fact]
    public async Task EditGroup_ShouldBe_GroupNotFoundException()
    {
        var command = Fixture.Create<EditGroupCommand>();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }

    [Fact]
    public async Task EditGroup_ShouldBe_SpecialityNotFoundException()
    {
        var group = Fixture.Create<Group>();

        await DbHelper.AddGroupsToContext(group);

        var command = Fixture.Build<EditGroupCommand>().With(x => x.Id, group.Id).Create();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async Task EditGroup_ShouldBe_TeacherNotFoundException()
    {
        var group = Fixture.Create<Group>();

        var speciality = Fixture.Create<Speciality>();

        await DbHelper.AddGroupsToContext(group);
        await DbHelper.AddSpecialitiesToContext(speciality);

        var command = Fixture
            .Build<EditGroupCommand>()
            .With(x => x.Id, group.Id)
            .With(x => x.SpecialityId, speciality.Id)
            .Create();

        Func<Task> act = async () => await Action(command);

        await act.Should().ThrowAsync<TeacherNotFoundException>();
    }

    private async Task<Group> Action(EditGroupCommand command)
    {
        var handler = new EditGroupCommandHandler(Context);

        return await handler.Handle(command, CancellationToken.None);
    }
}
