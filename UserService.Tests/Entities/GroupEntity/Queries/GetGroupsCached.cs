using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<GetGroupsQuery, PaginationList<Group>>> _mockHandler;
    private readonly IFixture _fixture;
    private readonly GetGroupsQuery _query;

    public GetGroupsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetGroupsQuery, PaginationList<Group>>>();
        _fixture = new Fixture();
        _query = new GetGroupsQuery
        {
            Page = 1,
            PageSize = 10,
            DeletedStatus = DeletedStatus.All,
            GraduatedStatus = GroupGraduatedStatus.All,
            SearchString = "",
            SortState = GroupSortState.GroupAsc
        };
    }

    [Fact]
    public async Task GetGroupsCached_ShouldBe_Success_WhenQueryIsValid()
    {
        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<PaginationList<Group>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Group>>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(new PaginationList<Group> { Items = new List<Group> { new Group() } });

        var handler = new GetGroupsQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var query = _query with
        {
            GraduatedStatus = GroupGraduatedStatus.OnlyActive,
            DeletedStatus = DeletedStatus.OnlyActive
        };

        var result = await handler.Handle(query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Never());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<PaginationList<Group>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Group>>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetGroupsCached_ShouldBe_Success_WhenQueryIsInvalid()
    {
        _mockHandler
            .Setup(x => x.Handle(It.IsAny<GetGroupsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaginationList<Group> { Items = new List<Group> { new Group() } });

        var handler = new GetGroupsQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var result = await handler.Handle(_query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Once());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<PaginationList<Group>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Group>>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never()
        );

        result.Should().NotBeNull();
    }
}
