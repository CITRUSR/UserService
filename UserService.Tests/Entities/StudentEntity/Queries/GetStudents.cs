using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Application.Enums;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudents(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithPageSize()
    {
        await TestPagination(pageSize: 10, expectedCount: 10, expectedMaxPage: 2);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithPageNumber()
    {
        await TestPagination(page: 2, expectedCount: 2, expectedMaxPage: 2);
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
        await TestGroupFiltration(SortState.GroupAsc, true);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithFiltrationByGroupDesc()
    {
        await TestGroupFiltration(SortState.GroupDesc, false);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDroppedOutStatus_All()
    {
        await TestDroppedOutStatus(StudentDroppedOutStatus.All, 2);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDroppedOutStatus_OnlyDroppedOut()
    {
        await TestDroppedOutStatus(
            StudentDroppedOutStatus.OnlyDroppedOut,
            1,
            (studentA, studentB) => studentA
        );
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDroppedOutStatus_OnlyActive()
    {
        await TestDroppedOutStatus(
            StudentDroppedOutStatus.OnlyActive,
            1,
            (studentA, studentB) => studentB
        );
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDeletedStatus_OnlyActive()
    {
        await TestDeletedStatus(DeletedStatus.OnlyActive, 1, (studentA, studentB) => studentB);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDeletedStatus_OnlyDeleted()
    {
        await TestDeletedStatus(DeletedStatus.OnlyDeleted, 1, (studentA, studentB) => studentA);
    }

    [Fact]
    public async Task GetStudents_ShouldBe_SuccessWithDeletedStatus_All()
    {
        await TestDeletedStatus(DeletedStatus.All, 2);
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
            student =>
                student.Group.Speciality.Abbreavation == "H" && student.Group.CurrentCourse == 2
        );
    }

    private async Task TestPagination(
        int pageSize = 10,
        int page = 1,
        int expectedCount = 10,
        int expectedMaxPage = 1
    )
    {
        var students = Fixture.CreateMany<Student>(12);
        await AddStudentsToContext(students.ToArray());

        var query = CreateQuery(page: page, pageSize: pageSize);
        var studentsRes = await Action(query);

        studentsRes.Items.Should().HaveCount(expectedCount);
        studentsRes.MaxPage.Should().Be(expectedMaxPage);
    }

    private async Task TestFiltration(
        SortState sortState,
        Func<Student, Student, Student[]> expectedOrder
    )
    {
        var (studentA, studentB) = await SeedDataForTests();

        var query = CreateQuery(sortState: sortState);
        var students = await Action(query);

        students.Items.Should().BeEquivalentTo(expectedOrder(studentA, studentB));
    }

    private async Task TestGroupFiltration(SortState sortState, bool isAsc)
    {
        await SeedDataForTests();

        var studentExc = isAsc
            ? Context
                .Students.OrderBy(s => s.Group.CurrentSemester)
                .ThenBy(s => s.Group.Speciality.Abbreavation)
                .ThenBy(s => s.Group.SubGroup)
            : Context
                .Students.OrderByDescending(s => s.Group.CurrentSemester)
                .ThenBy(s => s.Group.Speciality.Abbreavation)
                .ThenBy(s => s.Group.SubGroup);

        var query = CreateQuery(sortState: sortState);
        var students = await Action(query);

        students.Items.Should().BeEquivalentTo(studentExc, options => options.WithStrictOrdering());
    }

    private async Task TestDroppedOutStatus(
        StudentDroppedOutStatus status,
        int expectedCount,
        Func<Student, Student, Student> expectedStudent = null
    )
    {
        var (specialityA, specialityB) = await SeedDataForTests();

        var query = CreateQuery(droppedOutStatus: status);
        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(expectedCount);

        if (expectedStudent != null)
        {
            specialities
                .Items[0]
                .Should()
                .BeEquivalentTo(expectedStudent(specialityA, specialityB));
        }
    }

    private async Task TestDeletedStatus(
        DeletedStatus status,
        int expectedCount,
        Func<Student, Student, Student> expectedStudent = null
    )
    {
        var (specialityA, specialityB) = await SeedDataForTests();

        var query = CreateQuery(deletedStatus: status);
        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(expectedCount);

        if (expectedStudent != null)
        {
            specialities
                .Items[0]
                .Should()
                .BeEquivalentTo(expectedStudent(specialityA, specialityB));
        }
    }

    private async Task TestSearchString(
        string searchString,
        int expectedCount,
        Func<Student, bool> predicate
    )
    {
        await SeedDataForSearchStringTests();

        var query = CreateQuery(searchString: searchString);
        var students = await Action(query);

        students.Items.Should().HaveCount(expectedCount);
        students.Items.Should().Contain(student => predicate(student));
    }

    private async Task SeedDataForSearchStringTests()
    {
        var speciality = Fixture.Build<Speciality>().With(x => x.Abbreavation, "H").Create();

        var group1 = Fixture
            .Build<Group>()
            .With(x => x.Speciality, speciality)
            .With(x => x.CurrentCourse, 2)
            .Create();

        var studentA = Fixture
            .Build<Student>()
            .With(x => x.FirstName, "AAA")
            .With(x => x.LastName, "BBB")
            .With(x => x.Group, group1)
            .Create();

        var studentB = Fixture
            .Build<Student>()
            .With(x => x.FirstName, "CCC")
            .With(x => x.LastName, "DDD")
            .With(x => x.PatronymicName, "GGG")
            .With(x => x.Group, group1)
            .Create();

        await AddStudentsToContext(new[] { studentA, studentB });
    }

    private async Task<(Student, Student)> SeedDataForTests()
    {
        var studentA = Fixture
            .Build<Student>()
            .With(x => x.FirstName, "AAA")
            .With(x => x.LastName, "AAA")
            .With(x => x.DroppedOutAt, DateTime.Now)
            .With(x => x.IsDeleted, true)
            .Create();

        var studentB = Fixture
            .Build<Student>()
            .With(x => x.FirstName, "BBB")
            .With(x => x.LastName, "BBB")
            .Without(x => x.DroppedOutAt)
            .Without(x => x.IsDeleted)
            .Create();

        await AddStudentsToContext(new[] { studentA, studentB });

        return (studentA, studentB);
    }

    private GetStudentsQuery CreateQuery(
        int page = 1,
        int pageSize = 10,
        string searchString = "",
        SortState sortState = SortState.LastNameAsc,
        StudentDroppedOutStatus droppedOutStatus = StudentDroppedOutStatus.All,
        DeletedStatus deletedStatus = DeletedStatus.All
    )
    {
        return new GetStudentsQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchString = searchString,
            SortState = sortState,
            DroppedOutStatus = droppedOutStatus,
            DeletedStatus = deletedStatus,
        };
    }

    private async Task<PaginationList<Student>> Action(GetStudentsQuery query)
    {
        var handler = new GetStudentsQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}
