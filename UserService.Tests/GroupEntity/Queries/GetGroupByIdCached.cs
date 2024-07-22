using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;
using UserService.Domain.Entities;
using UserService.Persistance.Extensions;
using UserService.Tests.Common;

namespace UserService.Tests.GroupEntity.Queries;

public class GetGroupByIdCached : RedisTest
{
    [Fact]
    public async void GetGroupByIdCached_ShouldBe_SuccessWithoutCache()
    {
        ClearDataBase();

        var group = Fixture.Build<Group>()
            .Without(x => x.Curator)
            .Without(x => x.Speciality)
            .Create();

        await Context.Groups.AddAsync(group);
        await Context.SaveChangesAsync();

        var query = new GetGroupByIdQuery(group.Id);

        var groupRes = await Action(query);

        Context.Groups.Find(groupRes.Id).Should().BeEquivalentTo(group);
        JsonConvert.DeserializeObject<Group>(Redis.GetString(CacheKeys.GroupById(query.Id))).Should()
            .BeEquivalentTo(group);
    }

    [Fact]
    public async void GetGroupByIdCached_ShouldBe_SuccessWithCache()
    {
        var group = Fixture.Build<Group>()
            .Without(x => x.Curator)
            .Without(x => x.Speciality)
            .Create();

        await Redis.SetObjetAsync(CacheKeys.GroupById(group.Id), group);

        var query = new GetGroupByIdQuery(group.Id);

        var groupRes = await Action(query);

        groupRes.Should().BeEquivalentTo(group);
        JsonConvert.DeserializeObject<Group>(Redis.GetString(CacheKeys.GroupById(query.Id))).Should()
            .BeEquivalentTo(group);
    }

    private async Task<Group> Action(GetGroupByIdQuery query)
    {
        var handler = new GetGroupsByIdQueryHandlerCached(new GetGroupByIdQueryHandler(Context), Redis);

        return await handler.Handle(query, CancellationToken.None);
    }
}