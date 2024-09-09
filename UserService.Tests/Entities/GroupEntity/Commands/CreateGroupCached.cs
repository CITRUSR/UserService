using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class CreateGroupCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<CreateGroupCommand, GroupShortInfoDto>> _mockHandler;
    private readonly IFixture _fixture;

    public CreateGroupCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<CreateGroupCommand, GroupShortInfoDto>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CreateGroupCached_ShouldBe_Success()
    {
        var group = _fixture.Create<GroupShortInfoDto>();

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<CreateGroupCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var command = _fixture.Create<CreateGroupCommand>();

        var handler = new CreateGroupCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(
            x => x.Handle(It.IsAny<CreateGroupCommand>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Group>())),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
