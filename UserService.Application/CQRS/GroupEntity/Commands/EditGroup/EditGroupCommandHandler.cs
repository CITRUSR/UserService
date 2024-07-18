using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UserService.Application.Common.Exceptions;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.EditGroup;

public class EditGroupCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext), IRequestHandler<EditGroupCommand, int>
{
    public async Task<int> Handle(EditGroupCommand request, CancellationToken cancellationToken)
    {
        var group = await DbContext.Groups.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (group == null)
        {
            throw new GroupNotFoundException(request.Id);
        }

        var speciality = await DbContext.Specialities.FindAsync(
            new object?[] { request.SpecialityId, cancellationToken }, cancellationToken: cancellationToken);

        if (speciality == null)
        {
            throw new SpecialityNotFoundException(request.SpecialityId);
        }

        var curator = await DbContext.Teachers.FindAsync(new object?[] { request.CuratorId, cancellationToken },
            cancellationToken: cancellationToken);

        if (curator == null)
        {
            throw new TeacherNotFoundException(request.CuratorId);
        }

        var oldGroup = new Group
        {
            Id = group.Id,
            SpecialityId = group.SpecialityId,
            CuratorId = group.CuratorId,
            CurrentCourse = group.CurrentCourse,
            CurrentSemester = group.CurrentSemester,
            SubGroup = group.SubGroup,
            StartedAt = group.StartedAt,
            GraduatedAt = group.GraduatedAt,
        };

        group.CuratorId = request.CuratorId;
        group.SpecialityId = request.SpecialityId;
        group.SubGroup = request.SubGroup;
        group.CurrentSemester = request.CurrentSemester;
        group.CurrentCourse = request.CurrentCourse;

        await DbContext.SaveChangesAsync(cancellationToken);

        Log.Information($"The group with id:{request.Id} is updated" + "old state:{@oldGroup} new state:{@group}");

        return request.Id;
    }
}