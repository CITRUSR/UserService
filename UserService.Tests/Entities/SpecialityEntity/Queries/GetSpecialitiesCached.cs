using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.CQRS.SpecialityEntity.Responses;
using UserService.Application.Enums;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialitiesCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<GetSpecialitiesQuery, GetSpecialitiesResponse>
    > _mockHandler;
    private readonly GetSpecialitiesQuery _query;

    public GetSpecialitiesCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetSpecialitiesQuery, GetSpecialitiesResponse>>();
        _query = new GetSpecialitiesQuery
        {
            Page = 1,
            PageSize = 10,
            DeletedStatus = DeletedStatus.All,
            SearchString = "",
            SortState = SpecialitySortState.NameAsc
        };
    }

    [Fact]
    public async Task GetSpecialitiesCached_ShouldBe_Success_WhenQueryIsValid()
    {
        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<GetSpecialitiesResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetSpecialitiesResponse>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new GetSpecialitiesResponse
                {
                    LastPage = 1,
                    Specialities = new List<SpecialityViewModel> { new SpecialityViewModel() }
                }
            );

        var handler = new GetSpecialitiesQueryHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var query = _query with
        {
            SortState = SpecialitySortState.NameAsc,
            DeletedStatus = DeletedStatus.OnlyActive
        };

        var result = await handler.Handle(query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Never());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<GetSpecialitiesResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetSpecialitiesResponse>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSpecialitiesCached_ShouldBe_Success_WhenQueryIsInvalid()
    {
        _mockHandler
            .Setup(x => x.Handle(It.IsAny<GetSpecialitiesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetSpecialitiesResponse
                {
                    LastPage = 1,
                    Specialities = new List<SpecialityViewModel> { new SpecialityViewModel() }
                }
            );

        var handler = new GetSpecialitiesQueryHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(_query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Once());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<GetSpecialitiesQuery>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetSpecialitiesQuery>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never()
        );

        result.Should().NotBeNull();
    }
}
