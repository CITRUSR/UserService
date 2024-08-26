using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.GroupEntity.Commands.CreateGroup;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Commands;

public class CreateGroupCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<CreateGroupCommand, Group>> _mockHandler;
    private readonly CreateGroupCommand _command;
    private readonly IFixture _fixture;

    public CreateGroupCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<CreateGroupCommand, Group>>();
        _fixture = new Fixture();
        _command = _fixture.Create<CreateGroupCommand>();
    }

    [Fact]
    public async Task CreateGroupCached_ShouldBe_Success()
    {
        var handler = new CreateGroupCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(_command, default);

        _mockHandler.Verify(x => x.Handle(_command, default), Times.Once());

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
