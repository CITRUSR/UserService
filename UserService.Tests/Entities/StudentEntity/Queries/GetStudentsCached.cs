using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Application.Enums;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<GetStudentsQuery, GetStudentsResponse>> _mockHandler;
    private readonly Mock<ICacheOptions> _mockCacheOptions;
    private readonly GetStudentsQuery _query;

    public GetStudentsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetStudentsQuery, GetStudentsResponse>>();
        _mockCacheOptions = new Mock<ICacheOptions>();
        _query = new GetStudentsQuery
        {
            Page = 1,
            PageSize = 10,
            DeletedStatus = DeletedStatus.All,
            DroppedOutStatus = StudentDroppedOutStatus.All,
            SearchString = "",
            SortState = SortState.LastNameAsc,
        };
    }

    [Fact]
    public async Task GetStudentsCached_ShouldBe_Success_WhenQueryIsValid()
    {
        _mockCacheOptions.Setup(x => x.PagesForCaching).Returns(3);

        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<GetStudentsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetStudentsResponse>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new GetStudentsResponse
                {
                    LastPage = 1,
                    Students = new List<StudentViewModel> { new StudentViewModel() }
                }
            );

        var handler = new GetStudentsQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object,
            _mockCacheOptions.Object
        );

        var query = _query with
        {
            DroppedOutStatus = StudentDroppedOutStatus.OnlyActive,
            DeletedStatus = DeletedStatus.OnlyActive
        };

        var result = await handler.Handle(query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Never());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<GetStudentsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetStudentsResponse>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetStudentsCached_ShouldBe_Success_WhenQueryIsInvalid()
    {
        _mockHandler
            .Setup(x => x.Handle(It.IsAny<GetStudentsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new GetStudentsResponse
                {
                    LastPage = 1,
                    Students = new List<StudentViewModel> { new StudentViewModel() }
                }
            );

        var handler = new GetStudentsQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object,
            _mockCacheOptions.Object
        );

        var result = await handler.Handle(_query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Once());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<GetStudentsResponse>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<GetStudentsResponse>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never()
        );

        result.Should().NotBeNull();
    }
}
