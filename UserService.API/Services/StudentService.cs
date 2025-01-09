using Grpc.Core;
using Mapster;
using MediatR;
using UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;
using UserService.Application.CQRS.StudentEntity.Commands.EditStudent;
using UserService.Application.CQRS.StudentEntity.Commands.RecoveryStudents;
using UserService.Application.CQRS.StudentEntity.Commands.SoftDeleteStudents;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentsByGroupId;

namespace UserService.API.Services;

public class StudentService(IMediator mediator) : UserService.StudentService.StudentServiceBase
{
    private readonly IMediator _mediator = mediator;

    public override async Task<StudentShortInfo> CreateStudent(
        CreateStudentRequest request,
        ServerCallContext context
    )
    {
        var command = new CreateStudentCommand(
            Guid.Parse(request.SsoId),
            request.FirstName,
            request.LastName,
            request.PatronymicName,
            request.GroupId
        );

        var student = await _mediator.Send(command);

        return student.Adapt<StudentShortInfo>();
    }

    public override async Task<DropOutStudentsResponse> DropOutStudents(
        DropOutStudentsRequest request,
        ServerCallContext context
    )
    {
        var command = new DropOutStudentsCommand(
            [.. request.Ids.Select(x => Guid.Parse(x))],
            DateTime.Parse(request.DroppedTime)
        );

        var students = await _mediator.Send(command);

        return students.Adapt<DropOutStudentsResponse>();
    }

    public override async Task<DeleteStudentsResponse> DeleteStudents(
        DeleteStudentsRequest request,
        ServerCallContext context
    )
    {
        var command = new DeleteStudentsCommand([.. request.Ids.Select(x => Guid.Parse(x))]);

        var student = await _mediator.Send(command);

        return student.Adapt<DeleteStudentsResponse>();
    }

    public override async Task<SoftDeleteStudentsResponse> SoftDeleteStudents(
        SoftDeleteStudentsRequest request,
        ServerCallContext context
    )
    {
        var command = new SoftDeleteStudentsCommand([.. request.Ids.Select(x => Guid.Parse(x))]);

        var students = await _mediator.Send(command);

        return students.Adapt<SoftDeleteStudentsResponse>();
    }

    public override async Task<RecoveryStudentsResponse> RecoveryStudents(
        RecoveryStudentsRequest request,
        ServerCallContext context
    )
    {
        var command = new RecoveryStudentsCommand([.. request.Ids.Select(x => Guid.Parse(x))]);

        var students = await _mediator.Send(command);

        return students.Adapt<RecoveryStudentsResponse>();
    }

    public override async Task<StudentShortInfo> EditStudent(
        EditStudentRequest request,
        ServerCallContext context
    )
    {
        var command = new EditStudentCommand(
            Guid.Parse(request.Id),
            request.FirstName,
            request.LastName,
            request.PatronymicName,
            request.GroupId,
            request.IsDeleted
        );

        var student = await _mediator.Send(command);

        return student.Adapt<StudentShortInfo>();
    }

    public override async Task<StudentModel> GetStudentById(
        GetStudentByIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetStudentByIdQuery(Guid.Parse(request.Id));

        var student = await _mediator.Send(query);

        return student.Adapt<StudentModel>();
    }

    public override async Task<StudentModel> GetStudentBySsoId(
        GetStudentBySsoIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetStudentBySsoIdQuery(Guid.Parse(request.SsoId));

        var student = await _mediator.Send(query);

        return student.Adapt<StudentModel>();
    }

    public override async Task<GetStudentsByGroupIdResponse> GetStudentsByGroupId(
        GetStudentsByGroupIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetStudentsByGroupIdQuery(request.GroupId);

        var students = await _mediator.Send(query);

        return students.Adapt<GetStudentsByGroupIdResponse>();
    }

    public override async Task<GetStudentsResponse> GetStudents(
        GetStudentsRequest request,
        ServerCallContext context
    )
    {
        var query = new GetStudentsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SearchString = request.SearchString,
            SortState = (Application.CQRS.StudentEntity.Queries.GetStudents.SortState)
                request.SortState,
            DroppedOutStatus = (StudentDroppedOutStatus)request.DroppedOutStatus,
            DeletedStatus = (Application.Enums.DeletedStatus)request.DeletedStatus,
        };

        var students = await _mediator.Send(query);

        return students.Adapt<GetStudentsResponse>();
    }
}
