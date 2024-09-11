using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UserService.Application.Abstraction;
using UserService.Persistance.Cache;

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

        services.ConfigureOptions<CacheOptionsSetup>();

        services.AddSingleton<ICacheOptions>(sp =>
            sp.GetRequiredService<IOptions<CacheOptions>>().Value
        );

        services.AddSingleton<ICacheService, CacheService>();

        return services;
    }
}
