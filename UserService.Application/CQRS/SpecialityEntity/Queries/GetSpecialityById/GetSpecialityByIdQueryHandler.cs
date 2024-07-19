using MediatR;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;

public class GetSpecialityByIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetSpecialityByIdQuery, Speciality>
{
    public async Task<Speciality> Handle(GetSpecialityByIdQuery request, CancellationToken cancellationToken)
    {
        var speciality = await DbContext.Specialities.FindAsync(new object?[] { request.Id, cancellationToken },
            cancellationToken: cancellationToken);

        if (speciality == null)
        {
            throw new SpecialityNotFoundException(request.Id);
        }

        return speciality;
    }
}