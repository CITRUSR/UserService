using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialities;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.SpecialityEntity.Queries;

public class GetSpecialitiesCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<GetSpecialitiesQuery, PaginationList<Speciality>>
    > _mockHandler;
    private readonly GetSpecialitiesQuery _query;

    public GetSpecialitiesCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler =
            new Mock<IRequestHandler<GetSpecialitiesQuery, PaginationList<Speciality>>>();
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
                x.GetOrCreateAsync<PaginationList<Speciality>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Speciality>>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new PaginationList<Speciality> { Items = new List<Speciality> { new Speciality() } }
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
                x.GetOrCreateAsync<PaginationList<Speciality>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Speciality>>>>(),
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
                new PaginationList<Speciality> { Items = new List<Speciality> { new Speciality() } }
            );

        var handler = new GetSpecialitiesQueryHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(_query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Once());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<PaginationList<Speciality>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Speciality>>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never()
        );

        result.Should().NotBeNull();
    }
}
