using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.Student.Queries.GetStudentBySsoId;

public class GetStudentBySsoIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetStudentBySsoIdQuery, Domain.Entities.Student>
{
    public async Task<Domain.Entities.Student> Handle(GetStudentBySsoIdQuery request,
        CancellationToken cancellationToken)
    {
        var student = await DbContext.Students.FirstOrDefaultAsync(x => x.SsoId == request.SsoId, cancellationToken);

        if (student == null)
        {
            throw new StudentNotFoundException(request.SsoId);
        }

        return student;
    }
}