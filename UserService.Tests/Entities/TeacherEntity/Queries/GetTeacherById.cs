using FluentAssertions;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.TeacherEntity.Queries.GetTeacherById;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.TeacherEntity.Queries;

public class GetTeacherById
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly Fixture _fixture;
    private readonly GetTeacherByIdQuery _query;
    private readonly GetTeacherByIdQueryHandler _handler;

    public GetTeacherById()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _query = _fixture.Create<GetTeacherByIdQuery>();
        _handler = new GetTeacherByIdQueryHandler(_mockDbContext.Object);
    }

    [Fact]
    public async Task GetTeacherById_ShouldBe_Success()
    {
        var teacher = _fixture.Create<Teacher>();

        _mockDbContext
            .Setup(x => x.Teachers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(teacher);

        var result = await _handler.Handle(_query, default);

        _mockDbContext.Verify(
            x => x.Teachers.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );

        result.Id.Should().Be(teacher.Id);
    }

    [Fact]
    public async Task GetTeacherById_ShouldBe_TeacherNotFoundException()
    {
        _mockDbContext
            .Setup(x => x.Teachers.FindAsync(It.IsAny<object[]>()))
            .ReturnsAsync((Teacher?)null);

        Func<Task> act = async () => await _handler.Handle(_query, CancellationToken.None);

        await act.Should().ThrowAsync<TeacherNotFoundException>();
    }
}
