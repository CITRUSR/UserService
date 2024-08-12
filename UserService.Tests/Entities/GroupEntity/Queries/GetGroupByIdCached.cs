using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupByIdCached(DatabaseFixture databaseFixture) : RedisTest(databaseFixture)
{
    [Fact]
    public async Task GetGroupByIdCached_ShouldBe_SuccessWithoutCache()
    {
        var speciality = Fixture.Create<Speciality>();
        var curator = Fixture.Create<Teacher>();

        var group = CreateGroup(curator.Id, speciality.Id);

        await AddTeachersToContext(curator);
        await AddSpecialitiesToContext(speciality);
        await AddGroupsToContext(group);

        var query = new GetGroupByIdQuery(group.Id);

        var groupRes = await Action(query);
        var groupFromCache = await CacheService.GetObjectAsync<Group>(
            CacheKeys.ById<Group, int>(group.Id)
        );

        Context.Groups.Find(groupRes.Id).Should().BeEquivalentTo(group);
        groupFromCache
            .Should()
            .BeEquivalentTo(
                group,
                options => options.Excluding(x => x.Curator).Excluding(x => x.Speciality)
            );
    }

    [Fact]
    public async Task GetGroupByIdCached_ShouldBe_SuccessWithCache()
    {
        var group = CreateGroup(Guid.NewGuid(), 123);

        await CacheService.SetObjectAsync(CacheKeys.ById<Group, int>(group.Id), group);

        var query = new GetGroupByIdQuery(group.Id);

        var groupRes = await Action(query);
        var groupFromCache = await CacheService.GetObjectAsync<Group>(
            CacheKeys.ById<Group, int>(group.Id)
        );

        groupRes.Should().BeEquivalentTo(group);
        groupFromCache.Should().BeEquivalentTo(group);
    }

    private Group CreateGroup(Guid curatorId, int specialityId)
    {
        return Fixture
            .Build<Group>()
            .Without(x => x.Curator)
            .Without(x => x.Speciality)
            .With(x => x.SpecialityId, specialityId)
            .With(x => x.CuratorId, curatorId)
            .Create();
    }

    private async Task<Group> Action(GetGroupByIdQuery query)
    {
        var handler = new GetGroupByIdQueryHandlerCached(
            new GetGroupByIdQueryHandler(Context),
            CacheService
        );

        return await handler.Handle(query, CancellationToken.None);
    }
}
