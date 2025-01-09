using Mapster;
using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.TeacherEntity.Respones;

namespace UserService.Application.CQRS.TeacherEntity.Queries.GetTeacherById;

public class GetTeacherByIdQueryHandler(IAppDbContext appDbContext)
    : IRequestHandler<GetTeacherByIdQuery, TeacherDto>
{
    private readonly IAppDbContext _appDbContext = appDbContext;

    public async Task<TeacherDto> Handle(
        GetTeacherByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var teacher = await _appDbContext.Teachers.FindAsync(
            new object?[] { request.TeacherId },
            cancellationToken: cancellationToken
        );

        if (teacher == null)
        {
            throw new TeacherNotFoundException(request.TeacherId);
        }

        return teacher.Adapt<TeacherDto>();
    }
}
