using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Queries.GetGroupById;

public class GetGroupByIdQueryHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<GetGroupByIdQuery, GroupDto>
{
    public async Task<GroupDto> Handle(
        GetGroupByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var group = await DbContext.Groups.FirstOrDefaultAsync(
            x => x.Id == request.Id,
            cancellationToken
        );

        if (group == null)
        {
            throw new GroupNotFoundException(request.Id);
        }

        return group.Adapt<GroupDto>();
    }
}
