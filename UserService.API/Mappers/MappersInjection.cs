namespace UserService.Persistance.Mappers;

public static class MappersInjection
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<IMapper<Domain.Entities.Student, StudentModel>, StudentMapper>();
        services.AddSingleton<IMapper<Domain.Entities.Group, GroupModel>, GroupMapper>();

        return services;
    }
}