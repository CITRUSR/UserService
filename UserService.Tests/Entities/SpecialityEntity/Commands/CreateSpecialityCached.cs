using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.CreateSpeciality;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class CreateSpecialityCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<CreateSpecialityCommand, Speciality>> _mockHandler;
    private readonly IFixture _fixture;

    public CreateSpecialityCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<CreateSpecialityCommand, Speciality>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CreateSpecialityCached_ShouldBe_Success()
    {
        var group = _fixture.Create<Speciality>();

        _mockHandler
            .Setup(x =>
                x.Handle(It.IsAny<CreateSpecialityCommand>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(group);

        var command = _fixture.Create<CreateSpecialityCommand>();

        var handler = new CreateSpecialityCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(
            x => x.Handle(It.IsAny<CreateSpecialityCommand>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Speciality>())),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
