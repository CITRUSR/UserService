using MediatR;
using UserService.Application.CQRS.GroupEntity.Responses;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;

public record TransferGroupsToNextSemesterCommand(List<int> IdGroups)
    : IRequest<List<GroupShortInfoDto>>;
