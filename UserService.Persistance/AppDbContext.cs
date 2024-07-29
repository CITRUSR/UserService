using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Domain.Entities;
using UserService.Persistance.Configurations;

namespace UserService.Persistance;

public class AppDbContext : DbContext, IAppDbContext
{
    public DbSet<Student> Students { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Speciality> Specialities { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new TeacherConfiguration());
        modelBuilder.ApplyConfiguration(new GroupConfiguration());
        modelBuilder.ApplyConfiguration(new SpecialityConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public async Task BeginTransactionAsync()
    {
        if (Database.CurrentTransaction == null)
        {
            await Database.BeginTransactionAsync();
        }
    }

    public async Task CommitTransactionAsync()
    {
        var transaction = Database.CurrentTransaction;
        if (transaction != null)
        {
            await transaction.CommitAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        var transaction = Database.CurrentTransaction;
        if (transaction != null)
        {
            await transaction.RollbackAsync();
        }
    }
}