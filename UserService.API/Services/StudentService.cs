using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using UserService.Application.CQRS.Student.Commands.DeleteStudent;
using UserService.Application.CQRS.Student.Commands.DropOutStudent;
using UserService.Application.CQRS.Student.Commands.EditStudent;
using UserService.Application.CQRS.Student.Quereis;
using UserService.Application.CQRS.Student.Queries.GetStudentBySsoId;
using UserService.Application.CQRS.Student.Queries.GetStudents;
using UserService.Application.Student.Commands.CreateStudent;

namespace UserService.API.Services;

public class StudentService(IMediator mediator) : Student.StudentBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<CreateStudentResponse> CreateStudent(CreateStudentRequest request,
        ServerCallContext context)
    {
        var command = new CreateStudentCommand(Guid.Parse(request.Id), request.FirstName, request.LastName,
            request.PatronymicName,
            request.GroupId);

        var id = await _mediator.Send(command);

        return new CreateStudentResponse
        {
            Id = id.ToString()
        };
    }

    public override async Task<DropOutStudentResponse> DropOutStudent(DropOutStudentRequest request,
        ServerCallContext context)
    {
        var command = new DropOutStudentCommand(Guid.Parse(request.Id), request.DroppedTime.ToDateTime());

        var id = await _mediator.Send(command);

        return new DropOutStudentResponse
        {
            Id = id.ToString(),
        };
    }

    public override async Task<DeleteStudentResponse> DeleteStudent(DeleteStudentRequest request,
        ServerCallContext context)
    {
        var command = new DeleteStudentCommand(Guid.Parse(request.Id));

        var id = await _mediator.Send(command);

        return new DeleteStudentResponse
        {
            Id = id.ToString(),
        };
    }

    public override async Task<EditStudentResponse> EditStudent(EditStudentRequest request, ServerCallContext context)
    {
        var command = new EditStudentCommand(Guid.Parse(request.Id), request.FirstName, request.LastName,
            request.PatronymicName,
            request.GroupId);

        var id = await _mediator.Send(command);

        return new EditStudentResponse
        {
            Id = id.ToString(),
        };
    }

    public override async Task<StudentModel> GetStudentById(GetStudentByIdRequest request,
        ServerCallContext context)
    {
        var query = new GetStudentByIdQuery(Guid.Parse(request.Id));

        var student = await _mediator.Send(query);

        return new StudentModel
        {
            Id = student.Id.ToString(),
            SsoId = student.SsoId.ToString(),
            FistName = student.FirstName,
            LastName = student.LastName,
            PatronymicName = student.PatronymicName,
            GroupId = student.GroupId,
            IsDropped = student.DroppedOutAt is not null,
            DroppedTime = Timestamp.FromDateTime(student.DroppedOutAt.Value)
        };
    }

    public override async Task<StudentModel> GetStudentBySsoId(GetStudentBySsoIdRequest request,
        ServerCallContext context)
    {
        var query = new GetStudentBySsoIdQuery(Guid.Parse(request.SsoId));

        var student = await _mediator.Send(query);

        return new StudentModel
        {
            Id = student.Id.ToString(),
            SsoId = student.SsoId.ToString(),
            FistName = student.FirstName,
            LastName = student.LastName,
            PatronymicName = student.PatronymicName,
            GroupId = student.GroupId,
            IsDropped = student.DroppedOutAt is not null,
            DroppedTime = Timestamp.FromDateTime(student.DroppedOutAt.Value)
        };
    }

    public override async Task<GetStudentsResponse> GetStudents(GetStudentsRequest request, ServerCallContext context)
    {
        var query = new GetStudentsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SearchString = request.SearchString,
            SortState = (Application.CQRS.Student.Queries.GetStudents.SortState)request.SortState,
        };

        var students = await _mediator.Send(query);

        return new GetStudentsResponse
        {
            Students =
            {
                students.Select(x => new StudentModel
                {
                    Id = x.Id.ToString(),
                    SsoId = x.SsoId.ToString(),
                    FistName = x.FirstName,
                    LastName = x.LastName,
                    PatronymicName = x.PatronymicName,
                    GroupId = x.GroupId,
                    IsDropped = x.DroppedOutAt is not null,
                    DroppedTime = Timestamp.FromDateTime(x.DroppedOutAt.Value)
                })
            }
        };
    }
}