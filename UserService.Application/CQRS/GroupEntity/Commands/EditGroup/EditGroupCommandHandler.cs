using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.EditGroup;

public class EditGroupCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<EditGroupCommand, GroupShortInfoDto>
{
    public async Task<GroupShortInfoDto> Handle(
        EditGroupCommand request,
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

        var speciality = await DbContext.Specialities.FindAsync(
            new object?[] { request.SpecialityId, cancellationToken },
            cancellationToken: cancellationToken
        );

        if (speciality == null)
        {
            throw new SpecialityNotFoundException(request.SpecialityId);
        }

        var curator = await DbContext.Teachers.FindAsync(
            new object?[] { request.CuratorId, cancellationToken },
            cancellationToken: cancellationToken
        );

        if (curator == null)
        {
            throw new TeacherNotFoundException(request.CuratorId);
        }

        group.CuratorId = request.CuratorId;
        group.SpecialityId = request.SpecialityId;
        group.SubGroup = request.SubGroup;
        group.CurrentSemester = request.CurrentSemester;
        group.CurrentCourse = request.CurrentCourse;
        group.IsDeleted = request.IsDeleted;

        await DbContext.SaveChangesAsync(cancellationToken);

        return group.Adapt<GroupShortInfoDto>();
    }
}
