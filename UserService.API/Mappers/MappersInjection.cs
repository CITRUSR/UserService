using UserService.Domain.Entities;

namespace UserService.API.Mappers;

public static class MappersInjection
{
    public static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.AddSingleton<IMapper<Student, StudentModel>, StudentMapper>();
        services.AddSingleton<IMapper<Group, GroupModel>, GroupMapper>();
        services.AddSingleton<IMapper<Speciality, SpecialityModel>, SpecialityMapper>();

        return services;
    }
}