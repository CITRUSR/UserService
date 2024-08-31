using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialityByIdCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<GetSpecialityByIdQuery, Speciality>> _mockHandler;
    private readonly IFixture _fixture;

    public GetSpecialityByIdCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetSpecialityByIdQuery, Speciality>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetSpecialityById_ShouldBe_Success()
    {
        var speciality = _fixture.Create<Speciality>();

        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<Speciality>(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Speciality, int>(speciality.Id))),
                    It.IsAny<Func<Task<Speciality>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(speciality);

        var query = new GetSpecialityByIdQuery(speciality.Id);

        var handler = new GetSpecialityByIdQueryHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(query, default);

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<Speciality>(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Speciality, int>(speciality.Id))),
                    It.IsAny<Func<Task<Speciality>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
