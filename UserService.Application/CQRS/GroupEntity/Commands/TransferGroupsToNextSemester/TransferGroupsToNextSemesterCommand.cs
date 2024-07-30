using MediatR;
using UserService.Domain.Entities;

namespace UserService.Application.CQRS.GroupEntity.Commands.TransferGroupsToNextSemester;

public record TransferGroupsToNextSemesterCommand(List<int> IdGroups) : IRequest<List<Group>>;