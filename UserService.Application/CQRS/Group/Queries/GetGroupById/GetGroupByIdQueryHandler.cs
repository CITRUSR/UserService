using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Common.Exceptions;

namespace UserService.Application.CQRS.Group.Queries.GetGroupById;

public class GetGroupByIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<GetGroupByIdQuery, Domain.Entities.Group>
{
    public async Task<Domain.Entities.Group> Handle(GetGroupByIdQuery request, CancellationToken cancellationToken)
    {
        var group = await DbContext.Groups.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(request.Id);
        }

        return group;
    }
}