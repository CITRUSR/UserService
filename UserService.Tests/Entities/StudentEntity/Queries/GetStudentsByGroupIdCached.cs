using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentsByGroupId;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentsByGroupIdCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<GetStudentsByGroupIdQuery, List<StudentViewModel>>
    > _mockHandler;
    private readonly IFixture _fixture;

    public GetStudentsByGroupIdCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler =
            new Mock<IRequestHandler<GetStudentsByGroupIdQuery, List<StudentViewModel>>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetStudentsByGroupIdCached_ShouldBe_Success()
    {
        var students = _fixture.CreateMany<StudentViewModel>(20);

        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<List<StudentViewModel>>(
                    It.Is<string>(x => x.Equals(CacheKeys.EntitiesByGroupId<Student>(1))),
                    It.IsAny<Func<Task<List<StudentViewModel>>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync([.. students]);

        var query = new GetStudentsByGroupIdQuery(1);

        var handler = new GetStudentsByGroupIdQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var result = await handler.Handle(query, default);

        _mockCacheService.Verify(x =>
            x.GetOrCreateAsync<List<StudentViewModel>>(
                It.Is<string>(x => x.Equals(CacheKeys.EntitiesByGroupId<Student>(1))),
                It.IsAny<Func<Task<List<StudentViewModel>>>>(),
                It.IsAny<CancellationToken>()
            )
        );

        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
}
