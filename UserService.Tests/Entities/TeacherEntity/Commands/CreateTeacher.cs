using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.CQRS.TeacherEntity.Commands.CreateTeacher;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.TeacherEntity.Commands;

public class CreateTeacher
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;

    public CreateTeacher()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CreateSpeciality_ShouldBe_Success()
    {
        _mockDbContext.Setup(x => x.Teachers).ReturnsDbSet([]);

        var command = _fixture.Create<CreateTeacherCommand>();

        var handler = new CreateTeacherCommandHandler(_mockDbContext.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        _mockDbContext.Verify(
            x => x.Teachers.AddAsync(It.IsAny<Teacher>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

        result.Should().NotBeEmpty();
    }
}
