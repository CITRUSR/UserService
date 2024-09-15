using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.SoftDeleteStudents;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class SoftDeleteStudents
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public SoftDeleteStudents()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task SoftDeleteStudents_ShouldBe_Success()
    {
        _fixture.Customize<Student>(x => x.With(x => x.IsDeleted, false));

        var students = _fixture.CreateMany<Student>(3);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([.. students]);

        var command = _fixture
            .Build<SoftDeleteStudentsCommand>()
            .With(x => x.StudentIds, [.. students.Select(x => x.Id)])
            .Create();

        var handler = new SoftDeleteStudentsCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());

        students.All(x => x.IsDeleted).Should().BeTrue();
    }

    [Fact]
    public async Task SoftDeleteStudents_ShouldBe_StudentsNotFoundException_WhenStudentsDoNotExist()
    {
        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([]);

        var command = _fixture
            .Build<SoftDeleteStudentsCommand>()
            .With(x => x.StudentIds, [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()])
            .Create();

        var handler = new SoftDeleteStudentsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }

    [Fact]
    public async Task SoftDeleteStudents_ShouldBe_CallRallback_WhenThrowException()
    {
        var students = _fixture.CreateMany<Student>(3);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([.. students]);

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = _fixture
            .Build<SoftDeleteStudentsCommand>()
            .With(x => x.StudentIds, [.. students.Select(x => x.Id)])
            .Create();

        var handler = new SoftDeleteStudentsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
