using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DeleteStudentsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<DeleteStudentsCommand, List<StudentShortInfoDto>>
    > _mockHandler;
    private readonly IFixture _fixture;

    public DeleteStudentsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler =
            new Mock<IRequestHandler<DeleteStudentsCommand, List<StudentShortInfoDto>>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteStudentsCached_ShouldBe_Success()
    {
        var students = _fixture.Create<List<StudentShortInfoDto>>();

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<DeleteStudentsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        var command = new DeleteStudentsCommand([.. students.Select(x => x.Id)]);

        var handler = new DeleteStudentsCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(x => x.Handle(command, default), Times.Once());

        foreach (var student in students)
        {
            _mockCacheService.Verify(
                x =>
                    x.RemoveAsync(
                        It.Is<string>(x => x.Equals(CacheKeys.ById<Student, Guid>(student.Id))),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once()
            );
        }

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Student>())),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

        result.Should().NotBeNull();
    }
}
