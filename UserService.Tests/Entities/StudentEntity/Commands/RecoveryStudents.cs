using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.RecoveryStudents;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class RecoveryStudents
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public RecoveryStudents()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task RecoveryStudents_ShouldBe_Success()
    {
        _fixture.Customize<Student>(x => x.With(x => x.IsDeleted, true));

        var students = _fixture.CreateMany<Student>(3);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([.. students]);

        var command = _fixture
            .Build<RecoveryStudentsCommand>()
            .With(x => x.StudentIds, [.. students.Select(x => x.Id)])
            .Create();

        var handler = new RecoveryStudentsCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(x => x.BeginTransactionAsync(), Times.Once);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());

        students.All(x => x.IsDeleted).Should().BeFalse();
    }

    [Fact]
    public async Task RecoveryStudents_ShouldBe_StudentNotFoundException()
    {
        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([]);

        var command = _fixture
            .Build<RecoveryStudentsCommand>()
            .With(x => x.StudentIds, [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()])
            .Create();

        var handler = new RecoveryStudentsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }

    [Fact]
    public async Task RecoveryStudents_ShouldBe_CallRallback_WhenThrowException()
    {
        var students = _fixture.CreateMany<Student>(3);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([.. students]);

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = _fixture
            .Build<RecoveryStudentsCommand>()
            .With(x => x.StudentIds, [.. students.Select(x => x.Id)])
            .Create();

        var handler = new RecoveryStudentsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
