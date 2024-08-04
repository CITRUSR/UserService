using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class TransferGroupsToNextSemesterCached(DatabaseFixture databaseFixture)
    : RedisTest(databaseFixture)
{
    [Fact]
    public async void TransferGroupsToNextSemesterCached_ShouldBe_Success()
    {
        var group = Fixture.Build<Group>().With(x => x.CurrentSemester, 2).Create();

        await AddGroupsToContext(group);

        var command = new TransferGroupsToNextSemesterCommand([group.Id]);

        var handler = new TransferGroupsToNextSemesterCommandHandlerCached(
            new TransferGroupsToNextSemesterCommandHandler(Context),
            CacheService
        );

        var groups = await handler.Handle(command, CancellationToken.None);

        var key = CacheKeys.ById<Group, int>(group.Id);

        var cachedString = await CacheService.GetStringAsync(key);

        var groupFromCache = await CacheService.GetObjectAsync<Group>(key);

        cachedString.Should().NotBeNullOrEmpty();

        for (int i = 0; i < CacheConstants.PagesForCaching; i++)
        {
            var cacheString = await CacheService.GetStringAsync(
                CacheKeys.GetEntities<Group>(i, 10)
            );

            cacheString.Should().BeNull();
        }

        groupFromCache
            .Should()
            .BeEquivalentTo(
                group,
                options => options.Excluding(x => x.Curator).Excluding(x => x.Speciality)
            );
    }
}
