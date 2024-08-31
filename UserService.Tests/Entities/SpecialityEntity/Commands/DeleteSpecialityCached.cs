using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Commands.DeleteSpeciality;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Commands;

public class DeleteSpecialitiesCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<DeleteSpecialityCommand, List<Speciality>>> _mockHandler;
    private readonly IFixture _fixture;

    public DeleteSpecialitiesCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<DeleteSpecialityCommand, List<Speciality>>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteSpecialitiesCached_ShouldBe_Success()
    {
        var handler = new DeleteSpecialityCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var specialities = _fixture.CreateMany<Speciality>(3).ToList();
        var ids = specialities.Select(x => x.Id).ToList();

        var command = _fixture
            .Build<DeleteSpecialityCommand>()
            .With(x => x.SpecialitiesId, ids)
            .Create();

        _mockHandler
            .Setup(x =>
                x.Handle(
                    It.Is<DeleteSpecialityCommand>(x => x.SpecialitiesId == ids),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(specialities);

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

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Speciality>())),
                    default
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
