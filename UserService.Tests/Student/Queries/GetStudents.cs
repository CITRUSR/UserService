using FluentAssertions;
using UserService.Application.Common.Paging;
using UserService.Application.CQRS.Student.Queries.GetStudents;
using UserService.Tests.Common;

namespace UserService.Tests.Student.Queries;

public class GetStudents : CommonTest
{
    private readonly struct CreateStudentOption(string firstName, string lastName)
    {
        public string FirstName { get; } = firstName;
        public string LastName { get; } = lastName;
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithPageSize()
    {
        ClearDataBase();

        var students = Fixture.CreateMany<Domain.Entities.Student>(12);

        await Context.AddRangeAsync(students);
        await Context.SaveChangesAsync();

        var query = new GetStudentsQuery
        {
            SortState = SortState.FistNameAsc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var studentsRes = await Action(query);

        studentsRes.Items.Should().HaveCount(10);
        studentsRes.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithPageNumber()
    {
        ClearDataBase();

        var students = Fixture.CreateMany<Domain.Entities.Student>(12);

        await Context.AddRangeAsync(students);
        await Context.SaveChangesAsync();

        var query = new GetStudentsQuery
        {
            SortState = SortState.FistNameAsc,
            Page = 2,
            PageSize = 10,
            SearchString = "",
        };

        var studentsRes = await Action(query);

        studentsRes.Items.Should().HaveCount(2);
        studentsRes.MaxPage.Should().Be(2);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByNameAsc()
    {
        var (studentA, studentB) =
            await Arrange(new CreateStudentOption("Abas", "Asb"), new CreateStudentOption("Bbs", "bbs"));

        var query = new GetStudentsQuery()
        {
            SortState = SortState.FistNameAsc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var students = await Action(query);

        students.Items[0].Should().BeEquivalentTo(studentA);
        students.Items[1].Should().BeEquivalentTo(studentB);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByNameDesc()
    {
        var (studentA, studentB) =
            await Arrange(new CreateStudentOption("Abas", "Asb"), new CreateStudentOption("Bbs", "bbs"));

        var query = new GetStudentsQuery()
        {
            SortState = SortState.FirstNameDesc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var students = await Action(query);

        students.Items[0].Should().BeEquivalentTo(studentB);
        students.Items[1].Should().BeEquivalentTo(studentA);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByLastNameAsc()
    {
        var (studentA, studentB) =
            await Arrange(new CreateStudentOption("Abas", "Asb"), new CreateStudentOption("Bbs", "bbs"));

        var query = new GetStudentsQuery()
        {
            SortState = SortState.LastNameAsc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var students = await Action(query);

        students.Items[0].Should().BeEquivalentTo(studentA);
        students.Items[1].Should().BeEquivalentTo(studentB);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByLastNameDesc()
    {
        var (studentA, studentB) =
            await Arrange(new CreateStudentOption("Abas", "Asb"), new CreateStudentOption("Bbs", "bbs"));

        var query = new GetStudentsQuery()
        {
            SortState = SortState.LastNameDesc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var students = await Action(query);

        students.Items[0].Should().BeEquivalentTo(studentB);
        students.Items[1].Should().BeEquivalentTo(studentA);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByGroupAsc()
    {
        ArrangeForGroupTests();

        var studentExc = Context.Students.OrderBy(s => s.Group.CurrentSemester)
            .ThenBy(s => s.Group.Speciality.Abbreavation).ThenBy(s => s.Group.SubGroup);

        var query = new GetStudentsQuery()
        {
            SortState = SortState.GroupAsc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var students = await Action(query);

        students.Items.Should().BeEquivalentTo(studentExc, options => options.WithStrictOrdering());
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByGroupDesc()
    {
        ArrangeForGroupTests();

        var studentExc = Context.Students.OrderByDescending(s => s.Group.CurrentSemester)
            .ThenBy(s => s.Group.Speciality.Abbreavation).ThenBy(s => s.Group.SubGroup);

        var query = new GetStudentsQuery()
        {
            SortState = SortState.GroupDesc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var students = await Action(query);

        students.Items.Should().BeEquivalentTo(studentExc, options => options.WithStrictOrdering());
    }

    private async Task<(Domain.Entities.Student studentA, Domain.Entities.Student studentB)> Arrange(
        CreateStudentOption studentOptionA, CreateStudentOption studentOptionB)
    {
        ClearDataBase();

        var studentA = Fixture.Build<Domain.Entities.Student>()
            .With(x => x.FirstName, studentOptionA.FirstName)
            .With(x => x.LastName, studentOptionA.LastName)
            .Create();

        var studentB = Fixture.Build<Domain.Entities.Student>()
            .With(x => x.FirstName, studentOptionB.FirstName)
            .With(x => x.LastName, studentOptionB.LastName)
            .Create();

        await Context.AddAsync(studentA);
        await Context.AddAsync(studentB);

        await Context.SaveChangesAsync();

        return (studentA, studentB);
    }

    private async void ArrangeForGroupTests()
    {
        ClearDataBase();

        var studentA = Fixture.Create<Domain.Entities.Student>();
        var studentB = Fixture.Create<Domain.Entities.Student>();

        await Context.AddAsync(studentA);
        await Context.AddAsync(studentB);

        await Context.SaveChangesAsync();
    }

    private async Task<PaginationList<Domain.Entities.Student>> Action(GetStudentsQuery query)
    {
        var handler = new GetStudentsQueryHandler(Context);

        return await handler.Handle(query, CancellationToken.None);
    }
}