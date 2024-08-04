using Microsoft.EntityFrameworkCore;
using UserService.Persistance;

namespace UserService.Tests.Common;

public class ContextFactory
{
    public static AppDbContext Create()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AppDbContext(options);

        return context;
    }

    public static void Destroy(AppDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }
}
