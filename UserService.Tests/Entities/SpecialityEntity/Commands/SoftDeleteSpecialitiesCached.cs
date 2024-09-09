using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.SoftDeleteSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class SoftDeleteSpecialitiesCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<SoftDeleteSpecialitiesCommand, List<SpecialityShortInfoDto>>
    > _mockHandler;
    private readonly IFixture _fixture;

    public SoftDeleteSpecialitiesCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler =
            new Mock<
                IRequestHandler<SoftDeleteSpecialitiesCommand, List<SpecialityShortInfoDto>>
            >();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task SoftDeleteSpecialititesCached_ShouldBe_Success()
    {
        var groups = _fixture.CreateMany<SpecialityShortInfoDto>(3).ToList();
        var ids = groups.Select(x => x.Id).ToList();

        var command = _fixture
            .Build<SoftDeleteSpecialitiesCommand>()
            .With(x => x.SpecialitiesId, ids)
            .Create();

        _mockHandler
            .Setup(x =>
                x.Handle(
                    It.Is<SoftDeleteSpecialitiesCommand>(x => x.SpecialitiesId == ids),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(groups);

        var handler = new SoftDeleteSpecialitiesCommandHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(x => x.Handle(command, default), Times.Once());

        foreach (var group in groups)
        {
            _mockCacheService.Verify(
                x =>
                    x.RemoveAsync(
                        It.Is<string>(x => x.Equals(CacheKeys.ById<Speciality, int>(group.Id))),
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
