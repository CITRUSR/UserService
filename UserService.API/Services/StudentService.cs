﻿using Grpc.Core;
using MediatR;
using UserService.Application.CQRS.Student.Commands.DeleteStudent;
using UserService.Application.CQRS.Student.Commands.DropOutStudent;
using UserService.Application.CQRS.Student.Commands.EditStudent;
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

    public override async Task<DeleteStudentResponse> DeleteStudent(DeleteStudentRequest request,
        ServerCallContext context)
    {
        var command = new DeleteStudentCommand(request.Id);

        var id = await _mediator.Send(command);

        return new DeleteStudentResponse
        {
            Id = id,
        };
    }

    public override async Task<EditStudentResponse> EditStudent(EditStudentRequest request, ServerCallContext context)
    {
        var command = new EditStudentCommand(request.Id, request.FirstName, request.LastName, request.PatronymicName,
            request.GroupId);

        var id = await _mediator.Send(command);

        return new EditStudentResponse
        {
            Id = id,
        };
    }
}