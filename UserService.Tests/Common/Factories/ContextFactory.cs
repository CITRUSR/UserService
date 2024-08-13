using Microsoft.EntityFrameworkCore;
using UserService.Persistance;

namespace UserService.Tests.Common;

public class ContextFactory
{
    public static AppDbContext Create(string connectionString)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        var db = new AppDbContext(options);

        db.Database.EnsureCreated();

        return db;
    }
}
