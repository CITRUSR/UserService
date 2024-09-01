using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.DeleteGroups;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class DeleteGroupsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<DeleteGroupsCommand, List<GroupShortInfoDto>>
    > _mockHandler;
    private readonly IFixture _fixture;

    public DeleteGroupsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<DeleteGroupsCommand, List<GroupShortInfoDto>>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteGroupsCached_ShouldBe_Success()
    {
        var handler = new DeleteGroupsCommandHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var groups = _fixture.CreateMany<GroupShortInfoDto>(3).ToList();
        var ids = groups.Select(x => x.Id).ToList();

        var command = _fixture.Build<DeleteGroupsCommand>().With(x => x.Ids, ids).Create();

        _mockHandler
            .Setup(x =>
                x.Handle(
                    It.Is<DeleteGroupsCommand>(x => x.Ids == ids),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(groups);

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
    }
}
