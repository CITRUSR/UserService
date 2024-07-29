using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Domain.Entities;
using UserService.Tests.Common;

namespace UserService.Tests.Entities.StudentEntity.Queries;

public class GetStudents(DatabaseFixture databaseFixture) : CommonTest(databaseFixture)
{
    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithPageSize()
    {
        var students = Fixture.CreateMany<Student>(12);

        await AddStudentsToContext(students.ToArray());

        var query = CreateQuery();

        var studentsRes = await Action(query);

        studentsRes.Items.Should().HaveCount(10);
        studentsRes.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithPageNumber()
    {
        var students = Fixture.CreateMany<Student>(12);

        await AddStudentsToContext(students.ToArray());

        var query = CreateQuery(page: 2);

        var studentsRes = await Action(query);

        studentsRes.Items.Should().HaveCount(2);
        studentsRes.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByNameAsc()
    {
        var (studentA, studentB) = await SeedDataForTests();

        var query = CreateQuery();

        var students = await Action(query);

        students.Items.Should().BeEquivalentTo([studentA, studentB]);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByNameDesc()
    {
        var (studentA, studentB) = await SeedDataForTests();

        var query = CreateQuery(sortState: SortState.FirstNameDesc);

        var students = await Action(query);

        students.Items.Should().BeEquivalentTo([studentB, studentA]);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByLastNameAsc()
    {
        var (studentA, studentB) = await SeedDataForTests();

        var query = CreateQuery(sortState: SortState.FistNameAsc);

        var students = await Action(query);

        students.Items.Should().BeEquivalentTo([studentA, studentB]);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByLastNameDesc()
    {
        var (studentA, studentB) = await SeedDataForTests();

        var query = CreateQuery(sortState: SortState.LastNameDesc);

        var students = await Action(query);

        students.Items.Should().BeEquivalentTo([studentB, studentA]);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByGroupAsc()
    {
        await SeedDataForTests();

        var studentExc = Context.Students.OrderBy(s => s.Group.CurrentSemester)
            .ThenBy(s => s.Group.Speciality.Abbreavation).ThenBy(s => s.Group.SubGroup);

        var query = CreateQuery(sortState: SortState.GroupAsc);

        var students = await Action(query);

        students.Items.Should().BeEquivalentTo(studentExc, options => options.WithStrictOrdering());
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByGroupDesc()
    {
        await SeedDataForTests();

        var studentExc = Context.Students.OrderByDescending(s => s.Group.CurrentSemester)
            .ThenBy(s => s.Group.Speciality.Abbreavation).ThenBy(s => s.Group.SubGroup);

        var query = CreateQuery(sortState: SortState.GroupDesc);

        var students = await Action(query);

        students.Items.Should().BeEquivalentTo(studentExc, options => options.WithStrictOrdering());
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithDroppedOutStatus_All()
    {
        var (specialityA, specialityB) = await SeedDataForTests();

        var query = CreateQuery();

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(2);
        specialities.Items.Should().BeEquivalentTo([specialityA, specialityB]);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithDroppedOutStatus_OnlyDroppedOut()
    {
        var (specialityA, specialityB) = await SeedDataForTests();

        var query = CreateQuery(droppedOutStatus: StudentDroppedOutStatus.OnlyDroppedOut);

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(1);
        specialities.Items[0].Should().BeEquivalentTo(specialityA);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithDroppedOutStatus_OnlyActive()
    {
        var (specialityA, specialityB) = await SeedDataForTests();

        var query = CreateQuery(droppedOutStatus: StudentDroppedOutStatus.OnlyActive);

        var specialities = await Action(query);

        specialities.Items.Should().HaveCount(1);
        specialities.Items[0].Should().BeEquivalentTo(specialityB);
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithSearchStringWithFirstName()
    {
        await TestSearchString("AA", 1, student => student.FirstName == "AAA");
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithSearchStringWithLastName()
    {
        await TestSearchString("BB", 1, student => student.LastName == "BBB");
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithSearchStringWithPatronymicName()
    {
        await TestSearchString("GG", 1, student => student.PatronymicName == "GGG");
    }

    [Fact]
    public async void GetSpecialities_ShouldBe_SuccessWithSearchStringWithGroup()
    {
        await TestSearchString("2-H", 2, student => student.Group.Speciality.Abbreavation == "H" &&
                                                    student.Group.CurrentCourse == 2);
    }

    private async Task TestSearchString(string searchString, int expectedCount, Func<Student, bool> predicate)
    {
        await SeedDataForSearchStringTests();

        var query = CreateQuery(searchString: searchString);

        var students = await Action(query);

        students.Items.Should().HaveCount(expectedCount);
        students.Items.Should().Contain(student => predicate(student));
    }

    private async Task SeedDataForSearchStringTests()
    {
        var speciality = Fixture.Build<Speciality>()
            .With(x => x.Abbreavation, "H")
            .Create();

        var group1 = Fixture.Build<Group>()
            .With(x => x.Speciality, speciality)
            .With(x => x.CurrentCourse, 2)
            .Create();

        var studentA = Fixture.Build<Student>()
            .With(x => x.FirstName, "AAA")
            .With(x => x.LastName, "BBB")
            .With(x => x.Group, group1)
            .Create();
        var studentB = Fixture.Build<Student>()
            .With(x => x.FirstName, "CCC")
            .With(x => x.LastName, "DDD")
            .With(x => x.PatronymicName, "GGG")
            .With(x => x.Group, group1)
            .Create();

        await AddStudentsToContext([studentA, studentB]);
    }

    private async Task<(Student, Student)> SeedDataForTests()
    {
        var studentA = Fixture.Build<Student>()
            .With(x => x.FirstName, "AAA")
            .With(x => x.LastName, "AAA")
            .With(x => x.DroppedOutAt, DateTime.Now)
            .Create();
        var studentB = Fixture.Build<Student>()
            .With(x => x.FirstName, "BBB")
            .With(x => x.LastName, "BBB")
            .Without(x => x.DroppedOutAt)
            .Create();

        await AddStudentsToContext([studentA, studentB]);

        return (studentA, studentB);
    }

    private GetStudentsQuery CreateQuery(int page = 1, int pageSize = 10, string searchString = "",
        SortState sortState = SortState.LastNameAsc,
        StudentDroppedOutStatus droppedOutStatus = StudentDroppedOutStatus.All)
    {
        return new GetStudentsQuery
        {
            Page = page,
            PageSize = pageSize,
            SearchString = searchString,
            SortState = sortState,
            DroppedOutStatus = droppedOutStatus,
        };
    }

    private async Task<PaginationList<Student>> Action(GetStudentsQuery query)
    {
        var handler = new GetStudentsQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}