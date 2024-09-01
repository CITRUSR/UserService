using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<GetStudentsQuery, PaginationList<Student>>> _mockHandler;
    private readonly GetStudentsQuery _query;

    public GetStudentsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetStudentsQuery, PaginationList<Student>>>();
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
        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<PaginationList<Student>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Student>>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(
                new PaginationList<Student> { Items = new List<Student> { new Student() } }
            );

        var handler = new GetStudentsQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
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
                x.GetOrCreateAsync<PaginationList<Student>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Student>>>>(),
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
                new PaginationList<Student> { Items = new List<Student> { new Student() } }
            );

        var handler = new GetStudentsQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var result = await handler.Handle(_query, CancellationToken.None);

        _mockHandler.Verify(x => x.Handle(_query, CancellationToken.None), Times.Once());

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<PaginationList<Student>>(
                    It.IsAny<string>(),
                    It.IsAny<Func<Task<PaginationList<Student>>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Never()
        );

        result.Should().NotBeNull();
    }
}
