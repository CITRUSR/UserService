using Grpc.Core;
using MediatR;
using UserService.Application.CQRS.TeacherEntity.Commands.CreateTeacher;

namespace UserService.API.Services;

public class TeacherService(IMediator mediator) : UserService.TeacherService.TeacherServiceBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<CreateTeacherResponse> CreateTeacher(
        CreateTeacherRequest request,
        ServerCallContext context
    )
    {
        var command = new CreateTeacherCommand(
            Guid.Parse(request.SsoId),
            request.FirstName,
            request.LastName,
            request.PatronymicName,
            (short)request.RoomId
        );

        var id = await _mediator.Send(command);

        return new CreateTeacherResponse { Id = id.ToString(), };
    }
}
