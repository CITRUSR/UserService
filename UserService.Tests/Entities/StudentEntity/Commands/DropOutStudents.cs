using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DropOutStudents
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public DropOutStudents()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _fixture.Customize<Student>(x => x.Without(x => x.DroppedOutAt));
    }

    [Fact]
    public async Task DropOutStudents_ShouldBe_Success()
    {
        var students = _fixture.CreateMany<Student>(4);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet(students);

        var command = new DropOutStudentsCommand([.. students.Select(x => x.Id)], DateTime.Now);

        var handler = new DropOutStudentsCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(command, default);

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        foreach (var student in students)
        {
            student.DroppedOutAt.Should().NotBeNull();
        }

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task DropOutStudents_ShouldBe_StudentNotFoundException_WhenStudentDoesNotExist()
    {
        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([]);

        var command = new DropOutStudentsCommand([Guid.NewGuid(), Guid.NewGuid()], DateTime.Now);

        var handler = new DropOutStudentsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<StudentNotFoundException>();
    }

    [Fact]
    public async Task DropOutStudents_ShouldBe_CallRallback_WhenThrowException()
    {
        var students = _fixture.CreateMany<Student>(4);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet(students);

        _mockDbContext
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Throws(new Exception());

        var command = new DropOutStudentsCommand([.. students.Select(x => x.Id)], DateTime.Now);

        var handler = new DropOutStudentsCommandHandler(_mockDbContext.Object);

        Func<Task> act = async () => await handler.Handle(command, default);

        await act.Should().ThrowAsync<Exception>();

        _mockDbContext.Verify(x => x.RollbackTransactionAsync(), Times.Once());
    }
}
