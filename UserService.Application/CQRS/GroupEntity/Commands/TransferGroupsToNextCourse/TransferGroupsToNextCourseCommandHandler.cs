﻿using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Abstraction;
using UserService.Application.Common.Exceptions;
using UserService.Application.CQRS.GroupEntity.Responses;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextCourse;

public class TransferGroupsToNextCourseCommandHandler(IAppDbContext dbContext)
    : HandlerBase(dbContext),
        IRequestHandler<TransferGroupsToNextCourseCommand, List<GroupShortInfoDto>>
{
    public async Task<List<GroupShortInfoDto>> Handle(
        TransferGroupsToNextCourseCommand request,
        CancellationToken cancellationToken
    )
    {
        var groups = await DbContext
            .Groups.Where(x => request.IdGroups.Contains(x.Id))
            .Include(x => x.Speciality)
            .ToListAsync(cancellationToken);

        if (groups.Count < request.IdGroups.Count)
        {
            var notFoundIds = request.IdGroups.Except(groups.Select(x => x.Id));

            throw new GroupNotFoundException([.. notFoundIds]);
        }

        try
        {
            await DbContext.BeginTransactionAsync();

            if (!TryIncreaseCourse(groups, out var invalidGroups))
            {
                throw new GroupCourseOutOfRangeException([.. invalidGroups]);
            }

            await DbContext.SaveChangesAsync(cancellationToken);

            await DbContext.CommitTransactionAsync();
        }
        catch (Exception e)
        {
            await DbContext.RollbackTransactionAsync();
            throw;
        }

        return groups.Adapt<List<GroupShortInfoDto>>();
    }

    private bool TryIncreaseCourse(List<Group> groups, out ICollection<Group> invalidGroups)
    {
        invalidGroups = [];

        foreach (var group in groups)
        {
            var maxCourse = Math.Ceiling(group.Speciality.DurationMonths / 12.0);

            if (group.CurrentCourse + 1 > maxCourse)
            {
                invalidGroups.Add(group);
            }
            else
            {
                group.CurrentCourse++;
            }
        }

        if (invalidGroups.Count != 0)
        {
            return false;
        }

        return true;
    }
}
