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
    public async Task TransferGroupsToNextSemesterCached_ShouldBe_Success()
    {
        var speciality = Fixture.Build<Speciality>().With(x => x.DurationMonths, 48).Create();

        await DbHelper.AddSpecialitiesToContext(speciality);

        var group = Fixture
            .Build<Group>()
            .With(x => x.CurrentSemester, 2)
            .With(x => x.Speciality, speciality)
            .Create();

        await DbHelper.AddGroupsToContext(group);

        var key = CacheKeys.ById<Group, int>(group.Id);

        await CacheService.SetObjectAsync<Group>(key, group);

        var command = new TransferGroupsToNextSemesterCommand([group.Id]);

        var handler = new TransferGroupsToNextSemesterCommandHandlerCached(
            new TransferGroupsToNextSemesterCommandHandler(Context),
            CacheService
        );

        var groups = await handler.Handle(command, CancellationToken.None);

        var groupFromCache = await CacheService.GetObjectAsync<Group>(key);

        groupFromCache.Should().BeNull();
    }
}
