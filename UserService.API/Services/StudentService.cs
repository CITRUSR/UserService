using Grpc.Core;
using Mapster;
using MediatR;
using UserService.API.Mappers;
using UserService.Application.CQRS.StudentEntity.Commands.CreateStudent;
using UserService.Application.CQRS.StudentEntity.Commands.DeleteStudents;
using UserService.Application.CQRS.StudentEntity.Commands.DropOutStudents;
using UserService.Application.CQRS.StudentEntity.Commands.EditStudent;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentById;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudentBySsoId;
using UserService.Application.CQRS.StudentEntity.Queries.GetStudents;
using UserService.Domain.Entities;

namespace UserService.API.Services;

public class StudentService(IMediator mediator, IMapper<Student, StudentModel> mapper)
    : UserService.StudentService.StudentServiceBase
{
    private readonly IMediator _mediator = mediator;
    private readonly IMapper<Student, StudentModel> _mapper = mapper;

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
            request.DroppedTime.ToDateTime()
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

    public override async Task<EditStudentResponse> EditStudent(
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

        return new EditStudentResponse { Student = _mapper.Map(student), };
    }

    public override async Task<StudentModel> GetStudentById(
        GetStudentByIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetStudentByIdQuery(Guid.Parse(request.Id));

        var student = await _mediator.Send(query);

        return _mapper.Map(student);
    }

    public override async Task<StudentModel> GetStudentBySsoId(
        GetStudentBySsoIdRequest request,
        ServerCallContext context
    )
    {
        var query = new GetStudentBySsoIdQuery(Guid.Parse(request.SsoId));

        var student = await _mediator.Send(query);

        return _mapper.Map(student);
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

        return new GetStudentsResponse
        {
            Students = { students.Items.Select(x => _mapper.Map(x)) },
            LastPage = students.MaxPage,
        };
    }
}
