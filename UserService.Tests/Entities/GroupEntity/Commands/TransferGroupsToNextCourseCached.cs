using FluentAssertions;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class TransferGroupsToNextCourseCached(DatabaseFixture databaseFixture)
    : RedisTest(databaseFixture)
{
    [Fact]
    public async void TransferGroupsToNextCourseCached_ShouldBe_Success()
    {
        var group = Fixture.Build<Group>().With(x => x.CurrentCourse, 2).Create();

        await AddGroupsToContext(group);

        var command = new TransferGroupsToNextCourseCommand([group.Id]);

        var handler = new TransferGroupsToNextCourseCommandHandlerCached(
            CacheService,
            new TransferGroupsToNextCourseCommandHandler(Context)
        );

        var groupRes = await handler.Handle(command, CancellationToken.None);

        var key = CacheKeys.ById<Group, int>(group.Id);

        var cachedString = await CacheService.GetStringAsync(key);

        var groupFromCache = await CacheService.GetObjectAsync<Group>(key);

        cachedString.Should().NotBeNullOrEmpty();
        groupFromCache
            .Should()
            .BeEquivalentTo(
                group,
                options => options.Excluding(x => x.Curator).Excluding(x => x.Speciality)
            );
    }
}
