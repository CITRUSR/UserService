using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentsByGroupId;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudentsByGroupId
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;
    private readonly GetStudentsByGroupIdQueryHandler _handler;

    public GetStudentsByGroupId()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _handler = new GetStudentsByGroupIdQueryHandler(_mockDbContext.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetStudentsByGroupId_ShouldBe_Success()
    {
        var group = _fixture.Create<Group>();

        _fixture.Customize<Student>(x => x.With(x => x.GroupId, group.Id));

        var students = _fixture.CreateMany<Student>(25);

        _fixture.Customize<Student>(x => x.With(x => x.GroupId, 123124124));

        var wrongStudents = _fixture.CreateMany<Student>(25);

        _mockDbContext
            .Setup(x => x.Groups.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([.. students, .. wrongStudents]);

        var query = new GetStudentsByGroupIdQuery(group.Id);

        var result = await _handler.Handle(query, default);

        result.Select(x => x.Id).Should().BeEquivalentTo(students.Select(x => x.Id));
    }

    [Fact]
    public async Task GetStudentsByGroupId_ShouldBe_GroupNotFoundException()
    {
        _mockDbContext
            .Setup(x => x.Groups.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Group?)null);

        var query = new GetStudentsByGroupIdQuery(123123123);

        Func<Task> act = async () => await _handler.Handle(query, default);

        await act.Should().ThrowAsync<GroupNotFoundException>();
    }
}
