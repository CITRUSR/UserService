using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserService.Application.Abstraction;

namespace UserService.Persistance;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IAppDbContext>(provider => provider.GetService<AppDbContext>());

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }
}
