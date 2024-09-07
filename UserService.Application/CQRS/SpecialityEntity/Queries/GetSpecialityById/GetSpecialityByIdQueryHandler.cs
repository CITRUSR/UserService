using Mapster;
using MediatR;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.SpecialityEntity.Responses;

namespace UserService.Application.CQRS.SpecialityEntity.Queries.GetSpecialityById;

public class GetSpecialityByIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GetSpecialityByIdQuery, SpecialityDto>
{
    public async Task<SpecialityDto> Handle(
        GetSpecialityByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var speciality = await DbContext.Specialities.FindAsync(
            new object?[] { request.Id, cancellationToken },
            cancellationToken: cancellationToken
        );

        if (speciality == null)
        {
            throw new SpecialityNotFoundException(request.Id);
        }

        return speciality.Adapt<SpecialityDto>();
    }
}
