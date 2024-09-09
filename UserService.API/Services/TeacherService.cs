using Grpc.Core;
using Mapster;
using MediatR;
using UserService.Application.CQRS.TeacherEntity.Commands.CreateTeacher;

namespace UserService.API.Services;

public class TeacherService(IMediator mediator) : UserService.TeacherService.TeacherServiceBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<TeacherShortInfo> CreateTeacher(
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

        var teacher = await _mediator.Send(command);

        return teacher.Adapt<TeacherShortInfo>();
    }
}
