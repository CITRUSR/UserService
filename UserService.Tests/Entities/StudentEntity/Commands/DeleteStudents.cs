using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DeleteStudents
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public DeleteStudents()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteStudents_ShouldBe_Success()
    {
        var students = _fixture.CreateMany<Student>(5);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([.. students]);

        var command = _fixture
            .Build<DeleteStudentsCommand>()
            .With(x => x.StudentIds, students.Select(x => x.Id).ToList())
            .Create();

        var handler = new DeleteStudentsCommandHandler(_mockDbContext.Object);

        await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(x => x.BeginTransactionAsync(), Times.Once());

        _mockDbContext.Verify(
            x => x.Students.RemoveRange(It.Is<List<Student>>(x => x.SequenceEqual(students))),
            Times.Once()
        );

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        _mockDbContext.Verify(x => x.CommitTransactionAsync(), Times.Once());
    }

    [Fact]
    public async Task DeleteStudents_ShouldBe_StudentNotFoundException_WhenStudentsDoNotExist()
    {
        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([]);

        var command = new DeleteStudentsCommand([Guid.NewGuid(), Guid.NewGuid()]);

        var handler = new DeleteStudentsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }

    [Fact]
    public async Task DeleteStudents_ShouldBe_CallRallback_WhenThrowException()
    {
        var students = _fixture.CreateMany<Student>(5);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([.. students]);

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = _fixture
            .Build<DeleteStudentsCommand>()
            .With(x => x.StudentIds, students.Select(x => x.Id).ToList())
            .Create();

        var handler = new DeleteStudentsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
