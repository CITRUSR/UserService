using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.RecoverySpecialities;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class RecoverySpecialitiesCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<RecoverySpecialitiesCommand, List<SpecialityShortInfoDto>>
    > _mockHandler;
    private readonly IFixture _fixture;

    public RecoverySpecialitiesCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler =
            new Mock<IRequestHandler<RecoverySpecialitiesCommand, List<SpecialityShortInfoDto>>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task RecoverySpecialitiesCached_ShouldBe_Success()
    {
        var specialities = _fixture.CreateMany<SpecialityShortInfoDto>(3).ToList();

        var ids = specialities.Select(x => x.Id).ToList();

        var command = _fixture
            .Build<RecoverySpecialitiesCommand>()
            .With(x => x.SpecialityIds, ids)
            .Create();

        _mockHandler
            .Setup(x =>
                x.Handle(
                    It.Is<RecoverySpecialitiesCommand>(x => x.SpecialityIds == ids),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(specialities);

        var handler = new RecoverySpecialitiesCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(x => x.Handle(command, default), Times.Once());

        foreach (var speciality in specialities)
        {
            _mockCacheService.Verify(
                x =>
                    x.RemoveAsync(
                        It.Is<string>(x =>
                            x.Equals(CacheKeys.ById<Speciality, int>(speciality.Id))
                        ),
                        default
                    ),
                Times.Once()
            );
        }

        _mockCacheService.Verify(x =>
            x.RemoveAsync(
                It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Speciality>())),
                default
            )
        );

        result.Should().NotBeNull();
    }
}
