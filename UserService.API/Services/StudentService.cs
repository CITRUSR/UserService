using Grpc.Core;
using MediatR;
using UserService.Application.CQRS.Student.Commands.DropOutStudent;
using UserService.Application.Student.Commands.CreateStudent;

namespace UserService.API.Services;

public class StudentService(IMediator mediator) : UserService.Student.StudentBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<CreateStudentResponse> CreateStudent(CreateStudentRequest request,
        ServerCallContext context)
    {
        var command = new CreateStudentCommand(request.Id, request.FirstName, request.LastName, request.PatronymicName,
            request.GroupId);

        var id = await _mediator.Send(command);

        return new CreateStudentResponse
        {
            Id = id
        };
    }

    public override async Task<DropOutStudentResponse> DropOutStudent(DropOutStudentRequest request,
        ServerCallContext context)
    {
        var command = new DropOutStudentCommand(request.Id, request.DroppedTime.ToDateTime());

        var id = await _mediator.Send(command);

        return new DropOutStudentResponse
        {
            Id = id,
        };
    }
}