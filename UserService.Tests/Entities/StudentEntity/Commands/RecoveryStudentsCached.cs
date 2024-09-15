using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.RecoveryStudents;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class RecoveryStudentsCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<
        IRequestHandler<RecoveryStudentsCommand, List<StudentShortInfoDto>>
    > _mockHandler;
    private readonly IFixture _fixture;

    public RecoveryStudentsCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler =
            new Mock<IRequestHandler<RecoveryStudentsCommand, List<StudentShortInfoDto>>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task RecoveryStudentsCached_ShouldBe_Success()
    {
        var students = _fixture.CreateMany<StudentShortInfoDto>(3).ToList();

        var ids = students.Select(x => x.Id).ToList();

        var command = _fixture
            .Build<RecoveryStudentsCommand>()
            .With(x => x.StudentIds, ids)
            .Create();

        _mockHandler
            .Setup(x =>
                x.Handle(
                    It.Is<RecoveryStudentsCommand>(x => x.StudentIds == ids),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(students);

        var handler = new RecoveryStudentsCommandHandlerCached(
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
                        default
                    ),
                Times.Once()
            );
        }

        _mockCacheService.Verify(x =>
            x.RemoveAsync(It.Is<string>(x => x.Equals(CacheKeys.GetEntities<Student>())), default)
        );

        result.Should().NotBeNull();
    }
}
