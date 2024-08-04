using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Domain.Entities;
using UserService.Persistance;

namespace UserService.Tests.Common;

public class CommonTest : IAsyncLifetime, IClassFixture<DatabaseFixture>
{
    protected readonly IAppDbContext Context;
    protected readonly Fixture Fixture;
    private readonly DatabaseFixture _databaseFixture;

    public CommonTest(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
        Context = CreateDbContext();
        Fixture = new Fixture();
    }

    public async Task InitializeAsync()
    {
        await Context.BeginTransactionAsync();
    }

    public async Task DisposeAsync()
    {
        await Context.RollbackTransactionAsync();
    }

    protected async Task AddGroupsToContext(params Group[] groups)
    {
        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync(CancellationToken.None);
    }

    protected async Task AddTeachersToContext(params Teacher[] teachers)
    {
        await Context.Teachers.AddRangeAsync(teachers);
        await Context.SaveChangesAsync(CancellationToken.None);
    }

    protected async Task AddSpecialitiesToContext(params Speciality[] specialities)
    {
        await Context.Specialities.AddRangeAsync(specialities);
        await Context.SaveChangesAsync(CancellationToken.None);
    }

    protected async Task AddStudentsToContext(params Student[] students)
    {
        await Context.Students.AddRangeAsync(students);
        await Context.SaveChangesAsync(CancellationToken.None);
    }

    private IAppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_databaseFixture.ConnectionString)
            .Options;

        var db = new AppDbContext(options);

        db.Database.EnsureCreated();

        return db;
    }
}
