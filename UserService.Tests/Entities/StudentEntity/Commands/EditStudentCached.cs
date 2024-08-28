using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.EditStudent;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class EditStudentCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<EditStudentCommand, Student>> _mockHandler;
    private readonly IFixture _fixture;

    public EditStudentCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<EditStudentCommand, Student>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task EditStudentCached_ShouldBe_Success()
    {
        var student = _fixture.Create<Student>();

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<EditStudentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        var command = _fixture.Create<EditStudentCommand>();

        var handler = new EditStudentCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(x => x.Handle(command, It.IsAny<CancellationToken>()), Times.Once());

        _mockCacheService.Verify(
            x =>
                x.RemoveAsync(
                    It.Is<string>(x => x.Equals(CacheKeys.ById<Student, Guid>(student.Id))),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once()
        );

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
