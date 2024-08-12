using FluentAssertions;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class CreateGroup(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task CreateGroup_ShouldBe_Success()
    {
        var speciality = Fixture.Create<Speciality>();
        var curator = Fixture.Create<Teacher>();

        await AddSpecialitiesToContext(speciality);
        await AddTeachersToContext(curator);

        var command = Fixture
            .Build<CreateGroupCommand>()
            .With(x => x.CuratorId, curator.Id)
            .With(x => x.SpecialityId, speciality.Id)
            .Create();

        var handler = new CreateGroupCommandHandler(Context);

        var group = await handler.Handle(command, CancellationToken.None);
        Context.Groups.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateGroup_ShouldBe_SpecialityNotFoundException()
    {
        var curator = Fixture.Create<Teacher>();

        await AddTeachersToContext(curator);

        var command = Fixture
            .Build<CreateGroupCommand>()
            .With(x => x.CuratorId, curator.Id)
            .Create();

        var handler = new CreateGroupCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<SpecialityNotFoundException>();
    }

    [Fact]
    public async Task CreateGroup_ShouldBe_TeacherNotFoundException()
    {
        var speciality = Fixture.Create<Speciality>();

        await AddSpecialitiesToContext(speciality);

        var command = Fixture
            .Build<CreateGroupCommand>()
            .With(x => x.SpecialityId, speciality.Id)
            .Create();

        var handler = new CreateGroupCommandHandler(Context);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);
        await act.Should().ThrowAsync<TeacherNotFoundException>();
    }
}
