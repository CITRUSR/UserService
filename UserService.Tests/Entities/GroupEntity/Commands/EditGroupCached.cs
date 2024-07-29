using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class EditGroupCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async void EditGroupCached_ShouldBe_Success()
    {
        var (curatorId, specialityId, oldGroup) = await SeedDataForTests();

        await CacheService.SetObjectAsync(CacheKeys.ById<Group, int>(oldGroup.Id), oldGroup);

        var newGroup = Fixture.Build<Group>()
            .With(x => x.Id, oldGroup.Id)
            .Without(x => x.Speciality)
            .Without(x => x.Curator)
            .With(x => x.CuratorId, curatorId)
            .With(x => x.SpecialityId, specialityId)
            .Create();

        var command = new EditGroupCommand(newGroup.Id, newGroup.SpecialityId, newGroup.CuratorId,
            newGroup.CurrentCourse, newGroup.CurrentSemester, newGroup.SubGroup);

        var group = await Action(command);

        var groupFromCache = await CacheService.GetObjectAsync<Group>(CacheKeys.ById<Group, int>(oldGroup.Id));

        Context.Groups.FirstOrDefault(x => x.Id == oldGroup.Id).Should().BeEquivalentTo(group);
        groupFromCache.Should().BeEquivalentTo(groupFromCache);
    }

    private async Task<(Guid curatorId, int SpecialityId, Group oldGroup)> SeedDataForTests()
    {
        var oldGroup = Fixture.Build<Group>()
            .Without(x => x.Curator)
            .Without(x => x.Speciality)
            .Create();

        var speciality = Fixture.Create<Speciality>();

        var curator = Fixture.Create<Teacher>();

        oldGroup.CuratorId = curator.Id;
        oldGroup.SpecialityId = speciality.Id;

        await AddSpecialitiesToContext(speciality);
        await AddTeachersToContext(curator);
        await AddGroupsToContext(oldGroup);

        return (curator.Id, speciality.Id, oldGroup);
    }

    private async Task<Group> Action(EditGroupCommand command)
    {
        var handler = new EditGroupCommandHandlerCached(new EditGroupCommandHandler(Context), CacheService);

        return await handler.Handle(command, CancellationToken.None);
    }
}