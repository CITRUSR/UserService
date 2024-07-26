using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupByIdCached : RedisTest
{
    [Fact]
    public async void GetGroupByIdCached_ShouldBe_SuccessWithoutCache()
    {
        ClearDataBase();

        var group = CreateGroup();

        await Context.Groups.AddAsync(group);
        await Context.SaveChangesAsync();

        var query = new GetGroupByIdQuery(group.Id);

        var groupRes = await Action(query);
        var groupFromCache = await CacheService.GetObjectAsync<Group>(CacheKeys.ById<Group, int>(group.Id));

        Context.Groups.Find(groupRes.Id).Should().BeEquivalentTo(group);
        groupFromCache.Should().BeEquivalentTo(group);
    }

    [Fact]
    public async void GetGroupByIdCached_ShouldBe_SuccessWithCache()
    {
        var group = CreateGroup();

        await CacheService.SetObjectAsync(CacheKeys.ById<Group, int>(group.Id), group);

        var query = new GetGroupByIdQuery(group.Id);

        var groupRes = await Action(query);
        var groupFromCache = await CacheService.GetObjectAsync<Group>(CacheKeys.ById<Group, int>(group.Id));

        groupRes.Should().BeEquivalentTo(group);
        groupFromCache.Should().BeEquivalentTo(group);
    }

    private Group CreateGroup()
    {
        return Fixture.Build<Group>()
            .Without(x => x.Curator)
            .Without(x => x.Speciality)
            .Create();
    }

    private async Task<Group> Action(GetGroupByIdQuery query)
    {
        var handler = new GetGroupByIdQueryHandlerCached(new GetGroupByIdQueryHandler(Context), CacheService);

        return await handler.Handle(query, CancellationToken.None);
    }
}