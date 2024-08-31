using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentBySsoIdCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<GetStudentBySsoIdQuery, Student>> _mockHandler;
    private readonly IFixture _fixture;

    public GetStudentBySsoIdCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetStudentBySsoIdQuery, Student>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetStudentByIdCached_ShouldBe_Success()
    {
        var student = _fixture.Create<Student>();

        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<Student>(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Student, Guid>(student.SsoId))),
                    It.IsAny<Func<Task<Student>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(student);

        var query = new GetStudentBySsoIdQuery(student.SsoId);

        var handler = new GetStudentBySsoIdQueryHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
        );

        var result = await handler.Handle(query, default);

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<Student>(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Student, Guid>(student.SsoId))),
                    It.IsAny<Func<Task<Student>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
