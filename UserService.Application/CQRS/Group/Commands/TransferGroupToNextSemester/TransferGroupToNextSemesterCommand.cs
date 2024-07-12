using MediatR;

namespace UserService.Application.CQRS.Group.Commands.TransferGroupToNextSemester;

public record TransferGroupToNextSemesterCommand(int GroupId) : IRequest<int>;