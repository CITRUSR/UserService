using FluentAssertions;
using Moq;
using Moq.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Application.CQRS.StudentEntity.Responses;
using UserService.Application.Enums;
using UserService.Domain.Entities;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudents
{
    private readonly Mock<IAppDbContext> _mockDbContext;
    private readonly IFixture _fixture;
    private readonly GetStudentsQuery _query;

    public GetStudents()
    {
        _mockDbContext = new Mock<IAppDbContext>();
        _fixture = new Fixture();
        _query = new GetStudentsQuery
        {
            Page = 1,
            PageSize = 10,
            SortState = SortState.LastNameAsc,
            SearchString = "",
            DeletedStatus = DeletedStatus.All,
            DroppedOutStatus = StudentDroppedOutStatus.All,
        };
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithFiltrationByNameAsc()
    {
        await TestFiltration(SortState.FistNameAsc, (studentA, studentB) => [studentA, studentB]);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithFiltrationByNameDesc()
    {
        await TestFiltration(SortState.FirstNameDesc, (studentA, studentB) => [studentB, studentA]);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithFiltrationByLastNameAsc()
    {
        await TestFiltration(SortState.LastNameAsc, (studentA, studentB) => [studentA, studentB]);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithFiltrationByLastNameDesc()
    {
        await TestFiltration(SortState.LastNameDesc, (studentA, studentB) => [studentB, studentA]);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithFiltrationByGroupAsc()
    {
        await TestFiltration(SortState.GroupAsc, (studentA, studentB) => [studentA, studentB]);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithFiltrationByGroupDesc()
    {
        await TestFiltration(SortState.GroupDesc, (studentA, studentB) => [studentB, studentA]);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDroppedOutStatus_All()
    {
        await TestDroppedOutStatus(
            StudentDroppedOutStatus.All,
            (StudentA, StudentB) => [StudentA, StudentB]
        );
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDroppedOutStatus_OnlyDroppedOut()
    {
        await TestDroppedOutStatus(
            StudentDroppedOutStatus.OnlyDroppedOut,
            (studentA, studentB) => [studentA]
        );
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDroppedOutStatus_OnlyActive()
    {
        await TestDroppedOutStatus(
            StudentDroppedOutStatus.OnlyActive,
            (studentA, studentB) => [studentB]
        );
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDeletedStatus_OnlyActive()
    {
        await TestDeletedStatus(DeletedStatus.OnlyActive, (studentA, studentB) => [studentB]);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDeletedStatus_OnlyDeleted()
    {
        await TestDeletedStatus(DeletedStatus.OnlyDeleted, (studentA, studentB) => [studentA]);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDeletedStatus_All()
    {
        await TestDeletedStatus(DeletedStatus.All, (studentA, studentB) => [studentA, studentB]);
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithSearchStringWithFirstName()
    {
        await TestSearchString("AA", 1, student => student.FirstName == "AAA");
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithSearchStringWithLastName()
    {
        await TestSearchString("BB", 1, student => student.LastName == "BBB");
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithSearchStringWithPatronymicName()
    {
        await TestSearchString("GG", 1, student => student.PatronymicName == "GGG");
    }

    [Fact]
    public async Task GetSpecialities_ShouldBe_SuccessWithSearchStringWithGroup()
    {
        await TestSearchString(
            "2-H",
            2,
            student => student.GroupName.Contains('H') && student.GroupName.Contains('2')
        );
    }

    private async Task TestFiltration(
        SortState sortState,
        Func<Student, Student, Student[]> expectedOrder
    )
    {
        var specialityA = _fixture.Build<Speciality>().With(x => x.Abbreviation, "AAA").Create();
        var specialityB = _fixture.Build<Speciality>().With(x => x.Abbreviation, "BBB").Create();

        var groupA = _fixture
            .Build<Group>()
            .With(x => x.Speciality, specialityA)
            .With(x => x.CurrentCourse, 1)
            .With(x => x.SubGroup, 1)
            .Create();

        var groupB = _fixture
            .Build<Group>()
            .With(x => x.Speciality, specialityB)
            .With(x => x.CurrentCourse, 2)
            .With(x => x.SubGroup, 2)
            .Create();

        var studentA = _fixture
            .Build<Student>()
            .With(x => x.FirstName, "AAA")
            .With(x => x.LastName, "AAA")
            .With(x => x.Group, groupA)
            .Create();

        var studentB = _fixture
            .Build<Student>()
            .With(x => x.FirstName, "BBB")
            .With(x => x.LastName, "BBB")
            .With(x => x.Group, groupB)
            .Create();

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([studentA, studentB]);

        var handler = new GetStudentsQueryHandler(_mockDbContext.Object);

        var query = _query with { SortState = sortState };

        var result = await handler.Handle(query, default);

        result
            .Students.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(
                expectedOrder(studentA, studentB).Select(x => x.Id),
                options => options.WithStrictOrdering()
            );
    }

    private async Task TestDroppedOutStatus(
        StudentDroppedOutStatus status,
        Func<Student, Student, Student[]> expectedStudent
    )
    {
        var studentA = _fixture.Build<Student>().With(x => x.DroppedOutAt, DateTime.Now).Create();
        var studentB = _fixture.Build<Student>().Without(x => x.DroppedOutAt).Create();

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([studentA, studentB]);

        var handler = new GetStudentsQueryHandler(_mockDbContext.Object);

        var query = _query with { DroppedOutStatus = status };

        var result = await handler.Handle(query, default);

        result
            .Students.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(expectedStudent(studentA, studentB).Select(x => x.Id));
    }

    private async Task TestDeletedStatus(
        DeletedStatus status,
        Func<Student, Student, Student[]> expectedStudent
    )
    {
        var studentA = _fixture.Build<Student>().With(x => x.IsDeleted, true).Create();
        var studentB = _fixture.Build<Student>().With(x => x.IsDeleted, false).Create();

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([studentA, studentB]);

        var handler = new GetStudentsQueryHandler(_mockDbContext.Object);

        var query = _query with { DeletedStatus = status };

        var result = await handler.Handle(query, default);

        result
            .Students.Select(x => x.Id)
            .Should()
            .BeEquivalentTo(expectedStudent(studentA, studentB).Select(x => x.Id));
    }

    private async Task TestSearchString(
        string searchString,
        int expectedCount,
        Func<StudentViewModel, bool> predicate
    )
    {
        var speciality = _fixture.Build<Speciality>().With(x => x.Abbreviation, "H").Create();

        var group = _fixture
            .Build<Group>()
            .With(x => x.Speciality, speciality)
            .With(x => x.CurrentCourse, 2)
            .Create();

        var studentA = _fixture
            .Build<Student>()
            .With(x => x.FirstName, "AAA")
            .With(x => x.LastName, "YYY")
            .With(x => x.PatronymicName, "GGG")
            .With(x => x.Group, group)
            .Create();

        var studentB = _fixture
            .Build<Student>()
            .With(x => x.FirstName, "PPP")
            .With(x => x.LastName, "BBB")
            .With(x => x.PatronymicName, "LLL")
            .With(x => x.Group, group)
            .Create();

        _mockDbContext.Setup(x => x.Students).ReturnsDbSet([studentA, studentB]);

        var query = _query with { SearchString = searchString };

        var handler = new GetStudentsQueryHandler(_mockDbContext.Object);

        var result = await handler.Handle(query, default);

        result.Students.Count.Should().Be(expectedCount);
        result.Students.Should().Contain(student => predicate(student));
    }
}
