using FluentAssertions;
using UserService.Application.CQRS.Student.Queries.GetStudents;
using UserService.Tests.Common;

namespace UserService.Tests.Student.Queries;

public class GetStudents : CommonTest
{
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

        var handler = new GetStudentsQueryHandler(Context);

        var studentsRes = await handler.Handle(query, CancellationToken.None);

        studentsRes.Should().HaveCount(10);
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

        var handler = new GetStudentsQueryHandler(Context);

        var studentsRes = await handler.Handle(query, CancellationToken.None);

        studentsRes.Should().HaveCount(2);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByNameAsc()
    {
        ClearDataBase();

        var studentA = Fixture.Build<Domain.Entities.Student>().With(x => x.FirstName, "Avgeei").Create();
        var studentB = Fixture.Build<Domain.Entities.Student>().With(x => x.FirstName, "Cdei").Create();

        await Context.AddAsync(studentA);
        await Context.AddAsync(studentB);

        await Context.SaveChangesAsync();

        var query = new GetStudentsQuery()
        {
            SortState = SortState.FistNameAsc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var handler = new GetStudentsQueryHandler(Context);

        var students = await handler.Handle(query, CancellationToken.None);

        students[0].Should().BeEquivalentTo(studentA);
        students[1].Should().BeEquivalentTo(studentB);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByNameDesc()
    {
        ClearDataBase();

        var studentA = Fixture.Build<Domain.Entities.Student>().With(x => x.FirstName, "Avgeei").Create();
        var studentB = Fixture.Build<Domain.Entities.Student>().With(x => x.FirstName, "Cdei").Create();

        await Context.AddAsync(studentA);
        await Context.AddAsync(studentB);

        await Context.SaveChangesAsync();

        var query = new GetStudentsQuery()
        {
            SortState = SortState.FirstNameDesc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var handler = new GetStudentsQueryHandler(Context);

        var students = await handler.Handle(query, CancellationToken.None);

        students[0].Should().BeEquivalentTo(studentB);
        students[1].Should().BeEquivalentTo(studentA);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByLastNameAsc()
    {
        ClearDataBase();

        var studentA = Fixture.Build<Domain.Entities.Student>().With(x => x.LastName, "Avgeei").Create();
        var studentB = Fixture.Build<Domain.Entities.Student>().With(x => x.LastName, "Cdei").Create();

        await Context.AddAsync(studentA);
        await Context.AddAsync(studentB);

        await Context.SaveChangesAsync();

        var query = new GetStudentsQuery()
        {
            SortState = SortState.LastNameAsc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var handler = new GetStudentsQueryHandler(Context);

        var students = await handler.Handle(query, CancellationToken.None);

        students[0].Should().BeEquivalentTo(studentA);
        students[1].Should().BeEquivalentTo(studentB);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByLastNameDesc()
    {
        ClearDataBase();

        var studentA = Fixture.Build<Domain.Entities.Student>().With(x => x.LastName, "Avgeei").Create();
        var studentB = Fixture.Build<Domain.Entities.Student>().With(x => x.LastName, "Cdei").Create();

        await Context.AddAsync(studentA);
        await Context.AddAsync(studentB);

        await Context.SaveChangesAsync();

        var query = new GetStudentsQuery()
        {
            SortState = SortState.LastNameDesc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var handler = new GetStudentsQueryHandler(Context);

        var students = await handler.Handle(query, CancellationToken.None);

        students[0].Should().BeEquivalentTo(studentB);
        students[1].Should().BeEquivalentTo(studentA);
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByGroupAsc()
    {
        ClearDataBase();

        var studentA = Fixture.Create<Domain.Entities.Student>();
        var studentB = Fixture.Create<Domain.Entities.Student>();

        await Context.AddAsync(studentA);
        await Context.AddAsync(studentB);

        await Context.SaveChangesAsync();

        var studentExc = Context.Students.OrderBy(s => s.Group.CurrentSemester)
            .ThenBy(s => s.Group.Speciality.Abbreavation).ThenBy(s => s.Group.SubGroup);

        var query = new GetStudentsQuery()
        {
            SortState = SortState.GroupAsc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var handler = new GetStudentsQueryHandler(Context);

        var students = await handler.Handle(query, CancellationToken.None);

        students.Should().BeEquivalentTo(studentExc, options => options.WithStrictOrdering());
    }

    [Fact]
    public async void GetStudents_ShouldBe_SuccessWithFiltrationByGroupDesc()
    {
        ClearDataBase();

        var studentA = Fixture.Create<Domain.Entities.Student>();
        var studentB = Fixture.Create<Domain.Entities.Student>();

        await Context.AddAsync(studentA);
        await Context.AddAsync(studentB);

        await Context.SaveChangesAsync();

        var studentExc = Context.Students.OrderByDescending(s => s.Group.CurrentSemester)
            .ThenBy(s => s.Group.Speciality.Abbreavation).ThenBy(s => s.Group.SubGroup);

        var query = new GetStudentsQuery()
        {
            SortState = SortState.GroupDesc,
            Page = 1,
            PageSize = 10,
            SearchString = "",
        };

        var handler = new GetStudentsQueryHandler(Context);

        var students = await handler.Handle(query, CancellationToken.None);

        students.Should().BeEquivalentTo(studentExc, options => options.WithStrictOrdering());
    }
}