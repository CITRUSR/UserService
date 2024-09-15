using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.CQRS.GroupEntity.Queries.GetGroups;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Application.Enums;

namespace UserService.Tests.Entities.GroupEntity.Queries;

public class GetGroupsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<GetGroupsQuery, GetGroupsResponse>> _mockHandler;
    private readonly Mock<ICacheOptions> _mockCacheOptions;
    private readonly GetGroupsQuery _query;

    public GetGroupsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetGroupsQuery, GetGroupsResponse>>();
        _mockCacheOptions = new Mock<ICacheOptions>();
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
        _mockCacheOptions.Setup(x => x.PagesForCaching).Returns(3);

        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<GetGroupsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetGroupsResponse>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new GetGroupsResponse
                {
                    LastPage = 1,
                    Groups = new List<GroupViewModel> { new GroupViewModel() }
                }
            );

        var handler = new GetGroupsQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object,
            _mockCacheOptions.Object
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
                x.GetOrCreateAsync<GetGroupsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetGroupsResponse>>>(),
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
            .ReturnsAsync(
                new GetGroupsResponse
                {
                    LastPage = 1,
                    Groups = new List<GroupViewModel> { new GroupViewModel() }
                }
            );

        var handler = new GetGroupsQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object,
            _mockCacheOptions.Object
        );

        var result = await handler.Handle(_query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Once());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<GetGroupsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetGroupsResponse>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never()
        );

        result.Should().NotBeNull();
    }
}
