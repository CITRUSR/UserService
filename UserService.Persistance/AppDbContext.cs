using Microsoft.EntityFrameworkCore;
using UserService.Application;
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
}