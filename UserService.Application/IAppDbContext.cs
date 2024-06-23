using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Application;

public interface IAppDbContext
{
    public DbSet<Domain.Entities.Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    public Task<int> SaveChangesAsync(CancellationToken token);
}