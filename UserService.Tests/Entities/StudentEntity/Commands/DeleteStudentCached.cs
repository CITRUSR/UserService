using FluentAssertions;
using MediatR;
using Moq;
using UserService.Application.Abstraction;
using UserService.Application.Common.Cache;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudent;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Commands;

public class DeleteStudentCached
{
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly Mock<IRequestHandler<DeleteStudentCommand, Guid>> _mockHandler;
    private readonly IFixture _fixture;

    public DeleteStudentCached()
    {
        _mockCacheService = new Mock<ICacheService>();
        _mockHandler = new Mock<IRequestHandler<DeleteStudentCommand, Guid>>();
        _fixture = new Fixture();
    }

    [Fact]
    public async Task DeleteStudentCached_ShouldBe_Success()
    {
        var student = _fixture.Create<Student>();

        _mockHandler
            .Setup(x => x.Handle(It.IsAny<DeleteStudentCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(student.Id);

        var command = new DeleteStudentCommand(student.Id);

        var handler = new DeleteStudentCommandHandlerCached(
            _mockCacheService.Object,
            _mockHandler.Object
        );

        var result = await handler.Handle(command, default);

        _mockHandler.Verify(x => x.Handle(command, default), Times.Once());

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

        result.Should().NotBeEmpty();
    }
}
