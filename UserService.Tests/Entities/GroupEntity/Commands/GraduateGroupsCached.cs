using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.GraduateGroups;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class GraduateGroupsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<GraduateGroupsCommand, List<GroupShortInfoDto>>
    > _mockHandler;
    private readonly IFixture _fixture;

    public GraduateGroupsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GraduateGroupsCommand, List<GroupShortInfoDto>>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GraduateGroupsCached_ShouldBe_Success()
    {
        var groups = _fixture.CreateMany<GroupShortInfoDto>(3).ToList();
        var ids = groups.Select(x => x.Id).ToList();

        var command = _fixture.Build<GraduateGroupsCommand>().With(x => x.GroupsId, ids).Create();

        _mockHandler
            .Setup(x =>
                x.Handle(
                    It.Is<GraduateGroupsCommand>(x => x.GroupsId == ids),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(groups);

        var handler = new GraduateGroupsCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(x => x.Handle(command, default), Times.Once());

        foreach (var group in groups)
        {
            _mockCacheService.Verify(
                x =>
                    x.RemoveAsync(
                        It.Is<string>(x => x.Equals(CacheKeys.ById<Group, int>(group.Id))),
                        default
                    ),
                Times.Once()
            );
        }

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Group>())),
                    default
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
