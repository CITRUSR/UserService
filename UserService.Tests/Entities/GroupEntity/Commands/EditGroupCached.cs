using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.EditGroup;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class EditGroupCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<EditGroupCommand, GroupShortInfoDto>> _mockHandler;
    private readonly EditGroupCommand _command;
    private readonly IFixture _fixture;

    public EditGroupCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<EditGroupCommand, GroupShortInfoDto>>();
        _fixture = new Fixture();
        _command = _fixture.Create<EditGroupCommand>();
    }

    [Fact]
    public async Task EditGroupCached_ShouldBe_Success()
    {
        var group = _fixture.Create<GroupShortInfoDto>();

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<EditGroupCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        var handler = new EditGroupCommandHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var result = await handler.Handle(_command, default);

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Group, int>(_command.Id))),
                    default
                ),
            Times.Once()
        );

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
