using UserService.Application.Abstraction;
using UserService.Domain.Entities;

namespace UserService.Tests.Common;

public class DbContextHelper(IAppDbContext context)
{
    private readonly IAppDbContext Context = context;

    public async Task AddGroupsToContext(params Group[] groups)
    {
        await Context.Groups.AddRangeAsync(groups);
        await Context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task AddTeachersToContext(params Teacher[] teachers)
    {
        await Context.Teachers.AddRangeAsync(teachers);
        await Context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task AddSpecialitiesToContext(params Speciality[] specialities)
    {
        await Context.Specialities.AddRangeAsync(specialities);
        await Context.SaveChangesAsync(CancellationToken.None);
    }

    public async Task AddStudentsToContext(params Student[] students)
    {
        await Context.Students.AddRangeAsync(students);
        await Context.SaveChangesAsync(CancellationToken.None);
    }
}
