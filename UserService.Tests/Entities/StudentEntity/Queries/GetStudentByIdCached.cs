using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentByIdCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<GetStudentByIdQuery, StudentDto>> _mockHandler;
    private readonly IFixture _fixture;

    public GetStudentByIdCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<GetStudentByIdQuery, StudentDto>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetStudentByIdCached_ShouldBe_Success()
    {
        var student = _fixture.Create<StudentDto>();

        _mockCacheService
            .Setup(x =>
                x.GetOrCreateAsync<StudentDto>(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Student, Guid>(student.Id))),
                    It.IsAny<Func<Task<StudentDto>>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(student);

        var query = new GetStudentByIdQuery(student.Id);

        var handler = new GetStudentByIdQueryHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(query, default);

        _mockCacheService.Verify(
            x =>
                x.GetOrCreateAsync<StudentDto>(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Student, Guid>(student.Id))),
                    It.IsAny<Func<Task<StudentDto>>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
