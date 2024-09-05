using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DropOutStudentsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>>
    > _mockHandler;
    private readonly IFixture _fixture;

    public DropOutStudentsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler =
            new Mock<IRequestHandler<DropOutStudentsCommand, List<StudentShortInfoDto>>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DropOutStudentsCached_ShouldBe_Success()
    {
        var students = _fixture.Create<List<StudentShortInfoDto>>();

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<DropOutStudentsCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(students);

        var command = new DropOutStudentsCommand([.. students.Select(x => x.Id)], DateTime.Now);

        var handler = new DropOutStudentsCommandHandlerCached(
            _mockHandler.Object,
            _mockCacheService.Object
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
