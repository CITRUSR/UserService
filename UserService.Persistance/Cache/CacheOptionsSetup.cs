using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace UserService.Persistance.Cache;

public class CacheOptionsSetup(IConfiguration configuration) : IConfigureOptions<CacheOptions>
{
    private const string SectionName = "Cache";
    private readonly IConfiguration _configuration = configuration;

    public void Configure(CacheOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}
